using System;
using TrackingApp.Services;
using TrackingApp.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TrackingApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LaborTabbedPage : TabbedPage
    {
        #region Services
        NavigationService navigationService;
        #endregion
        public LaborTabbedPage ()
        {
            if (Device.RuntimePlatform == Device.iOS) Padding = new Thickness(0, 30, 0, 0);
            navigationService = new NavigationService();
            InitializeComponent();
        }

        
        protected override void OnAppearing()
        {
            base.OnAppearing();
            App.Navigator = this.Navigation;
        }
    }
}