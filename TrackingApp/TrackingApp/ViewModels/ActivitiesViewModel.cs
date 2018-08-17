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
    public class ActivitiesViewModel : BaseViewModel
    {
        #region Attributes
        private ApiService apiService;
        private DialogService dialogService;
        private NavigationService navigationService;
        private bool isRefreshing;
        #endregion

        #region Properties
        public ObservableCollection<ActivityItemViewModel> ActivitiesList { get; set; }
               
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
        public ActivitiesViewModel()
        {
            instance = this;
            apiService = new ApiService();
            dialogService = new DialogService();
            navigationService = new NavigationService();
            ActivitiesList = new ObservableCollection<ActivityItemViewModel>();
        }
        #endregion

        #region Singleton
        private static ActivitiesViewModel instance;
        public static ActivitiesViewModel GetInstance()
        {
            if (instance == null)
            {
                return new ActivitiesViewModel();
            }
            return instance;
        }
        #endregion

        #region Methods
        private async Task<bool> LoadActivities()
        {
            try
            {
                DateTime fechaActual = DateTime.Now;
                string sqlFecha = fechaActual.ToString("yyyy-M-d");
                var modelMain = MainViewModel.GetInstance();
                var response = await apiService.Get<Activity>(modelMain.urlBase, "/api", string.Format("/ActividadAPI/GetActividadesRecurso/0/{0}/{1}/1/{2}/{3}", modelMain.CurrentUser.UserId, Constant.Clasificacion, sqlFecha,sqlFecha));
                modelMain.Projects.LoadProjects();
                if (response.IsSuccess)
                {
                    ReloadActivities((List<Activity>)response.Result);
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

        private void ReloadActivities(List<Activity> activities)
        {
            ActivitiesList.Clear();
            activities = activities.Select(c =>
            {
                c.Detail =
                c.Detail == null ? null :
                c.Detail.Length <= 500 ? c.Detail :
                c.Detail.Substring(0, 495) + "..."; return c;
            }).ToList();
            foreach (var activity in activities)
            {
                ActivitiesList.Add(activity.Cast<ActivityItemViewModel>());
            }
        }

        #endregion

        #region Commands
        public ICommand RefreshCommand { get { return new RelayCommand(Refresh); } }

        public async void Refresh()
        {
            IsRefreshing = true;
            await LoadActivities();
            IsRefreshing = false;
        }

        public ICommand AddActivityCommand
        {
            get
            {
                return new RelayCommand(AddActivity);
            }
        }

        private async void AddActivity()
        {
            var activityEdit = ActivityViewModel.GetInstance();
            activityEdit.Detail = "";
            activityEdit.StartDate = DateTime.Today;
            activityEdit.EndDate = DateTime.Today;
            activityEdit.ActivityId = 0;
            activityEdit.EffortTime = 0;
            activityEdit.EffortMin = 1;
            activityEdit.ActivityTypeId = 0;
            activityEdit.Clasification = 0;
            activityEdit.IsUpdating = false;
            activityEdit.ProjectId = 0;
            await activityEdit.LoadActivityProjects();
            await navigationService.Navigate("ActivityPage");
        }
        #endregion Commands
    }
}
