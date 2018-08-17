using Xamarin.Forms;

namespace TrackingApp.Controls
{
    public class GradientColorStack : StackLayout
    {

        public static readonly BindableProperty StartColorProperty =
        BindableProperty.Create(nameof(StartColor),
            typeof(Color), typeof(CustomEditor), Color.Gray);
        // Gets or sets StartColor value  
        public Color StartColor
        {
            get => (Color)GetValue(StartColorProperty);
            set => SetValue(StartColorProperty, value);
        }

        public static readonly BindableProperty EndColorProperty =
        BindableProperty.Create(nameof(EndColor),
            typeof(Color), typeof(CustomEditor), Color.Gray);
        // Gets or sets StartColor value  
        public Color EndColor
        {
            get => (Color)GetValue(EndColorProperty);
            set => SetValue(EndColorProperty, value);
        }
        
    }
}
