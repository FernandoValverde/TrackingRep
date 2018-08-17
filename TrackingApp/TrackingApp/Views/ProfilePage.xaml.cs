using System;
using TrackingApp.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TrackingApp.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ProfilePage : ContentPage
	{
		public ProfilePage()
		{
            InitializeComponent();

            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    LoadActivity.Scale = 1.7;
                    LoadActivity.Margin = new Thickness(0, 0, 35, 10); ;
                    break;
                default:
                    LoadActivity.Scale = 0.8;
                    break;
            }

            var instance = ProfileViewModel.GetInstance();
            Appearing += (object sender, EventArgs e) =>
            {
                instance.RefreshCommand.Execute(this);
                
            };
        }
    }
}