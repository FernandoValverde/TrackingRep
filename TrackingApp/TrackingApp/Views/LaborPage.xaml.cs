using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TrackingApp.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class LaborPage : ContentPage
	{
		public LaborPage()
		{
			InitializeComponent();
            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    FloatingActionButtonAdd.Margin = 40;
                    FloatingActionButtonReturn.Margin = 40;
                    LoadActivity.Scale = 2;
                    break;
                default:
                    FloatingActionButtonAdd.Margin = new Thickness(0, 0, 10, 0); ;
                    FloatingActionButtonReturn.Margin = new Thickness(10, 0, 0, 0); ;
                    break;
            }
        }

        void EstimationTime_TextChanged(object sender, TextChangedEventArgs e)
        {
            var newText = e.NewTextValue;
            if (!string.IsNullOrEmpty(newText))
            {
                newText = newText.Replace(".", "").Replace(",", "").Replace("-", "");
            }
            else
            {
                newText = "0";
            }
            if (newText.Length > 3)
            {
                newText = newText.Substring(0,3);
            }
            if(newText!=e.OldTextValue || newText != e.NewTextValue)
            EstimationTime.Text = newText;
        }
        void WorkedTime_TextChanged(object sender, TextChangedEventArgs e)
        {
            var newText = e.NewTextValue;
            if (!string.IsNullOrEmpty(newText))
            {
                newText = newText.Replace(".", "").Replace(",", "").Replace("-", "");
            }
            else
            {
                newText = "0";
            }
            if (newText.Length > 3)
            {
                newText = newText.Substring(0, 3);
            }
            if (newText != e.OldTextValue || newText != e.NewTextValue)
                WorkedTime.Text = newText;
        }
    }
}