using DLToolkit.Forms.Controls;
using TrackingApp.Interfaces;
using TrackingApp.Models;
using TrackingApp.Services;
using TrackingApp.ViewModels;
using TrackingApp.Views;
using Xamarin.Forms;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;

namespace TrackingApp
{
    public partial class App : Application
    {

        #region Attributes
        private DataService dataService;
        #endregion

        #region Properties
        public static INavigation Navigator { get; internal set; }

        public static ProjectsPage Master { get; internal set; }

        #endregion
        
        #region Constructors
        public App()
        {
            InitializeComponent();

            //string v = DependencyService.Get<IAppVersion>().GetVersion();
            //int b = DependencyService.Get<IAppVersion>().GetBuild();

            FlowListView.Init();
            dataService = new DataService();

            var mainViewModel = MainViewModel.GetInstance();
            //LoadResources
            mainViewModel.urlBase = Application.Current.Resources["URLBase"].ToString();
            mainViewModel.urlBaseCostos = Application.Current.Resources["URLBaseCostos"].ToString();
            mainViewModel.systemCode = int.Parse(Application.Current.Resources["SystemCode"].ToString());

            var user = dataService.GetUser<User>(false);
            var theme = dataService.GetTheme<Theme>(false);
            if (theme != null)
            {
                App.Current.Resources["mainBarColor"] = theme.MainBarColor;
                App.Current.Resources["secondaryBarColor"] = theme.SecondaryBarColor;
                App.Current.Resources["backgroundColor"] = theme.BackgroundColor;
                App.Current.Resources["barTextColor"] = theme.BarTextColor;
            }
            else
            {
                App.Current.Resources["mainBarColor"] = "#001e33";
                App.Current.Resources["secondaryBarColor"] = "#002d4d";
                App.Current.Resources["backgroundColor"] = "#ffffff";
                App.Current.Resources["barTextColor"] = "#ffffff";
                theme = new Theme
                {
                    ThemeId = 1,
                    MainBarColor = "#001e33",
                    SecondaryBarColor = "#002d4d",
                    BackgroundColor = "#ffffff",
                    BarTextColor = "#ffffff"
                };
            }
            mainViewModel.CurrentTheme = theme;
            dataService.DeleteAllThemesAndInsert(theme);
            if (user != null)
            {
                user.Password = null;
                mainViewModel.SetCurrentUser(user);
                mainViewModel.Projects = new ProjectsViewModel();
                mainViewModel.Projects.GetListProjects();
                mainViewModel.Projects.ReloadProjects();
                mainViewModel.Activities = new ActivitiesViewModel();
                mainViewModel.ActivityEdit = new ActivityViewModel();
                mainViewModel.Dashboard = new DashboardViewModel();
                mainViewModel.Dashboard.GetQueryTypeList();
                mainViewModel.TrackingRegister = user.TrackingRegister;
                var projectsTabbed = new ProjectsTabbedPage();
                if (mainViewModel.TrackingRegister)
                {
                    projectsTabbed.Children.Insert(0, new ProjectsPage());
                    projectsTabbed.CurrentPage = projectsTabbed.Children[0];
                }

                MainPage = new NavigationPage(projectsTabbed);
                MainPage.Style = (Style)App.Current.Resources["navigationStyle"];
                mainViewModel.Projects.LoadProjects();

                VerifyUser(mainViewModel.CurrentUser);
            }
            else
            {                
                MainPage = new LoginPage();
            }
        }

        private async void VerifyUser(User user)
        {
            var mainViewModel = MainViewModel.GetInstance();
            var userLogin = await mainViewModel.Login.GetUserInfo(user);
            if (userLogin == null)
            {
                mainViewModel.Logout();
            }
        }

        #endregion

        #region Methods
        protected override void OnStart()
        {
            // Handle when your app starts
            AppCenter.Start("ios=b02d8609-9dc0-424c-a920-900b59176f41;" +
                  "uwp={Your UWP App secret here};" +
                  "android={Your Android App secret here}",
                            typeof(Analytics), typeof(Crashes));
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
        #endregion
    }
}
