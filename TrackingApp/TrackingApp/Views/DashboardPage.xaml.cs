using System.Linq;
using TrackingApp.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TrackingApp.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class DashboardPage : ContentPage
	{
		public DashboardPage ()
		{
            InitializeComponent ();
            if (Device.RuntimePlatform == Device.iOS)
            {
                LoadActivity.Scale = 2;
                LoadActivity.Margin = new Thickness(0, 35, 0, 30);
            };
            var dashboardViewModel = DashboardViewModel.GetInstance();
            var type = dashboardViewModel.QueryTypeList.FirstOrDefault();
            QueryTypeControl.SelectedItem = type;
        }

        public async void Project_ValueChanged(SegmentedControl.FormsPlugin.Abstractions.SegmentedControl o, int e)
        {
            var dashboardViewModel = DashboardViewModel.GetInstance();
            dashboardViewModel.TaskId = 0;
            await dashboardViewModel.LoadTasks();
            var defaultLabor = dashboardViewModel.TasksList.FirstOrDefault(l => l.TaskId == dashboardViewModel.TaskId);
            LaborsListControl.SelectedItem = defaultLabor;
        }
    }
}