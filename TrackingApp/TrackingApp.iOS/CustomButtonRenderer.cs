using TrackingApp.Controls;
using TrackingApp.iOS;
using UIKit;
using Xamarin.Forms.Platform.iOS;

[assembly: Xamarin.Forms.ExportRenderer(typeof(CustomButton), typeof(CustomButtonRenderer))]
namespace TrackingApp.iOS
{
    public class CustomButtonRenderer : ButtonRenderer
    {
        public CustomButtonRenderer() : base()
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Button> e)
        {
            base.OnElementChanged(e);
            if (Control != null)
            {
                Control.SetTitleColor(UIColor.White, UIControlState.Disabled);
                Control.SetTitleColor(UIColor.White, UIControlState.Normal);
            }
        }
    }
}