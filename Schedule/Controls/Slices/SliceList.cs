using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using Schedule.Models.ViewModels.Slices;

namespace Schedule.Controls.Slices
{
    public class SliceList : HeaderedItemsControl
    {
        public static readonly RoutedEvent ExpandEvent =
            EventManager.RegisterRoutedEvent("Expand", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SliceList));

        public event RoutedEventHandler Expand
        {
            add { AddHandler(ExpandEvent, value); }
            remove { RemoveHandler(ExpandEvent, value); }
        }

        static SliceList()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SliceList), new FrameworkPropertyMetadata(typeof(SliceList)));
        }

        public override void OnApplyTemplate()
        {
            Expand += SliceList_Expanded;
        }

        private void SliceList_Expanded(object sender, RoutedEventArgs e)
        {
            e.Handled = true;

            var cells = ItemsSource as IEnumerable<SliceCellViewModel>;
            if (cells != null)
                foreach (var cell in cells)
                    cell.IsExpanded = !cell.IsExpanded;
        }
    }
}
