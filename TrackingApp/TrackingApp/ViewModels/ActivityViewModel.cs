using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using TrackingApp.Classes;
using TrackingApp.Models;
using TrackingApp.Services;

namespace TrackingApp.ViewModels
{
    public class ActivityViewModel : BaseViewModel
    {
        #region Attributes
        private ApiService apiService;
        private DialogService dialogService;
        private NavigationService navigationService;
        private bool isRefreshing;
        private bool isEnabled;
        private bool isUpdating;

        private int clasification;
        private int activityId;
        private int activityTypeId;
        private string detail;
        private DateTime startDate;
        private DateTime endDate;
        private DateTime reversionDate;
        private double effortTime;
        private int effortMin;
        private int? projectId;
        private List<ActivityType> activityTypesList;
        #endregion

        #region Properties
        public List<KeyValuePair<int, string>> MinutesList { get; set; }
        public List<Project> ProjectsActivityList { get; set; }
        public List<ActivityType> ActivityTypesList {
            set { SetValue<List<ActivityType>>(ref activityTypesList, value); }
            get { return activityTypesList; }
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

        public bool IsUpdating
        {
            set { SetValue<bool>(ref isUpdating, value); }
            get { return isUpdating; }
        }

        public bool ShowProjects
        {
            get { return ProjectsActivityList!=null && ProjectsActivityList.Count>1; }
        }

        public int ActivityId
        {
            set { SetValue<int>(ref activityId, value); }
            get { return activityId; }
        }

        public int? ProjectId
        {
            set { SetValue<int?>(ref projectId, value); }
            get { return projectId; }
        }

        public int ActivityTypeId
        {
            set { SetValue<int>(ref activityTypeId, value); }
            get { return activityTypeId; }
        }

        public int Clasification
        {
            set {
                SetValue<int>(ref clasification, value);
            }
            get { return clasification; }
        }

        public string Detail
        {
            set { SetValue<string>(ref detail, value); }
            get { return detail; }
        }

        public DateTime StartDate
        {
            set { SetValue<DateTime>(ref startDate, value); }
            get { return startDate; }
        }

        public DateTime EndDate
        {
            set { SetValue<DateTime>(ref endDate, value); }
            get { return endDate; }
        }

        public DateTime ReversionDate
        {
            set { SetValue<DateTime>(ref reversionDate, value); }
            get { return reversionDate; }
        }

        public double EffortTime
        {
            set
            {
                SetValue<double>(ref effortTime, value);
            }
            get { return effortTime; }
        }        

        public int EffortMin
        {
            set { SetValue<int>(ref effortMin, value); }
            get { return effortMin; }
        }

        //Variable para almacenar parametros generales del sistema
        private GeneralParameter generalParameter;

        #endregion

        #region Constructor
        public ActivityViewModel()
        {
            instance = this;
            apiService = new ApiService();
            dialogService = new DialogService();
            navigationService = new NavigationService();
            generalParameter = new GeneralParameter();
            startDate = DateTime.Today;
            IsEnabled = true;
            MinutesList = new List<KeyValuePair<int, string>>();
            ActivityTypesList = new List<ActivityType>();
            MinutesList.Clear();
            MinutesList.Add(new KeyValuePair<int, string>(1, "00"));
            MinutesList.Add(new KeyValuePair<int, string>(2, "15"));
            MinutesList.Add(new KeyValuePair<int, string>(3, "30"));
            MinutesList.Add(new KeyValuePair<int, string>(4, "45"));
        }
        #endregion

        #region Singleton
        private static ActivityViewModel instance;
        public static ActivityViewModel GetInstance()
        {
            if (instance == null)
            {
                return new ActivityViewModel();
            }
            return instance;
        }
        #endregion

        #region Commands

        public ICommand SaveActivityCommand
        {
            get
            {
                return new RelayCommand(SaveActivity);
            }
        }

        private void SaveActivity()
        {
            IsEnabled = false;
            IsRefreshing = true;
            if (this.ActivityId == 0)
            {
                ActivityInsert();
            }
            else
            {
                ActivityUpdate();
            }
        }

        public ICommand RevertActivityCommand
        {
            get
            {
                return new RelayCommand(RevertActivity);
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

        private async void RevertActivity()
        {
            IsEnabled = false;
            IsRefreshing = true;
            try
            {
                var mainModel = MainViewModel.GetInstance();
                var responseGestion = await apiService.GetOne<bool>(mainModel.urlBaseCostos, "api/", string.Format("ConsultasAPI/GetVerificarRegistrosCap/{0}/IdActividad", this.ActivityId));
                if (responseGestion.IsSuccess)
                {
                    var gestionExists = (bool)responseGestion.Result;

                    //Se llama al servicio que inserta una labor en la base de datos y se obtiene la repuesta
                    var response = await apiService.Delete(mainModel.urlBase, "api/", string.Format("ActividadAPI/{0}/{1}/{2}/{3}", this.ActivityId, gestionExists, mainModel.CurrentUser.UserCode, ResourceCodes.DciEliminarActividad));
                    if (response.IsSuccess)
                    {
                        IsEnabled = true;
                        IsRefreshing = false;
                        dialogService.ShortToast("Transacción procesada con éxito.");
                        Back();
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("reversada"))
                {
                    await dialogService.ShowMessage("", "Esta actividad ya fue reversada anteriormente.");
                }
                else
                {
                    await dialogService.ShowMessage("Error", "Ocurrió un error al guardar la información.");
                }
            }
            IsEnabled = true;
            IsRefreshing = false;
        }

        //Metodo para insertar una nueva actividad
        private async void ActivityInsert()
        {
            var oActivity = new Activity();
            var mainModel = MainViewModel.GetInstance();
            try
            {
                oActivity = await GetCurrentActivity();
                oActivity.Action = ResourceCodes.DciAgregarActividad;

                DateTime fecha = DateTime.Now;
                fecha = fecha.AddDays(oActivity.GeneralParameterDays * -1);

                if (ValidateValues())
                {
                    if (oActivity.StartDate < fecha)
                    {
                        await dialogService.ShowMessage("", "La Fecha Inicio no puede ser menor de " + oActivity.GeneralParameterDays + " días al día de hoy.");
                        return;
                    }

                    //Se llama al servicio que inserta una labor en la base de datos y se obtiene la repuesta
                    var response = await apiService.Post(mainModel.urlBase, "api/", "ActividadAPI/", oActivity);
                    IsEnabled = true;
                    IsRefreshing = false;
                    if (response.IsSuccess)
                    {
                        dialogService.ShortToast("Transacción procesada con éxito.");
                        Back();
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("incapacidad"))
                {
                    await dialogService.ShowMessage("", "El rango de fechas ya está considerado para una Incapacidad.");
                }
                else if (ex.Message.Contains("permiso") && !ex.Message.Contains("asociado"))
                {
                    await dialogService.ShowMessage("", "El rango de fechas ya está considerado para un permiso.");
                }
                else if (ex.Message.Contains("actividad") && !ex.Message.Contains("asociado"))
                {
                    await dialogService.ShowMessage("", "El rango de fechas ya está considerado para una Actividad.");
                }
                else if (ex.Message.Contains("asociado") && !ex.Message.Contains("proyecto"))
                {
                    await dialogService.ShowMessage("", "No puede registrar la actividad, el permiso ya se encuentra asociado.");
                }
                else if (ex.Message.Contains("asociado") && ex.Message.Contains("proyecto"))
                {
                    await dialogService.ShowMessage("", "No se puede registrar la actividad, el permiso ya está asociado al proyecto.");
                }
                else if (ex.Message.Contains("rol"))
                {
                    await dialogService.ShowMessage("", "Las fechas inicio y fin de la actividad deben estar dentro del rol del Proyecto.");
                }
                else
                {
                    await dialogService.ShowMessage("Error", "Ocurrió un error al guardar la información.");
                }
                
            }
            IsEnabled = true;
            IsRefreshing = false;
        }

        //Metodo para actualizar una nueva actividad
        private async void ActivityUpdate()
        {
            var oActivity = new Activity();
            var mainModel = MainViewModel.GetInstance();
            try
            {
                oActivity = await GetCurrentActivity();
                oActivity.Action = ResourceCodes.DciModificarActividad;

                DateTime fecha = DateTime.Now;
                fecha = fecha.AddDays(oActivity.GeneralParameterDays * -1);

                if (ValidateValues())
                {
                    if (oActivity.StartDate < fecha)
                    {
                        await dialogService.ShowMessage("", "La Fecha Inicio no puede ser menor de " + oActivity.GeneralParameterDays + " días al día de hoy.");
                        return;
                    }

                    //Se llama al servicio que inserta una labor en la base de datos y se obtiene la repuesta
                    var response = await apiService.Put(mainModel.urlBase, "api/", "ActividadAPI/", oActivity);
                    if (response.IsSuccess)
                    {
                        IsEnabled = true;
                        IsRefreshing = false;
                        dialogService.ShortToast("Transacción procesada con éxito.");
                        Back();
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("procesada"))
                {
                    await dialogService.ShowMessage("", "La actividad ya fue procesada, no la puede actualizar.");
                }
                else if (ex.Message.Contains("incapacidad"))
                {
                    await dialogService.ShowMessage("", "El rango de fechas ya está considerado para una Incapacidad.");
                }
                else if (ex.Message.Contains("permiso") && !ex.Message.Contains("asociado"))
                {
                    await dialogService.ShowMessage("", "El rango de fechas ya está considerado para un permiso.");
                }
                else if (ex.Message.Contains("actividad") && !ex.Message.Contains("asociado"))
                {
                    await dialogService.ShowMessage("", "El rango de fechas ya está considerado para una Actividad.");
                }
                else if (ex.Message.Contains("asociado") && !ex.Message.Contains("proyecto"))
                {
                    await dialogService.ShowMessage("", "No puede registrar la actividad, el permiso ya se encuentra asociado.");
                }
                else if (ex.Message.Contains("asociado") && ex.Message.Contains("proyecto"))
                {
                    await dialogService.ShowMessage("", "No se puede registrar la actividad, el permiso ya está asociado al proyecto.");
                }
                else if (ex.Message.Contains("rol"))
                {
                    await dialogService.ShowMessage("", "Las fechas inicio y fin de la actividad deben estar dentro del rol del Proyecto.");
                }
                else
                {
                    await dialogService.ShowMessage("Error", "Ocurrió un error al guardar la información.");
                }

            }
            IsEnabled = true;
            IsRefreshing = false;
        }

        public async Task<Activity> GetCurrentActivity()
        {
            this.Detail=this.Detail?.Trim();
            this.EffortTime = ValidateInteger(this.EffortTime);
            var mainModel = MainViewModel.GetInstance();
            var oActivity = new Activity();
            oActivity.ActivityId = this.ActivityId;
            oActivity.ActivityClasification = new ActivityClasification { ActivityClasificationId = this.Clasification==0?Constant.Personal:Constant.Administrativa };
            oActivity.ActivityType = new ActivityType { ActivityTypeId = this.ActivityTypeId };
            
            DateTime dateStart = this.StartDate;
            string dateStartActivity = dateStart.Year + "-" + dateStart.Month + "-" + dateStart.Day + " " + dateStart.Hour + ":" + dateStart.Minute + ":" + dateStart.Second;
            oActivity.StartDate = Convert.ToDateTime(dateStartActivity);
            DateTime dateEnd = this.EndDate;
            string dateEndActivity = dateEnd.Year + "-" + dateEnd.Month + "-" + dateEnd.Day + " " + dateEnd.Hour + ":" + dateEnd.Minute + ":" + dateEnd.Second;
            oActivity.EndDate = Convert.ToDateTime(dateEndActivity);

            oActivity.Resource = mainModel.CurrentUser;
            oActivity.EffortTime = obtenerHoras(this.EffortMin, (int)this.EffortTime);
            oActivity.Detail = this.Detail;
            oActivity.LastUpdateUser = mainModel.CurrentUser.UserCode;
            oActivity.LastUpDate = DateTime.Now;
            oActivity.Project = new Project { ProjectId=(int)this.ProjectId};

            generalParameter = await LoadGeneralParameter();
            oActivity.GeneralParameterDays = obtenerDiasHabilesRegistro();
            return oActivity;
        }

        private int ValidateInteger(double value)
        {
            int result = 0;
            var val = value.ToString();
            val = val.Replace(".", "").Replace(",", "").Replace("-", "");
            int.TryParse(val, out result);
            return result;
        }

        //Calcula las horas de una labor
        private double obtenerHoras(int combo, int valor)
        {
            var cultura = CultureInfo.GetCultureInfo("es-MX");
            double cantidadHoras = 0;
            switch (combo)
            {
                case 1:
                    cantidadHoras = double.Parse(valor.ToString());
                    break;
                case 2:
                    cantidadHoras = double.Parse(valor + ".25", cultura);
                    break;
                case 3:
                    cantidadHoras = double.Parse(valor + ".5", cultura);
                    break;
                case 4:
                    cantidadHoras = double.Parse(valor + ".75", cultura);
                    break;
            }

            return cantidadHoras;
        }

        private async Task<GeneralParameter> LoadGeneralParameter()
        {
            var parameter = new GeneralParameter();
            try
            {
                var modelMain = MainViewModel.GetInstance();
                var response = await apiService.Get<GeneralParameter>(modelMain.urlBase, "/api", string.Format("/ParametroGeneralAPI/{0}", Constant.LimiteDias));
                if (response.IsSuccess)
                {
                    var result = (List<GeneralParameter>)response.Result;
                    if (result != null)
                    {
                        parameter = result.ToList().FirstOrDefault();
                    }
                    return parameter;
                }
            }
            catch (Exception ex)
            {
                dialogService.ShortToast("Error al obtener los datos.");
                await navigationService.Clear();
            }
            return null;
        }

        //Obtiene los días hábiles que tiene un usuaio para poder registrar una labor.
        private int obtenerDiasHabilesRegistro()
        {
            int cantidadDiasParametro;

            if (generalParameter.Valor.Contains("."))
            {
                var cantidadDías = generalParameter.Valor.Split('.');
                cantidadDiasParametro = int.Parse(cantidadDías[0]);
            }
            else if (generalParameter.Valor.Contains(","))
            {
                var cantidadDías = generalParameter.Valor.Split(',');
                cantidadDiasParametro = int.Parse(cantidadDías[0]);
            }
            else
            {
                cantidadDiasParametro = int.Parse(generalParameter.Valor);
            }

            return cantidadDiasParametro;
        }

        //Metodo para obtener los tipos de actividad del sistema
        public async Task<bool> GetActivityTypes(int IdClasification)
        {
            var _activityTypesList = new List<ActivityType>();
            ActivityTypesList.Clear();
            try
            {
                var typeDefault = new ActivityType
                {
                    ActivityTypeId = 0,
                    ActivityTypeName = "Seleccione"
                };
                var modelMain = MainViewModel.GetInstance();
                var result = await apiService.Get<ActivityType>(modelMain.urlBase, "api/", "TipoActividadAPI/" + IdClasification);
                if (result.IsSuccess)
                {
                    _activityTypesList = (List<ActivityType>)result.Result;
                    _activityTypesList.Insert(0, typeDefault);
                    ActivityTypesList = _activityTypesList;
                    return true;
                }
            }
            catch (Exception ex) { }
            return false;
        }

        private bool ValidateValues()
        {
            if (this.ActivityTypeId <= 0)
            {
                dialogService.ShortToast("Debe seleccionar el tipo de actividad.");
                return false;
            }else if (this.StartDate == null)
            {
                dialogService.ShortToast("La fecha de inicio de la actividad es requerida.");
                return false;
            }else if (this.EndDate == null)
            {
                dialogService.ShortToast("La fecha fin de la actividad es requerida.");
                return false;
            }
            else if (this.StartDate > this.EndDate)
            {
                dialogService.ShortToast("La fecha inicio no puede ser mayor a la fecha fin.");
                return false;
            }
            else if (string.IsNullOrEmpty(this.Detail))
            {
                dialogService.ShortToast("El detalle de la actividad es requerido.");
                return false;
            }
            else if (this.Detail.Contains("#"))
            {
                dialogService.ShortToast("El caracter # no está permitido.");
                return false;
            }
            else if ((this.EffortTime == null || this.effortTime == 0) && this.EffortMin == 1)
            {
                dialogService.ShortToast("El esfuerzo debe ser mayor a 0.");
                return false;
            }

            return true;
        }

        public async Task<bool> LoadActivityProjects()
        {
            try
            {
                ProjectsActivityList?.Clear();
                var modelMain = MainViewModel.GetInstance();
                var response = await apiService.Get<Project>(modelMain.urlBase, "/api", string.Format("/ProyectoAPI/GetProyectosRecursoTracking/{0}/2", modelMain.CurrentUser.UserId));
                if (response.IsSuccess)
                {
                    var listProjects = (List<Project>)response.Result;
                    ProjectsActivityList = listProjects;
                    ProjectsActivityList.Insert(0, new Project
                    {
                        ProjectId = 0,
                        ProjectName = "Seleccione"
                    });
                    return true;
                }
            }
            catch (Exception ex)
            {
                dialogService.ShortToast("Error al obtener los datos.");
                await navigationService.Clear();
            }
            return false;
        }

        #endregion
    }
}
