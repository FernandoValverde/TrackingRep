using TrackingApp.Models;
using TrackingApp.Views;
using TrackingApp.ViewModels;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Linq;

namespace TrackingApp.Services
{
    public class NavigationService
    {
        #region Methods

        public async System.Threading.Tasks.Task Navigate(string pageName)
        {
            var mainViewModel = MainViewModel.GetInstance();

            switch (pageName)
            {
                case "TasksPage":
                    mainViewModel.Tasks = new TasksViewModel();
                    mainViewModel.Tasks.ProjectsList = mainViewModel.Projects.Projects;
                    if (VerifyNavigation("TasksTabbedPage"))
                        await App.Navigator.PushAsync(new TasksTabbedPage());
                    break;
                case "LaborsPage":
                    mainViewModel.Labors = new LaborsViewModel();
                    mainViewModel.LaborEdit = new LaborViewModel();
                    if (VerifyNavigation("LaborTabbedPage"))
                        await App.Navigator.PushAsync(new LaborTabbedPage());
                    break;
                case "LaborPage":
                    if (VerifyModal("LaborPage"))
                        await App.Navigator.PushModalAsync(new LaborPage());
                    break;
                case "ActivityPage":
                    if (VerifyModal("ActivityPage"))
                    {
                        await App.Navigator.PushModalAsync(new ActivityPage());
                    }
                    break;
                case "ChartPage":
                    if (VerifyModal("ChartPage"))
                    {
                        await App.Navigator.PushModalAsync(new ChartPage());
                    }
                    break;
                case "SettingsPage":
                    mainViewModel.Settings = new SettingsViewModel();
                    if (VerifyNavigation("SettingsPage"))
                        await App.Navigator.PushAsync(new SettingsPage());
                    break;
                case "ProfilePage":
                    mainViewModel.Profile = new ProfileViewModel();
                    if (VerifyNavigation("ProfilePage"))
                        await App.Navigator.PushAsync(new ProfilePage());
                    break;
                default:
                    break;
            }

        }

        public bool VerifyNavigation(string page)
        {
            int countNavigator = App.Navigator.NavigationStack.Count - 1;
            if (countNavigator >= 0)
            {
                var currentPage = App.Navigator.NavigationStack[countNavigator];
                return !currentPage.ToString().Contains(page);
            }
            return true;
        }

        public bool VerifyModal(string modal)
        {
            int countNavigator = App.Navigator.ModalStack.Count - 1;
            if (countNavigator >= 0)
            {
                var currentPage = App.Navigator.ModalStack[countNavigator];
                return !currentPage.ToString().Contains(modal);
            }
            return true;
        }

        public void LogoutNav()
        {
            var mainViewModel = MainViewModel.GetInstance();
            App.Current.MainPage = new LoginPage();

        }

        public bool SetMainPage(string pageName)
        {
            var mainViewModel = MainViewModel.GetInstance();
            switch (pageName)
            {
                case "ProjectsTabbedPage":
                    mainViewModel.Projects = ProjectsViewModel.GetInstance();
                    mainViewModel.Activities = new ActivitiesViewModel();
                    mainViewModel.ActivityEdit = new ActivityViewModel();
                    mainViewModel.Dashboard = new DashboardViewModel();
                    mainViewModel.Dashboard.GetQueryTypeList();
                    var projectsTabbed = new ProjectsTabbedPage();
                    if (mainViewModel.CurrentUser.TrackingRegister)
                    {
                        projectsTabbed.Children.Insert(0, new ProjectsPage());
                        projectsTabbed.CurrentPage = projectsTabbed.Children[0];
                    }
                    App.Current.MainPage = new NavigationPage(projectsTabbed);
                    break;
                default:
                    break;
            }
            App.Current.MainPage.Style = (Style)App.Current.Resources["navigationStyle"];
            return true;
        }

        public async System.Threading.Tasks.Task Back()
        {
            try
            {
                await App.Navigator.PopAsync();
            }
            catch { }
        }

        public async System.Threading.Tasks.Task CloseModal()
        {
            try
            {
                await App.Navigator.PopModalAsync();
            }
            catch { }
        }

        public async System.Threading.Tasks.Task Clear()
        {
            try
            {
                await App.Navigator.PopToRootAsync();
            }
            catch { }
        }
        #endregion
    }

}
