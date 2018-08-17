using Acr.UserDialogs;
using GalaSoft.MvvmLight.Command;
using System;
using System.Globalization;
using System.Windows.Input;
using TrackingApp.Classes;
using TrackingApp.Models;
using TrackingApp.Services;

namespace TrackingApp.ViewModels
{
    public class LaborItemViewModel : Labor
    {

        #region Services
        private ApiService apiService;
        private NavigationService navigationService = new NavigationService();
        private DialogService dialogService = new DialogService();
        #endregion

        #region Constructor
        public LaborItemViewModel()
        {
            apiService = new ApiService();
            navigationService = new NavigationService();
            dialogService = new DialogService();
        }
        #endregion

        #region Commands

        public ICommand SelectLaborCommand
        {
            get
            {
                return new RelayCommand(SelectLabor);
            }
        }

        private async void SelectLabor()
        {
            var mainViewModel = MainViewModel.GetInstance();
            mainViewModel.LaborSelected = this.LaborId;
            var laborEdit = LaborViewModel.GetInstance();
            var laborList = LaborsViewModel.GetInstance();
            laborEdit.Description = this.Description;
            laborEdit.StartDate = this.StartDate ?? DateTime.Today;
            laborEdit.StartDateBak = this.StartDate ?? DateTime.Today;
            laborEdit.LaborId = this.LaborId;
            laborEdit.EstimationTime = this.EstimationTime ?? 0;
            laborEdit.WorkedTime = this.WorkedTime ?? 0;
            var worked = laborEdit.WorkedTime.ToString(CultureInfo.InvariantCulture).ToString().Split('.');
            var estimation = laborEdit.EstimationTime.ToString(CultureInfo.InvariantCulture).ToString().Split('.');
            laborEdit.WorkedTime = int.Parse(worked[0]);
            laborEdit.EstimationTime = int.Parse(estimation[0]);
            laborEdit.WorkedMin = GetMinutes(worked);
            laborEdit.EstimationMin = GetMinutes(estimation);
            laborEdit.CanEditLabor = laborList.CanEditTask;
            if (this.State.StateId != Constant.Iniciar)
            {
                laborEdit.CanEditLabor = false;
                await dialogService.ShowMessage("", "Solo puede editar una labor en ejecución.");
            }

            await navigationService.Navigate("LaborPage");
        }
         
        public ICommand ChangueStateCommand
        {
            get
            {
                return new RelayCommand(ChangueState);
            }
        }

        #endregion Commands

        #region Methods

        private async void ChangueState()
        {
            var LaborList = LaborsViewModel.GetInstance();
            if (LaborList.StateId == Constant.Revision)
            {
                await dialogService.ShowMessage("", "No se pueden realizar cambios a la labor, la tarea se encuentra en revisión.");
                return;
            }
            else if (LaborList.StateId != Constant.Iniciar)
            {
                await dialogService.ShowMessage("", "No puede realizarle cambios a la labor, la tarea no se encuentra en ejecución.");
                return;
            }
            if (navigationService.VerifyModal("LaborPage") && LaborList.CanEditTask)
            {
                string[] strings = { "Iniciar", "Suspender", "Finalizar", "Cancelar", "Eliminar" };
                var value = await dialogService.DisplayActionList("Labor: " + this.Description, strings);
                byte newState = 0;
                switch (value)
                {
                    case "Iniciar":
                        newState = Constant.Iniciar;
                        break;
                    case "Suspender":
                        newState = Constant.Suspender;
                        break;
                    case "Finalizar":
                        newState = Constant.Finalizar;
                        if (newState == this.State.StateId) return;
                        var selectDate = await UserDialogs.Instance.DatePromptAsync("Fecha en que termina la labor:", DateTime.Now);
                        if (selectDate.Ok)
                        {
                            if (selectDate.SelectedDate.Date > DateTime.Today.Date)
                            {
                                await dialogService.ShowMessage("", "La fecha fin de la labor no puede ser mayor a hoy.");
                                return;
                            }
                            else if (selectDate.SelectedDate.Date < this.StartDate)
                            {
                                await dialogService.ShowMessage("", "La fecha fin de la labor no puede ser menor a la fecha de inicio.");
                                return;
                            }
                            this.EndDate = selectDate.SelectedDate;
                        }
                        else { return; }
                        break;
                    case "Cancelar":
                        newState = Constant.Cancelar;
                        break;
                    case "Eliminar":
                        var confirmar = await dialogService.ShowConfirm("Eliminar Labor", "¿Seguro que desea eliminar la labor?");
                        if (confirmar)
                        {
                            DeleteLabor();
                        }
                        return;
                    default: return;
                }
                if (newState == this.State.StateId) return;
                var labor = this.Cast<Labor>();
                var mainModel = MainViewModel.GetInstance();
                labor.LastUpdateUser = mainModel.CurrentUser.UserCode;
                labor.NewStateId = newState;
                this.NewStateId = newState;
                labor.Action = ResourceCodes.DciModificarLabor;
                labor.Resource = mainModel.CurrentUser;

                try
                {
                    //Se llama al servicio que actualiza una labor en la base de datos y se obtiene la repuesta
                    var response = await apiService.Put(mainModel.urlBase, "api/", "LaborAPI/PutEstadoLabor/", labor);
                    
                    int result = 0;
                    int.TryParse(response.Message, out result);
                    var message = GetResultStateLabor(result);
                    if (string.IsNullOrEmpty(message))
                    {
                        this.State.StateId = newState;
                        LaborList.Refresh();
                    }
                    else
                    {
                        dialogService.ShortToast(message);
                    }
                }
                catch (Exception ex)
                {
                    await dialogService.ShowMessage("Error", "Ocurrió un error al guardar la información.");
                }
            }
        }

        public int GetMinutes(string[] time)
        {
            if (time.Length > 1)
            {
                switch (time[1])
                {
                    case "25":
                        return 2;
                    case "5":
                        return 3;
                    case "75":
                        return 4;
                }
            }
            return 1;
        }

        //Metodo para verificar el resultado del cambio de estado de una labor
        private string GetResultStateLabor(int result)
        {
            string message = "";
            switch (result)
            {
                case 2:
                    message = "No puede suspender esta labor.";
                    break;
                case 9:
                    message = "No puede finalizar esta labor.";
                    break;
                case 11:
                    message = "No puede finalizar una labor que no tenga horas acumuladas.";
                    break;
                case 13:
                    message = "No puede cancelar una labor finalizada.";
                    break;
            }
            return message;
        }

        private async void DeleteLabor()
        {
            var mainModel = MainViewModel.GetInstance();
            try
            {
                var responseGestion = await apiService.GetOne<bool>(mainModel.urlBaseCostos, "api/", string.Format("ConsultasAPI/GetVerificarRegistrosCap/{0}/IdLabor", this.LaborId));
                if (responseGestion.IsSuccess)
                {
                    var gestionExists = (bool)responseGestion.Result;

                    //Se llama al servicio que actualiza una labor en la base de datos y se obtiene la repuesta
                    var response = await apiService.Delete(mainModel.urlBase, "api/", string.Format("LaborAPI/{0}/{1}/{2}/{3}/", this.LaborId,gestionExists, mainModel.CurrentUser.UserCode, ResourceCodes.DciEliminarLabor));
                    if (response.IsSuccess)
                    {
                        var LaborList = LaborsViewModel.GetInstance();
                        LaborList.Refresh();
                    }
                }
            }
            catch (Exception ex)
            {
                await dialogService.ShowMessage("Error", "Ocurrió un error al intentar eliminar la labor.");
            }
        } 
        #endregion

    }
}
