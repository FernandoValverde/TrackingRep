using Xamarin.Forms;

namespace TrackingApp.Controls
{
    public class ImageCircle : Image
    {
        public static readonly BindableProperty BorderColorProperty =
        BindableProperty.Create(nameof(BorderColor),
            typeof(Color), typeof(CustomPicker), Color.Gray);
        // Gets or sets BorderColor value  
        public Color BorderColor
        {
            get => (Color)GetValue(BorderColorProperty);
            set => SetValue(BorderColorProperty, value);
        }
    }
}
