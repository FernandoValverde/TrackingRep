using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TrackingApp.Classes;
using TrackingApp.Models;
using TrackingApp.Services;

namespace TrackingApp.ViewModels
{
    public class LaborsViewModel : BaseViewModel
    {

        #region Attributes
        private ApiService apiService;
        private DialogService dialogService;
        private NavigationService navigationService;
        private bool isRefreshing;
        private bool isEnabled;
        private bool canEditTask;
        private int selectedTask;
        
        private int taskId;
        private string projectName;
        private string taskName;
        private string description;
        private string detail;
        private DateTime? creationDate;
        private DateTime? limitDate;
        private double estimationTime;
        private double workedTime;
        private int stateId;
        private string state;
        private int priorityId;
        private string priority;
        private bool backward;
        private string stage;
        private string colorText;
        private string stimateColorText;
        #endregion

        #region Properties
        public ObservableCollection<LaborItemViewModel> LaborsList { get; set; }
        public ObservableCollection<LaborItemViewModel> OtherLaborsList { get; set; }

        public int SelectedTask
        {
            get { return this.selectedTask; }
            set { SetValue(ref this.selectedTask, value); if (value > 0)
                { LoadLabors(); }
            }
        }

        public int TaskId {
            set { SetValue<int>(ref taskId, value); }
            get { return this.taskId; }
        }

        public string TaskName {
            set { SetValue<string>(ref taskName, value); }
            get { return this.taskName; }
        }

        public string ProjectName
        {
            set { SetValue<string>(ref projectName, value); }
            get { return this.projectName; }
        }

        public string Description {
            set { SetValue<string>(ref description, value); }
            get { return this.description; }
        }

        public string Detail {
            set { SetValue<string>(ref detail, value); }
            get { return this.detail; }
        }

        public DateTime? CreationDate {
            set { SetValue<DateTime?>(ref creationDate, value); }
            get { return this.creationDate; }
        }

        public DateTime? LimitDate {
            set { SetValue<DateTime?>(ref limitDate, value); }
            get { return this.limitDate; }
        }

        public double EstimationTime {
            set { SetValue<double>(ref estimationTime, value); }
            get { return this.estimationTime; }
        }

        public double WorkedTime {
            set { SetValue<double>(ref workedTime, value); }
            get { return this.workedTime; }
        }

        public int StateId {
            set { SetValue<int>(ref stateId, value); }
            get { return this.stateId; }
        }

        public string State {
            set { SetValue<string>(ref state, value); }
            get { return this.state; }
        }

        public int PriorityId {
            set { SetValue<int>(ref priorityId, value); }
            get { return this.priorityId; }
        }

        public string Priority {
            set { SetValue<string>(ref priority, value); }
            get { return this.priority; }
        }

        public bool Backward {
            set { SetValue<bool>(ref backward, value); }
            get { return this.backward; }
        }

        public string Stage
        {
            set { SetValue<string>(ref stage, value); }
            get { return this.stage; }
        }

        public bool IsRefreshing
        {
            set { SetValue<bool>(ref isRefreshing, value); }
            get { return isRefreshing; }
        }
        public bool IsEnabled
        {
            set { SetValue<bool>(ref isEnabled, value); }
            get { return isEnabled; }
        }
        public bool CanEditTask
        {
            set { SetValue<bool>(ref canEditTask, value); }
            get { return canEditTask; }
        }
        public string ColorText
        {
            set { SetValue<string>(ref colorText, value); }
            get { return this.colorText; }
        }
        public string StimateColorText
        {
            set { SetValue<string>(ref stimateColorText, value); }
            get { return this.stimateColorText; }
        }

        #endregion

        #region Constructor
        public LaborsViewModel()
        {
            instance = this;
            apiService = new ApiService();
            dialogService = new DialogService();
            navigationService = new NavigationService();
            LaborsList = new ObservableCollection<LaborItemViewModel>();
            OtherLaborsList = new ObservableCollection<LaborItemViewModel>();
            this.ColorText = "#000000";
            this.StimateColorText = "#000000";
        }
        #endregion

        #region Singleton
        private static LaborsViewModel instance;
        public static LaborsViewModel GetInstance()
        {
            if (instance == null)
            {
                return new LaborsViewModel();
            }
            return instance;
        }
        #endregion

        #region Methods
        private async Task<bool> LoadLabors()
        {
            try
            {
                var modelMain = MainViewModel.GetInstance();
                var task = await GetCurrentTaskDetail();
                if (task != null)
                {
                    modelMain.currentTask = task;
                    LoadTaskDetail();
                    var response = await apiService.Get<Labor>(modelMain.urlBase, "/api", string.Format("/LaborAPI/GetLaboresTarea/{0}/{1}/{2}/{3}", this.selectedTask, Constant.EstadoLabor, 3, modelMain.CurrentUser.UserId));
                    if (!response.IsSuccess) { return false; }
                    var labors = (List<Labor>)response.Result;
                    labors = labors.Select(c =>
                    {
                        c.Description =
                        c.Description == null ? null :
                        c.Description.Length <= 500 ? c.Description :
                        c.Description.Substring(0, 495) + "..."; return c;
                    }).ToList();
                    var otherLabors = labors.Where(l => l.Resource.UserId != modelMain.CurrentUser.UserId).ToList();
                    labors = labors.Where(l => l.Resource.UserId == modelMain.CurrentUser.UserId).ToList();
                    ReloadLabors(labors);
                    ReloadOtherLabors(otherLabors);
                }
            }
            catch (Exception ex)
            {
                dialogService.ShortToast("Error al obtener los datos.");
                await navigationService.Clear();
            }
            return true;
        }

