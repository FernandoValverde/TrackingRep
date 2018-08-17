using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using UIKit;
using Xamarin.Forms;

namespace TrackingApp.iOS
{
    public class TableSource : UITableViewSource
    {
        // Global variable for the secondary toolbar items and text to display in table row
        List<ToolbarItem> _tableItems;
        string[] _tableItemTexts;
        string CellIdentifier = "TableCell";

        public TableSource(List<ToolbarItem> items)
        {
            //Set the secondary toolbar items to global variables and get all text values from the toolbar items
            _tableItems = items;
            _tableItemTexts = items.Select(a => a.Text).ToArray();
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return _tableItemTexts.Length;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            UITableViewCell cell = tableView.DequeueReusableCell(CellIdentifier);
            string item = _tableItemTexts[indexPath.Row];
            if (cell == null)
            { cell = new UITableViewCell(UITableViewCellStyle.Default, CellIdentifier); }
            cell.TextLabel.Text = item;
            return cell;
        }

        public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            return 56; // Set default row height.
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            //Used command to excute and deselct the row and removed the table.
            var command = _tableItems[indexPath.Row].Command;
            command.Execute(_tableItems[indexPath.Row].CommandParameter);
            tableView.DeselectRow(indexPath, true);
            tableView.RemoveFromSuperview();
        }
    }
}