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
    public class TaskItemViewModel : Models.Task
    {
        #region Services
        private ApiService apiService;
        private NavigationService navigationService;
        private DialogService dialogService;
        #endregion

        #region Constructor
        public TaskItemViewModel()
        {
            apiService = new ApiService();
            navigationService = new NavigationService();
            dialogService = new DialogService();
        } 
        #endregion

        #region Commands

        public ICommand SelectTaskCommand
        {
            get
            {
                return new RelayCommand(SelectTask);
            }
        }

        private async void SelectTask()
        {
            if (this.StateId != Constant.Abrir)
            {
                var mainViewModel = MainViewModel.GetInstance();
                mainViewModel.TaskSelected = this.TaskId;
                var task = await GetCurrentTaskDetail();
                if (task != null)
                {
                    mainViewModel.currentTask = task;
                    if (this.StateId != Constant.Iniciar)
                    {
                        await dialogService.ShowMessage("", "Esta tarea no se encuentra en ejecución, solo puede consultar información.");
                    }
                    await navigationService.Navigate("LaborsPage");
                }
            }
            else
            {
                await dialogService.ShowMessage("", "No puede editar una tarea creada");
            }
        }

        public ICommand ChangueStateCommand
        {
            get
            {
                return new RelayCommand(ChangueState);
            }
        }

        private async void ChangueState()
        {
            var task = await GetCurrentTaskDetail();
            if (task != null)
            {
                if (task.Project.State.StateId == CodigosEstadosProyectos.EnEjecucion || task.Project.State.StateId == CodigosEstadosProyectos.EnGarantia)
                {
                    await ChangueTaskState(task);
                }
                else
                {
                    await dialogService.ShowMessage("", "Solo puede modificar las tareas para los proyectos En Ejecución y En Garantía.");
                }
            }
        }

        #endregion Commands

        #region Methods
        private async System.Threading.Tasks.Task<Models.Task> GetCurrentTaskDetail()
        {
            try
            {
                var modelMain = MainViewModel.GetInstance();
                var response = await apiService.Get<Models.Task>(modelMain.urlBase, "/api", string.Format("/TareaAPI/GetInformacionTarea/{0}/{1}/{2}",this.TaskId, modelMain.CurrentUser.UserId, Constant.EstadoTarea));
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

        private async Task<bool> ChangueTaskState(Models.Task task)
        {
            if (navigationService.VerifyNavigation("LaborTabbedPage"))
            {
                string[] strings = { "Iniciar", "Suspender", "Enviar a revisión" };
                var value = await dialogService.DisplayActionList("Tarea: " + this.TaskName, strings);
                byte newState = 0;
                switch (value)
                {
                    case "Iniciar":
                        newState = Constant.Iniciar;
                        break;
                    case "Suspender":
                        newState = Constant.Suspender;
                        if (newState == task.StateId) return false;
                        if (task.StateId == Constant.Abrir)
                        {
                            await dialogService.ShowMessage("", "Una tarea creada solo puede ser puesta en ejecución.");
                            return false;
                        }
                        if (task.StateId == Constant.Revision)
                        {
                            await dialogService.ShowMessage("", "La tarea se encuentra en revisión, no la puede suspender.");
                            return false;
                        }
                        this.ReasonMessage = await GetReason();
                        if (string.IsNullOrEmpty(this.ReasonMessage) || this.ReasonMessage == "Salir")
                        {
                            this.ReasonMessage = "";
                            return false;
                        }
                        break;
                    case "Enviar a revisión":
                        newState = Constant.Revision;
                        if (task.StateId == Constant.Abrir)
                        {
                            await dialogService.ShowMessage("", "Una tarea creada solo puede ser puesta en ejecución.");
                            return false;
                        }
                        if (task.IsRecurrent && task.StateId != Constant.Revision)
                        {
                            await dialogService.ShowMessage("", "No puede enviar a revisión una tarea recurrente.");
                            return false;
                        }
                        if (string.IsNullOrEmpty(task.Detail) && task.StateId != Constant.Abrir && task.LaborsCount == 0)
                        {
                            await dialogService.ShowMessage("", "Esta enviando una tarea a revisión sin labores. Debe ingresar un detalle.");
                            return false;
                        }
                        break;
                    default: return false;
                }
                if (newState == task.StateId) return false;

                var mainModel = MainViewModel.GetInstance();
                task.LastUpdateUser = mainModel.CurrentUser.UserCode;
                task.NewStateId = newState;
                this.NewStateId = newState;
                task.Action = ResourceCodes.DciModificarTarea;
                task.Resource = mainModel.CurrentUser;
                task.ReasonMessage = this.ReasonMessage; ;

                try
                {
                    //Se llama al servicio que actualiza una labor en la base de datos y se obtiene la repuesta
                    var response = await apiService.Put(mainModel.urlBase, "api/", "TareaAPI/PutEstadoTarea/", task);
                    if (response.IsSuccess)
                    {
                        if (response.Message == "2")
                        {
                            await dialogService.ShowMessage("", "No puede enviar la tarea a revisión, aún le quedan labores pendientes.");
                        }
                        else if (response.Message == "10")
                        {
                            await dialogService.ShowMessage("", "No puede enviar la tarea a revisión, hay labores con fecha final mayor a hoy.");
                        }
                        this.StateId = newState;
                        var TasksList = TasksViewModel.GetInstance();
                        TasksList.Refresh();
                    }
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("etapa"))
                    {
                        await dialogService.ShowMessage("", "No puede iniciar la tarea porque la etapa asociada aún no ha iniciado.");
                    }
                    else if (ex.Message.Contains("rol"))
                    {
                        await dialogService.ShowMessage("", "No puede iniciar la tarea porque su rol en el proyecto aún no ha iniciado.");
                    }
                    else
                    {
                        await dialogService.ShowMessage("Error", "Ocurrió un error al guardar la información.");
                    }
                }
            }
            return true;
        }

        private async Task<string> GetReason()
        {
            var modelTasks = TasksViewModel.GetInstance();
            return await dialogService.DisplayActionList("Detalle de suspensión", modelTasks.SuspendMessages.ToArray());
        }
        #endregion
    }
}
