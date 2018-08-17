using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TrackingApp.iOS;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: Xamarin.Forms.ExportRenderer(typeof(TabbedPage), typeof(RightToolbarMenuCustomRenderer))]
namespace TrackingApp.iOS
{
    public class RightToolbarMenuCustomRenderer : TabbedRenderer
    {
        //I used UITableView for showing the menulist of secondary toolbar items.
        List<ToolbarItem> _secondaryItems;
        UITableView table;

        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            //Get all secondary toolbar items and fill it to the gloabal list variable and remove from the content page.
            if (e.NewElement is TabbedPage page)
            {
                _secondaryItems = page.ToolbarItems.Where(i => i.Order == ToolbarItemOrder.Secondary).ToList();
                _secondaryItems.ForEach(t => page.ToolbarItems.Remove(t));
            }

            UITapGestureRecognizer tapGesture = new UITapGestureRecognizer(HideMenuToolbar);

            foreach (var subview in View.Subviews)
            {
                if(subview.GetType().ToString() != "UIKit.UITabBar")
                subview.AddGestureRecognizer(tapGesture);
            }

            var tabbarController = (UITabBarController)this.ViewController;
            if (null != tabbarController)
            {
                tabbarController.ViewControllerSelected += OnTabbarControllerItemSelected;
            }
            base.OnElementChanged(e);
        }
        
        public override void ViewWillAppear(bool animated)
        {
            var element = (TabbedPage)Element;
            element.ToolbarItems.Clear();
            //If global secondary toolbar items are not null, I created and added a primary toolbar item with image(Overflow) I         
            // want to show.
            if (_secondaryItems != null && _secondaryItems.Count > 0)
            {
                element.ToolbarItems.Add(new ToolbarItem()
                {
                    Order = ToolbarItemOrder.Primary,
                    Icon = "ic_more_vert.png",
                    Priority = 1,
                    Command = new Command(() =>
                    {
                        ToolClicked();
                    })
                });
            }
            
            base.ViewWillAppear(animated);
        }


        private void OnTabbarControllerItemSelected(object sender, UITabBarSelectionEventArgs eventArgs)
        {
            HideMenuToolbar();
        }

        private void HideMenuToolbar()
        {
            foreach (var subview in View.Subviews)
            {
                if (subview == table)
                {
                    table.RemoveFromSuperview();
                    break;
                }
            }
        }

        public override void ViewWillDisappear(bool animated)
        {
            foreach (var subview in View.Subviews)
            {
                if (subview == table)
                {
                    
                    table.RemoveFromSuperview();
                    return;
                }
            }
            base.ViewWillDisappear(animated);
        }

        //Create a table instance and added it to the view.
        private void ToolClicked()
        {
            if (table == null)
            {
                //Set the table position to right side. and set height to the content height.
                var childRect = new RectangleF((float)View.Bounds.Width - 250, 0, 250, _secondaryItems.Count() * 56);
                table = new UITableView(childRect)
                {
                    Source = new TableSource(_secondaryItems), // Created Table Source Class as Mentioned in the 
                                                               //Xamarin.iOS   Official site
                    ClipsToBounds = false
                };
                table.Layer.ShadowColor = UIColor.Black.CGColor;
                table.Layer.ShadowOpacity = 0.3f;
                table.Layer.ShadowRadius = 5.0f;
                table.Layer.ShadowOffset = new System.Drawing.SizeF(5f, 5f);
                table.BackgroundColor = UIColor.White;
                Add(table);
                return;
            }
            foreach (var subview in View.Subviews)
            {
                if (subview == table)
                {
                    table.RemoveFromSuperview();
                    return;
                }
            }
            Add(table);
        }
    }
}