using GalaSoft.MvvmLight.Command;
using System.Windows.Input;
using TrackingApp.Models;
using TrackingApp.Services;

namespace TrackingApp.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        #region Attributes
        private User currentUser;
        private int projectSelected;
        private int taskSelected;
        private int laborSelected;
        private bool trackingRegister;
        #endregion

        #region Properties

        public string urlBase;
        public string urlBaseCostos;
        public int systemCode;
        public bool TrackingRegister
        {
            get { return this.trackingRegister; }
            set { SetValue(ref this.trackingRegister, value); }
        }
        public User CurrentUser
        {
            get { return this.currentUser; }
            set { SetValue(ref this.currentUser, value); }
        }
        public Task currentTask;
        public int ProjectSelected
        {
            get { return this.projectSelected; }
            set { SetValue(ref this.projectSelected, value); }
        }
        public int TaskSelected
        {
            get { return this.taskSelected; }
            set { SetValue(ref this.taskSelected, value); }
        }
        public int LaborSelected
        {
            get { return this.laborSelected; }
            set { SetValue(ref this.laborSelected, value); }
        }
        public Theme CurrentTheme { get; set; }
        #endregion

        #region Services
        DataService dataService;
        NavigationService navigationService;
        #endregion

        #region ViewModels
        public LoginViewModel Login { get; set; }
        public ProjectsViewModel Projects { get; set; }
        public TasksViewModel Tasks { get; set; }
        public LaborsViewModel Labors { get; set; }
        public ActivitiesViewModel Activities { get; set; }
        public LaborViewModel LaborEdit { get; set; }
        public ActivityViewModel ActivityEdit { get; set; }
        public DashboardViewModel Dashboard { get; set; }
        public SettingsViewModel Settings { get; set; }
        public ProfileViewModel Profile { get; set; }
        #endregion

        #region Constructor
        public MainViewModel()
        {
            instance = this;
            Login = new LoginViewModel();
            dataService = new DataService();
            navigationService = new NavigationService();
            this.TrackingRegister = false;
        }
        #endregion

        #region Singleton
        private static MainViewModel instance;
        public static MainViewModel GetInstance()
        {
            if (instance == null)
            {
                return new MainViewModel();
            }
            return instance;
        }
        #endregion

        #region Methods
        public void SetCurrentUser(User user)
        {
            CurrentUser = user;
        }

        public void Logout()
        {
            dataService.Delete(this.currentUser);
            dataService.DeleteAllProjectsAndInsert();
            this.currentUser = null;
            this.projectSelected = 0;
            this.taskSelected = 0;
            this.laborSelected = 0;
            this.TrackingRegister = false;
            navigationService.LogoutNav();
        }
        #endregion

        public ICommand LogoutCommand { get { return new RelayCommand(Logout); } }
        public ICommand SettingsCommand { get { return new RelayCommand(GoSettings); } }
        public async void GoSettings()
        { await  navigationService.Navigate("SettingsPage"); }
        public ICommand ProfileCommand { get { return new RelayCommand(GoProfile); } }
        public async void GoProfile()
        { await navigationService.Navigate("ProfilePage"); }

    }
}
