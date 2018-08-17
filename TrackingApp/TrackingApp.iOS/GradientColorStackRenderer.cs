using CoreAnimation;
using CoreGraphics;
using TrackingApp.Controls;
using TrackingApp.iOS;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(GradientColorStack), typeof(GradientColorStackRenderer))]

namespace TrackingApp.iOS
{
    public class GradientColorStackRenderer : VisualElementRenderer<StackLayout>
    {
        public override void Draw(CGRect rect)
        {
            base.Draw(rect);
            GradientColorStack stack = (GradientColorStack)this.Element;
            var start= Color.FromHex("#000000").ToCGColor();
            CGColor startColor = stack.StartColor.ToCGColor();

            CGColor endColor = stack.EndColor.ToCGColor();

            #region for Vertical Gradient   
            //var gradientLayer = new CAGradientLayer();  
            #endregion

            #region for Horizontal Gradient     
            var gradientLayer = new CAGradientLayer()
            {
                StartPoint = new CGPoint(0,0),
                EndPoint = new CGPoint(0,1)
            };
            #endregion

            gradientLayer.Frame = rect;
            gradientLayer.Colors = new CGColor[] { start,startColor, endColor };

            NativeView.Layer.InsertSublayer(gradientLayer, 0);
        }
    }
}