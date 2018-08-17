
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using TrackingApp.Classes;
using TrackingApp.Models;
using TrackingApp.Services;

namespace TrackingApp.ViewModels
{
    public class DashboardViewModel : BaseViewModel
    {
        #region Attributes
        private ApiService apiService;
        private DialogService dialogService;
        private NavigationService navigationService;
        private bool isRefreshing;
        private bool isEnabled;
        private bool showChart;
        private bool showResultEmpty;
        private bool showProjects;
        private bool showTasks;
        private bool showDate;

        private string titleChart;
        private int queryType;
        private int catalogueType;
        private int taskId;
        private int methodologyId;
        private int pojectId;
        private DateTime? startDate;
        private DateTime? endDate;
        private List<PersonalState> personalState;
        private List<QueryType> queryTypeList;
        private List<Project> projectsList;
        private List<Models.Task> tasksList;

        #endregion

        #region Properties
                
        public List<Project> ProjectsList
        {
            set { SetValue<List<Project>>(ref projectsList, value); }
            get { return projectsList; }
        }

        public List<Models.Task> TasksList
        {
            set { SetValue<List<Models.Task>>(ref tasksList, value); }
            get { return tasksList; }
        }

        public List<QueryType> QueryTypeList
        {
            set { SetValue<List<QueryType>>(ref queryTypeList, value); }
            get { return queryTypeList; }
        }

        public List<PersonalState> PersonalState
        {
            set { SetValue<List<PersonalState>>(ref personalState, value); }
            get { return personalState; }
        }

        public bool IsRefreshing
        {
            set { SetValue<bool>(ref isRefreshing, value); }
            get { return isRefreshing; }
        }

        public bool IsEnabled
        {
            set { SetValue(ref isEnabled, value); }
            get { return isEnabled; }
        }

        public bool ShowChart
        {
            set { SetValue(ref showChart, value); }
            get { return showChart; }
        }

        public bool ShowEmptyChart
        {
            get { return !showChart; }
        }

        public bool ShowResultEmpty
        {
            set { SetValue(ref showResultEmpty, value); }
            get { return showResultEmpty; }
        }

        public bool ShowList
        {
            get { return !showResultEmpty; }
        }

        public bool ShowProjects
        {
            set { SetValue(ref showProjects, value); }
            get { return showProjects; }
        }

        public bool ShowTasks
        {
            set { SetValue(ref showTasks, value); }
            get { return showTasks; }
        }

        public bool ShowDate
        {
            set { SetValue(ref showDate, value); }
            get { return showDate; }
        }

        public string TitleChart
        {
            set { SetValue(ref titleChart, value); }
            get { return titleChart; }
        }

        public int QueryType
        {
            set { SetValue(ref queryType, value);
                SetVisibleQueryData();
            }
            get { return queryType; }
        }

        public int CatalogueType
        {
            set { SetValue(ref catalogueType, value); }
            get { return catalogueType; }
        }

        public int TaskId
        {
            set { SetValue(ref taskId, value); }
            get { return taskId; }
        }

        public int MethodologyId
        {
            set { SetValue(ref methodologyId, value); }
            get { return taskId; }
        }

        public int PojectId
        {
            set { SetValue(ref pojectId, value);
            }
            get { return pojectId; }
        }

        public DateTime? StartDate
        {
            set { SetValue(ref startDate, value); }
            get { return startDate; }
        }

        public DateTime? EndDate
        {
            set { SetValue(ref endDate, value); }
            get { return endDate; }
        }
        #endregion

        #region Constructor
        public DashboardViewModel()
        {
            instance = this;
            apiService = new ApiService();
            dialogService = new DialogService();
            navigationService = new NavigationService();
            StartDate = DateTime.Today;
            EndDate = DateTime.Today;
            IsEnabled = true;
        }
        #endregion

        #region Singleton
        private static DashboardViewModel instance;
        public static DashboardViewModel GetInstance()
        {
            if (instance == null)
            {
                return new DashboardViewModel();
            }
            return instance;
        }
        #endregion

        #region Commands

        public ICommand ViewChartCommand
        {
            get
            {
                return new RelayCommand(ViewChart);
            }
        }

        private async void ViewChart()
        {
            //Cargar datos
            IsEnabled = false;
            IsRefreshing = true;
            this.PersonalState?.Clear();
            this.PersonalState = await GetPersonalState();
            IsEnabled = true;
            IsRefreshing = false;
            if (PersonalState != null)
            {
                await navigationService.Navigate("ChartPage");
            }
        }

        public ICommand BackCommand
        {
            get
            {
                return new RelayCommand(Back);
            }
        }

        private async void Back()
        {
            IsEnabled = false;
            await navigationService.CloseModal();
            IsEnabled = true;
        }

        #endregion

        #region Methods

