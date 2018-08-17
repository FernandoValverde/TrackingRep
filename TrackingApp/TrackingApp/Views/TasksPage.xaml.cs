using System;
using TrackingApp.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TrackingApp.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class TasksPage : ContentPage
	{
        public TasksPage()
        {
            InitializeComponent();
            var instance = TasksViewModel.GetInstance();
            var mainInstance = MainViewModel.GetInstance();
            instance.SelectedProject = mainInstance.ProjectSelected;
        }

        public async void Project_ValueChanged(SegmentedControl.FormsPlugin.Abstractions.SegmentedControl o, int e)
        {
            var instance = TasksViewModel.GetInstance();
            await instance.LoadTasks();
        }
    }
}