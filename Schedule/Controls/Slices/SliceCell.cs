using System.Windows;
using System.Windows.Controls;

namespace Schedule.Controls.Slices
{
    public class SliceCell : ItemsControl
    {
        static SliceCell()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SliceCell), new FrameworkPropertyMetadata(typeof(SliceCell)));
        }
    }
}