        private void ReloadLabors(List<Labor> labors)
        {
            LaborsList.Clear();
            foreach (var labor in labors)
            {
                LaborsList.Add(labor.Cast<LaborItemViewModel>());
            }
        }

        private void ReloadOtherLabors(List<Labor> labors)
        {
            OtherLaborsList.Clear();
            foreach (var labor in labors)
            {
                OtherLaborsList.Add(labor.Cast<LaborItemViewModel>());
            }
        }

        private void LoadTaskDetail()
        {
            var modelMain = MainViewModel.GetInstance();
            this.TaskId = modelMain.currentTask.TaskId;
            this.TaskName = modelMain.currentTask.TaskName;
            this.Description = modelMain.currentTask.Description;
            this.Detail = modelMain.currentTask.Detail;
            this.CreationDate = modelMain.currentTask.CreationDate;
            this.LimitDate = modelMain.currentTask.LimitDate;
            this.EstimationTime = modelMain.currentTask.EstimationTime;
            this.WorkedTime = modelMain.currentTask.WorkedTime;
            this.StateId = modelMain.currentTask.StateId;
            this.State = modelMain.currentTask.State;
            this.PriorityId = modelMain.currentTask.PriorityId;
            this.Priority = modelMain.currentTask.Priority;
            this.Backward = modelMain.currentTask.Backward;
            this.Stage = modelMain.currentTask.Stage.StageName;
            this.ProjectName = modelMain.currentTask.Project.ProjectName;
            this.CanEditTask = this.StateId == Constant.Iniciar;
            this.ColorText = modelMain.currentTask.ColorText;
            this.StimateColorText= modelMain.currentTask.StimateColorText;
        }

        private async Task<bool> DetailTaskUpdate()
        {
            var oTask = new Models.Task();
            var mainModel = MainViewModel.GetInstance();
            try
            {
                this.Detail = this.Detail?.Trim();
                if (string.IsNullOrEmpty(this.Detail))
                {
                    dialogService.ShortToast("Debe ingresar un comentario.");
                    return false;
                }else if (this.Detail.Contains("#"))
                {
                    dialogService.ShortToast("El caracter # no está permitido.");
                    return false;
                }
                oTask.Resource = mainModel.CurrentUser;
                oTask.TaskId = this.TaskId;
                oTask.Detail = this.Detail;
                oTask.Action = ResourceCodes.DciAgregarLabor;
                oTask.LastUpDate = DateTime.Now;
                oTask.LastUpdateUser = mainModel.CurrentUser.UserCode;

                //Se llama al servicio que actualiza una labor en la base de datos y se obtiene la repuesta
                var response = await apiService.Put(mainModel.urlBase, "api/", "TareaAPI/PutDetalleTarea/", oTask);

                if (!response.IsSuccess)
                {
                    return false;
                }
                dialogService.ShortToast("Transacción procesada con éxito.");
                mainModel.currentTask.Detail = this.Detail;

            }
            catch (Exception ex)
            {
                await dialogService.ShowMessage("Error", "Ocurrió un error al guardar la información.");
            }
            return true;
        }

        private async System.Threading.Tasks.Task<Models.Task> GetCurrentTaskDetail()
        {
            try
            {
                var modelMain = MainViewModel.GetInstance();
                var response = await apiService.Get<Models.Task>(modelMain.urlBase, "/api", string.Format("/TareaAPI/GetInformacionTarea/{0}/{1}/{2}", modelMain.TaskSelected, modelMain.CurrentUser.UserId, Constant.EstadoTarea));
                if (response.IsSuccess)
                {
                    var taskList = (List<Models.Task>)response.Result;
                    return taskList.FirstOrDefault().Cast<Models.Task>();
                }
            }
            catch (Exception ex)
            {
                dialogService.ShortToast("Error al obtener los datos.");
                await navigationService.Clear();
            }
            return null;
        }

        #endregion

        #region Commands
        public ICommand RefreshCommand { get { return new RelayCommand(Refresh); } }

        public async void Refresh()
        {
            IsRefreshing = true;
            await LoadLabors();
            IsRefreshing = false;
        }

        public ICommand AddLaborCommand
        {
            get
            {
                return new RelayCommand(AddLabor);
            }
        }

        private async void AddLabor()
        {
            var mainViewModel = MainViewModel.GetInstance();
            mainViewModel.LaborSelected = 0;
            var laborEdit = LaborViewModel.GetInstance();
            laborEdit.Description =null;
            laborEdit.StartDate = DateTime.Today;
            laborEdit.StartDateBak = DateTime.Today;
            laborEdit.LaborId = 0;
            laborEdit.EstimationTime = 0;
            laborEdit.WorkedTime = 0;
            laborEdit.WorkedMin = 1;
            laborEdit.EstimationMin = 1;
            laborEdit.CanEditLabor = true;
            await navigationService.Navigate("LaborPage");
        }

        public ICommand SaveDetailTaskCommand
        {
            get
            {
                return new RelayCommand(SaveDetailTask);
            }
        }

        private async void SaveDetailTask()
        {
            IsRefreshing = true;
            IsEnabled = false;
            await DetailTaskUpdate();
            IsRefreshing = false;
            IsEnabled = true;
        }

        #endregion
    }
}
