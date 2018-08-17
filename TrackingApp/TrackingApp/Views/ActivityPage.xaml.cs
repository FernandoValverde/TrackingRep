using System.Linq;
using TrackingApp.Classes;
using TrackingApp.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TrackingApp.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ActivityPage : ContentPage
	{
		public ActivityPage ()
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

        public async void Clasification_ValueChanged(SegmentedControl.FormsPlugin.Abstractions.SegmentedControl o, int e)
        {
            var activityEdit = ActivityViewModel.GetInstance();
            var _clasification = o.SelectedSegment == 0 ? Constant.Personal : Constant.Administrativa;
            await activityEdit.GetActivityTypes(_clasification);
            var act = activityEdit.ActivityTypesList.FirstOrDefault(ac => ac.ActivityTypeId == activityEdit.ActivityTypeId);
            ActivityTypesControl.SelectedItem = act;
        }

        

        void EffortTime_TextChanged(object sender, TextChangedEventArgs e)
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
                EffortTime.Text = newText;
        }
    }
}