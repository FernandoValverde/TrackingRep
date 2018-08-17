using System;
using TrackingApp.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TrackingApp.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ActivitiesPage : ContentPage
	{
		public ActivitiesPage ()
		{
            InitializeComponent();
            var instance = ActivitiesViewModel.GetInstance();
            Appearing += (object sender, EventArgs e) =>
            {
                instance.RefreshCommand.Execute(this);
            };
        }
	}
}