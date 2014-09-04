using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Schedule.Controls
{
    public class CalendarDaysList : HeaderedItemsControl
    {
        static CalendarDaysList()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CalendarDaysList),
                new FrameworkPropertyMetadata(typeof(CalendarDaysList)));
        }
    }
}