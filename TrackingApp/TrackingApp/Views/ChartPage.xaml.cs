using Microcharts;
using SkiaSharp;
using System.Collections.Generic;
using System.Linq;
using TrackingApp.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Entry = Microcharts.Entry;

namespace TrackingApp.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ChartPage : ContentPage
	{
		public ChartPage ()
		{
			InitializeComponent ();
            var dashboard = DashboardViewModel.GetInstance();
            List<Entry> list = (from d in dashboard.PersonalState
                                select new Entry((float)d.Value)
                                {
                                    Label = "",
                                    ValueLabel = d.Value+"",
                                    Color = SKColor.Parse(d.Color),
                                    TextColor = SKColors.Black
                                }).ToList();
            DataDonutChart.Chart = new DonutChart { Entries = list };
            switch (Device.RuntimePlatform)
            {
                case Device.Android:
                    FloatingActionButtonReturn.Margin = new Thickness(8, 0, 0, 0);
                    break;
            }
        }
	}
}