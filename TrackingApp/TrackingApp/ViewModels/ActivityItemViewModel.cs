using GalaSoft.MvvmLight.Command;
using System;
using System.Globalization;
using System.Windows.Input;
using TrackingApp.Classes;
using TrackingApp.Models;
using TrackingApp.Services;

namespace TrackingApp.ViewModels
{
    public class ActivityItemViewModel : Activity
    {
        #region Services
        private ApiService apiService;
        private NavigationService navigationService = new NavigationService();
        private DialogService dialogService = new DialogService();
        #endregion

        #region Constructor
        public ActivityItemViewModel()
        {
            apiService = new ApiService();
            navigationService = new NavigationService();
            dialogService = new DialogService();
        }
        #endregion

        #region Commands

        public ICommand SelectActivityCommand
        {
            get
            {
                return new RelayCommand(SelectActivity);
            }
        }

        private async void SelectActivity()
        {
            var activityEdit = ActivityViewModel.GetInstance();
            activityEdit.ActivityId = this.ActivityId;
            activityEdit.Detail = this.Detail;
            activityEdit.StartDate = this.StartDate??DateTime.Today;
            activityEdit.EndDate = this.EndDate ?? DateTime.Today;
            activityEdit.EffortTime = this.EffortTime??0;
            var effort = activityEdit.EffortTime.ToString(CultureInfo.InvariantCulture).ToString().Split('.');
            activityEdit.EffortMin = GetMinutes(effort);
            activityEdit.EffortTime = int.Parse(effort[0]);            
            activityEdit.ActivityTypeId = this.ActivityType.ActivityTypeId;
            activityEdit.Clasification = this.ActivityClasification.ActivityClasificationId == Constant.Personal ? 0 : 1;
            activityEdit.IsUpdating = true;
            activityEdit.ProjectId = this.Project.ProjectId;
            await activityEdit.LoadActivityProjects();
            await navigationService.Navigate("ActivityPage");
        }       


        #endregion Commands

        #region Methods          
        
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
        
        #endregion
        
    }
}
