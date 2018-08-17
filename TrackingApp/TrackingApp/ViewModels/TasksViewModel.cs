using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using TrackingApp.Classes;
using TrackingApp.Models;
using TrackingApp.Services;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Linq;

namespace TrackingApp.ViewModels
{
    public class TasksViewModel : BaseViewModel
    {

        #region Attributes
        private ApiService apiService;
        private DialogService dialogService;
        private NavigationService navigationService;
        private bool isRefreshing;
        private int selectedProject;
        #endregion

        #region Properties
        public ObservableCollection<TaskItemViewModel> TasksList { get; set; }

        public List<Project> ProjectsList { get; set; }

        public List<string> SuspendMessages { get; set; }

        public int SelectedProject
        {
            get { return this.selectedProject; }
            set { SetValue(ref this.selectedProject, value); }
        }

        public bool IsRefreshing
        {
            set
            {
                SetValue<bool>(ref isRefreshing, value);
            }
            get
            {
                return isRefreshing;
            }
        }
        #endregion

        #region Constructor
        public TasksViewModel()
        {
            instance = this;
            apiService = new ApiService();
            dialogService = new DialogService();
            navigationService = new NavigationService();
            TasksList = new ObservableCollection<TaskItemViewModel>();
            
        }
        #endregion

        #region Singleton
        private static TasksViewModel instance;
        public static TasksViewModel GetInstance()
        {
            if (instance == null)
            {
                return new TasksViewModel();
            }
            return instance;
        }
        #endregion

        #region Methods
        public async Task<bool> LoadTasks()
        {
            IsRefreshing = true;
            try
            {
                var modelMain = MainViewModel.GetInstance();
                modelMain.ProjectSelected = this.selectedProject;
                TasksList.Clear();
                var response = await apiService.Get<Models.Task>(modelMain.urlBase, "/api", string.Format("/TareaAPI/GetTareasProyecto/{0}/{1}/1/{2}/{3}", modelMain.CurrentUser.UserId, this.selectedProject, Constant.Prioridad, Constant.EstadoTarea));
                if (response.IsSuccess)
                {
                    ReloadTasks((List<Models.Task>)response.Result);
                    GetSuspendMessages();
                }
            }
            catch (Exception ex)
            {
                dialogService.ShortToast("Error al obtener los datos.");
                await navigationService.Clear();
            }
            IsRefreshing = false;
            return true;
        }

        private void ReloadTasks(List<Models.Task> tasks)
        {
            tasks = tasks.Where(t => t.StateId != Constant.Finalizar).ToList();
            TasksList.Clear();
            foreach (var task in tasks)
            {
                TasksList.Add(task.Cast<TaskItemViewModel>());
            }
        }

        //Metodo para obtener los mensajes de suspension del sistema
        private async void GetSuspendMessages()
        {
            try
            {
                SuspendMessages = new List<string>();
                var modelMain = MainViewModel.GetInstance();
                var result = await apiService.Get<SuspendMessage>(modelMain.urlBase, "api/", "MensajeSuspensionAPI/0");
                if (result.IsSuccess)
                {
                    var messages = (List<SuspendMessage>)result.Result;
                    SuspendMessages = (from m in messages select m.Description).ToList();
                }
            }
            catch (Exception ex) { }
        }

        #endregion

        #region Commands
        public ICommand RefreshCommand { get { return new RelayCommand(Refresh); } }

        public async void Refresh()
        {
            await LoadTasks();
        }

        #endregion
    }

    public class SuspendMessage
    {
        [JsonProperty(PropertyName = "Descripcion")]
        public string Description { get; set; }
    }
}
