using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using TrackingApp.Classes;
using TrackingApp.Models;
using TrackingApp.Services;

namespace TrackingApp.ViewModels
{
    public class ProjectsViewModel:BaseViewModel
    {

        #region Attributes
        private ApiService apiService;
        private DialogService dialogService;
        private DataService dataService;
        private NavigationService navigationService;
        private bool isRefreshing;
        #endregion

        #region Properties
        public ObservableCollection<ProjectItemViewModel> ProjectsList { get; set; }

        public List<Project> Projects { get; set; }

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
        public ProjectsViewModel()
        {
            instance = this;
            apiService = new ApiService();
            dialogService = new DialogService();
            navigationService = new NavigationService();
            dataService = new DataService();
            ProjectsList = new ObservableCollection<ProjectItemViewModel>();
        }
        #endregion

        #region Singleton
        private static ProjectsViewModel instance;
        public static ProjectsViewModel GetInstance()
        {
            if (instance == null)
            {
                return new ProjectsViewModel();
            }
            return instance;
        }
        #endregion

        #region Methods
        public async Task<bool> LoadProjects(int? userId=null)
        {
            try
            {
                var modelMain = MainViewModel.GetInstance();
                if (userId == null && modelMain.CurrentUser != null)
                {
                    userId = modelMain.CurrentUser.UserId;
                }
                var response = await apiService.Get<Project>(modelMain.urlBase, "/api", string.Format("/ProyectoAPI/GetProyectosRecursoTracking/{0}/1", userId));
                if (!response.IsSuccess) { return false; }
                this.Projects = (List<Project>)response.Result;
                foreach (var project in Projects)
                {
                    var list = new List<Models.Task>();
                    try
                    {
                        var responseTasks = await apiService.Get<Models.Task>(modelMain.urlBase, "/api", string.Format("/TareaAPI/GetTareasProyecto/{0}/{1}/1/{2}/{3}", userId, project.ProjectId, Constant.Prioridad, Constant.EstadoTarea));
                        if (!responseTasks.IsSuccess) { return false; }
                        list = (List<Models.Task>)responseTasks.Result;
                    }
                    catch
                    {
                    }
                    project.TasksCount = list?.Count;
                }
                modelMain.TrackingRegister = false;
                if (Projects != null && Projects.Count > 0)
                {
                    modelMain.TrackingRegister = true;
                }
                var reload = false;
                var user = dataService.GetUser<User>(false);
                if (user != null)
                {
                    reload = user.TrackingRegister != modelMain.TrackingRegister;
                    user.TrackingRegister = modelMain.TrackingRegister;
                    modelMain.SetCurrentUser(user);
                    dataService.DeleteAllUsersAndInsert(user);
                }
                dataService.DeleteAllProjectsAndInsert(Projects);
                if (reload)
                {
                    navigationService.SetMainPage("ProjectsTabbedPage");
                    this.ReloadProjects();
                }
            }
            catch (Exception ex)
            {
                dialogService.ShortToast("Error al obtener los datos.");
                await navigationService.Clear();
            }
            return true;
        }

        public void ReloadProjects()
        {
            ProjectsList.Clear();
            foreach (var project in Projects)
            {
                ProjectsList.Add(project.Cast<ProjectItemViewModel>());
            }
        }

        public List<Project> GetListProjects()
        {
            Projects = dataService.GetProjects<Project>();
            return Projects;
        }

        #endregion

        #region Commands
        public ICommand RefreshCommand { get { return new RelayCommand(Refresh); } }

        public async void Refresh()
        {
            IsRefreshing = true;
            await LoadProjects();
            GetListProjects();
            ReloadProjects();
            IsRefreshing = false;
        }
               
        #endregion

    }
}