        private async Task<List<PersonalState>> GetPersonalState()
        {
            this.ShowResultEmpty = false;
            this.ShowChart = false;
            var result = new List<PersonalState>();
            try
            {
                var modelMain = MainViewModel.GetInstance();
                //Se obtiene el catalogo relacionado a la consulta
                switch (this.QueryType)
                {
                    case 2:
                        this.CatalogueType = Constant.EstadoTarea;
                        break;
                    case 4:
                        this.CatalogueType = Constant.EstadoLabor;
                        break;
                    case 6:
                        this.CatalogueType = Constant.Prioridad;
                        break;
                    default:
                        this.CatalogueType = Constant.Clasificacion;
                        break;
                }

                var response = await apiService.Get<PersonalState>(modelMain.urlBase, "/api", string.Format("/EstadoPersonalAPI/GetEstadoPersonal/{0}/{1}/{2}/{3}/{4}/{5}/{6}/{7}/{8}/{9}/{10}",
                    modelMain.CurrentUser.UserId, this.TaskId, this.QueryType, this.PojectId, this.StartDate.Value.ToString("yyyy-M-d"), this.EndDate.Value.ToString("yyyy-M-d"), this.MethodologyId, this.CatalogueType, modelMain.CurrentUser.UserCode,
                    ResourceCodes.DciConsultarEstadoPersonal, "1"));
                if (response.IsSuccess)
                {
                    result = (List<PersonalState>)response.Result;
                    var length = result.Count;
                    var colorLength = Colors.Length;
                    var idColor = 0;
                    if (length > 0)
                    {
                        this.TitleChart = result.FirstOrDefault().Title;
                        double datos = 0;
                        for (int i = 0; i < length; i++)
                        {
                            result[i].Color = Colors[idColor];
                            idColor++;
                            if (idColor == colorLength)
                            {
                                idColor = 0;
                            }
                            datos += result[i].Value;
                        }
                        if (datos > 0)
                        {
                            this.ShowChart = true;
                        }
                    }
                    else
                    {
                        this.ShowResultEmpty = true;
                        this.TitleChart = this.QueryTypeList.FirstOrDefault(e => e.QueryTypeId == this.QueryType).Description.ToUpper();
                    }
                }
                else { return null; }
            }
            catch (Exception ex)
            {
                dialogService.ShortToast("Error al obtener los datos.");
                await navigationService.Clear();
                return null;
            }
            return result;
        }

        public async Task<bool> LoadProjects()
        {
            try
            {
                var modelMain = MainViewModel.GetInstance();
                var response = await apiService.Get<Project>(modelMain.urlBase, "/api", string.Format("/ProyectoAPI/GetProyectosRecursoTracking/{0}/0", modelMain.CurrentUser.UserId));
                if (!response.IsSuccess){ return false; }
                var listProjects = (List<Project>)response.Result;
                listProjects.Add(new Project { ProjectName = "Todos los proyectos", ProjectId=0 });
                this.ProjectsList = listProjects;
            }
            catch (Exception ex)
            {
                dialogService.ShortToast("Error al obtener los datos.");
                await navigationService.Clear();
            }
            return true;
        }

        public async Task<bool> LoadTasks()
        {
            try
            {
                var modelMain = MainViewModel.GetInstance();
                var response = await apiService.Get<Models.Task>(modelMain.urlBase, "/api", string.Format("/TareaAPI/GetTareasProyecto/{0}/{1}/7/{2}/{3}", modelMain.CurrentUser.UserId,this.PojectId, Constant.Prioridad, Constant.EstadoTarea));
                if(!response.IsSuccess)
                { return false; }
                var listTasks = (List<Models.Task>)response.Result;
                listTasks.Add(new Models.Task { TaskName = "Todas las tareas", TaskId = 0 });
                this.TasksList = listTasks;
            }
            catch (Exception ex)
            {
                dialogService.ShortToast("Error al obtener los datos.");
                await navigationService.Clear();
            }
            return true;
        }

        public string[] Colors =
        {
            "#2c3e50",
            "#77d065",
            "#b455b6",
            "#3498db",
            "#ffa500",
            "#6a5acd",
            "#787878",
            "#979B2D",
            "#9B722D",
            "#73F30E"
        };

        public void GetQueryTypeList()
        {
            this.QueryTypeList = new List<QueryType>
            {
                new QueryType{ QueryTypeId = 3, Description = "Tareas por Proyecto"},//Fechas
                new QueryType{ QueryTypeId = 16, Description = "Tareas Atrasadas por Proyecto"},//
                new QueryType{ QueryTypeId = 2, Description = "Tareas según Estado | Proyecto"},//Fechas//Proyecto
                new QueryType{ QueryTypeId = 5, Description = "Tareas según Tipo | Proyecto"},//Fechas//Proyecto
                new QueryType{ QueryTypeId = 4, Description = "Labores según Tarea | Estado"},//Fechas//Proyecto//Tarea
                new QueryType{ QueryTypeId = 10, Description = "Horas Trabajadas por Proyecto"},//Fechas
                new QueryType{ QueryTypeId = 12, Description = "Horas según Tipo de Actividad"},//Fechas
                new QueryType{ QueryTypeId = 11, Description = "Horas según Tarea | Proyecto | Labores Finalizadas"}//Fechas//Proyecto
            };
            LoadProjects();
        }

        private void SetVisibleQueryData()
        {
            switch (this.QueryType)
            {
                case 16:
                    this.ShowProjects = false;
                    this.ShowTasks = false;
                    this.ShowDate = false;
                    break;
                case 2:
                case 5:
                case 11:
                    this.ShowProjects = true;
                    this.ShowTasks = false;
                    this.ShowDate = true;
                    break;
                case 4:
                    this.ShowProjects = true;
                    this.ShowTasks = true;
                    this.ShowDate = true;
                    break;
                default:
                    this.ShowProjects = false;
                    this.ShowTasks = false;
                    this.ShowDate = true;
                    break;
            }
        }

        #endregion
    }
}