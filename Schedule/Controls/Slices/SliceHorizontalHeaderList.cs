using System.Windows;
using System.Windows.Controls;

namespace Schedule.Controls.Slices
{
    public class SliceHorizontalHeaderList : ItemsControl
    {
        static SliceHorizontalHeaderList()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SliceHorizontalHeaderList), new FrameworkPropertyMetadata(typeof(SliceHorizontalHeaderList)));
        }
    }
}
