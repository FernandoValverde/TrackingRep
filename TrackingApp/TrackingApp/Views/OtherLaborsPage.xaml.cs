using System;
using TrackingApp.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TrackingApp.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class OtherLaborsPage : ContentPage
	{
		public OtherLaborsPage ()
		{
            InitializeComponent();
            var instance = LaborsViewModel.GetInstance();
            var mainInstance = MainViewModel.GetInstance();
            instance.SelectedTask = mainInstance.TaskSelected;
            Appearing += (object sender, EventArgs e) =>
            {
                instance.RefreshCommand.Execute(this);
            };
        }
	}
}