using System.Windows;
using System.Windows.Controls;

namespace Schedule.Controls.Calendar
{
    public class CalendarDaysList : ItemsControl
    {
        static CalendarDaysList()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CalendarDaysList),
                new FrameworkPropertyMetadata(typeof(CalendarDaysList)));
        }
    }
}