using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TrackingApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();
            if (Device.RuntimePlatform == Device.iOS) {
                LogueImage.Margin = new Thickness(15, 80, 15, 45);
                LoadActivity.Scale = 2;
                btnLogin.Margin = new Thickness(0, 40, 0, 0);
            };
        }
    }
}