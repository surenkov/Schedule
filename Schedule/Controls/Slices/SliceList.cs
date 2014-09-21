using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Schedule.Controls.Slices
{
    public class SliceList : HeaderedItemsControl
    {
        static SliceList()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SliceList), new FrameworkPropertyMetadata(typeof(SliceList)));
        }
    }
}
