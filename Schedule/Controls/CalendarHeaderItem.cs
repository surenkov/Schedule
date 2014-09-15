using System.Windows;
using System.Windows.Controls;

namespace Schedule.Controls
{
    public class CalendarHeaderItem : HeaderedContentControl
    {
        static CalendarHeaderItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CalendarHeaderItem), new FrameworkPropertyMetadata(typeof(CalendarHeaderItem)));
        }
    }
}
