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
    public class LaborViewModel : BaseViewModel
    {
        #region Attributes
        private ApiService apiService;
        private DialogService dialogService;
        private NavigationService navigationService;
        private bool isRefreshing;
        private bool isEnabled;
        private bool canEditLabor;

        private int laborId;
        private string description;
        private DateTime startDate;
        private DateTime startDateBak;
        private double estimationTime;
        private int estimationMin;
        private double workedTime;
        private int workedMin;
        #endregion

        #region Properties
        public List<KeyValuePair<int,string>> MinutesList { get; set; }

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

        public bool CanEditLabor
        {
            set { SetValue<bool>(ref canEditLabor, value); }
            get { return canEditLabor; }
        }

        public int LaborId
        {
            set { SetValue<int>(ref laborId, value); }
            get { return laborId; }
        }

        public string Description
        {
            set { SetValue<string>(ref description, value); }
            get { return description; }
        }

        public DateTime StartDate
        {
            set { SetValue<DateTime>(ref startDate, value); }
            get { return startDate; }
        }

        public DateTime StartDateBak
        {
            set { SetValue<DateTime>(ref startDateBak, value); }
            get { return startDateBak; }
        }

        public double EstimationTime
        {
            set {
                SetValue<double>(ref estimationTime, value); }
            get { return estimationTime; }
        }

        public int EstimationMin
        {
            set { SetValue<int>(ref estimationMin, value); }
            get { return estimationMin; }
        }

        public double WorkedTime
        {
            set {
                SetValue<double>(ref workedTime, value); }
            get { return workedTime; }
        }

        public int WorkedMin
        {
            set { SetValue<int>(ref workedMin, value); }
            get { return workedMin; }
        }

        //Variable para almacenar parametros generales del sistema
        private GeneralParameter generalParameter;

        #endregion

        #region Constructor
        public LaborViewModel()
        {
            instance = this;
            apiService = new ApiService();
            dialogService = new DialogService();
            navigationService = new NavigationService();
            generalParameter = new GeneralParameter();
            startDate = DateTime.Today;
            IsEnabled = true;
            MinutesList = new List<KeyValuePair<int,string>> ();
            MinutesList.Clear();
            MinutesList.Add(new KeyValuePair<int, string>(1,"00"));
            MinutesList.Add(new KeyValuePair<int, string>(2,"15"));
            MinutesList.Add(new KeyValuePair<int, string>(3,"30"));
            MinutesList.Add(new KeyValuePair<int, string>(4,"45"));
        }
        #endregion

        #region Singleton
        private static LaborViewModel instance;
        public static LaborViewModel GetInstance()
        {
            if (instance == null)
            {
                return new LaborViewModel();
            }
            return instance;
        }
        #endregion

        #region Commands

        public ICommand SaveLaborCommand
        {
            get
            {
                return new RelayCommand(SaveLabor);
            }
        }

        private void SaveLabor()
        {
            IsEnabled = false;
            IsRefreshing = true;
            if (this.LaborId == 0)
            {
                LaborInsert();
            }
            else
            {
                LaborUpdate();
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

        //Metodo para insertar una nueva labor
        private async void LaborInsert()
        {
            var oLabor = new Labor();
            var mainModel = MainViewModel.GetInstance();
            try
            {
                oLabor = await GetCurrentLabor();
                if (oLabor != null)
                {
                    oLabor.Action = ResourceCodes.DciAgregarLabor;
                    oLabor.State = new State() { StateId = Constant.Abrir };

                    if (oLabor.StartDate != null && oLabor.EndDate != null && ValidateValues())
                    {
                        if (oLabor.StartDate < mainModel.currentTask.Project.StartDate || oLabor.EndDate > mainModel.currentTask.Project.EndDate)
                        {
                            await dialogService.ShowMessage("", "La Fecha Inicio debe estar incluida en las fechas del Proyecto.");
                            IsEnabled = true;
                            IsRefreshing = false;
                            return;
                        }
                        else if (oLabor.StartDate < oLabor.EndDate)
                        {
                            await dialogService.ShowMessage("", "La Fecha Inicio no puede ser menor de " + oLabor.GeneralParameterDays + " días al día de hoy.");
                            IsEnabled = true;
                            IsRefreshing = false;
                            return;
                        }

                        //Se llama al servicio que inserta una labor en la base de datos y se obtiene la repuesta
                        var response = await apiService.Post(mainModel.urlBase, "api/", "LaborAPI/", oLabor);
                        if (response.IsSuccess)
                        {
                            IsEnabled = true;
                            IsRefreshing = false;
                            dialogService.ShortToast("Transacción procesada con éxito.");
                            Back();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("detalle"))
                {
                    await dialogService.ShowMessage("", "No puede duplicar registros, este detalle ya existe en la tarea.");
                }
                else
                {
                    await dialogService.ShowMessage("Error", "Ocurrió un error al guardar la información.");
                };
            }
            IsEnabled = true;
            IsRefreshing = false;
        }

        private async void LaborUpdate()
        {
            var oLabor = new Labor();
            var mainModel = MainViewModel.GetInstance();
            try
            {
                oLabor = await GetCurrentLabor();
                if (oLabor != null)
                {
                    oLabor.Action = ResourceCodes.DciModificarLabor;
                    if (oLabor.StartDate != null && oLabor.EndDate != null && ValidateValues())
                    {
                        if (oLabor.StartDate < mainModel.currentTask.Project.StartDate)
                        {
                            await dialogService.ShowMessage("", "La Fecha Inicio no puede ser menor a la Fecha de Inicio del Proyecto.");
                            IsEnabled = true;
                            IsRefreshing = false;
                            return;
                        }
                        else if (oLabor.StartDate < oLabor.EndDate && oLabor.StartDate!=this.StartDateBak)
                        {
                            await dialogService.ShowMessage("", "La Fecha Inicio no puede ser menor de " + oLabor.GeneralParameterDays + " días al día de hoy.");
                            IsEnabled = true;
                            IsRefreshing = false;
                            return;
                        }
                        
                        //Se llama al servicio que actualiza una labor en la base de datos y se obtiene la repuesta
                        var response = await apiService.Put(mainModel.urlBase, "/api", "/LaborAPI", oLabor);
                        if (response.IsSuccess)
                        {
                            IsEnabled = true;
                            IsRefreshing = false;
                            dialogService.ShortToast("Transacción procesada con éxito.");
                            Back();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("detalle"))
                {
                    await dialogService.ShowMessage("", "No puede duplicar registros, este detalle ya existe en la tarea.");
                }
                else if (ex.Message.Contains("fecha de creaci"))
                {
                    await dialogService.ShowMessage("", "No puede modificar una labor con una fecha de creación menor a "+ oLabor.GeneralParameterDays + " días de la fecha actual.");
                }
                else
                {
                    await dialogService.ShowMessage("Error", "Ocurrió un error al guardar la información.");
                }
            }
            IsEnabled = true;
            IsRefreshing = false;
        }

        private int ValidateInteger(double value)
        {
            int result = 0;
            var val = value.ToString();
            val = val.Replace(".", "").Replace(",", "").Replace("-", "");
            int.TryParse(val, out result);
            return result;
        }

        private bool ValidateValues()
        {
            if (this.StartDate == null)
            {
                dialogService.ShortToast("La fecha de inicio de la labor es requerida.");
                return false;
            }else if (string.IsNullOrEmpty(this.Description))
            {
                dialogService.ShortToast("La descripción de la labor es requerida.");
                return false;
            }
            else if (this.Description.Contains("#"))
            {
                dialogService.ShortToast("El caracter # no está permitido.");
                return false;
            }
            else if ((this.EstimationTime==null || this.EstimationTime == 0) && this.EstimationMin==1)
            {
                dialogService.ShortToast("El tiempo estimado debe ser mayor a 0.");
                return false;
            }

            return true;
        }

        public async Task<Labor> GetCurrentLabor()
        {
            generalParameter = await LoadGeneralParameter();
            if (generalParameter != null)
            {
                this.Description = this.Description?.Trim();
                this.EstimationTime = ValidateInteger(this.EstimationTime);
                this.WorkedTime = ValidateInteger(this.WorkedTime);
                var mainModel = MainViewModel.GetInstance();
                var oLabor = new Labor();
                oLabor.LaborId = this.LaborId;
                //Se captura el valor del cuadro de texto de horas estimadas
                oLabor.EstimationTime = obtenerHoras(this.EstimationMin, (int)this.EstimationTime);
                oLabor.LastUpDate = DateTime.Now;
                DateTime fechaInicio = this.StartDate;
                string fechaInicioLabor = fechaInicio.Year + "-" + fechaInicio.Month + "-" + fechaInicio.Day + " " + fechaInicio.Hour + ":" + fechaInicio.Minute + ":" + fechaInicio.Second;
                oLabor.StartDate = Convert.ToDateTime(fechaInicioLabor);
                oLabor.LastUpdateUser = mainModel.CurrentUser.UserCode;
                //Se captura el valor del cuadro de texto de horas estimadas
                oLabor.WorkedTime = obtenerHoras(this.WorkedMin, (int)this.WorkedTime);
                //Detalle de la labor
                oLabor.Description = this.Description;
                oLabor.Task = new Models.Task() { TaskId = mainModel.TaskSelected };
                oLabor.Resource = mainModel.CurrentUser;
                oLabor.GeneralParameterDays = obtenerDiasHabilesRegistro();
                DateTime fechaActual = DateTime.Now;
                oLabor.EndDate = fechaActual.AddDays(oLabor.GeneralParameterDays * -1);
                return oLabor;
            }
            return null;
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

        #endregion
    }
}
