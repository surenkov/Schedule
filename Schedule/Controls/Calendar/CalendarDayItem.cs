using System.Collections;
using System.Windows;
using System.Windows.Controls;

namespace Schedule.Controls.Calendar
{
    public class CalendarDayItem : Expander
    {
        public static readonly DependencyProperty ItemsSourceProperty;
        public static readonly DependencyProperty ItemsTemplateProperty;
        
        static CalendarDayItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CalendarDayItem),
                new FrameworkPropertyMetadata(typeof(CalendarDayItem)));

            ItemsSourceProperty = DependencyProperty.Register("ItemsSource",
                typeof(IEnumerable), typeof(CalendarDayItem));
            ItemsTemplateProperty = DependencyProperty.Register("ItemsTemplate",
                typeof (DataTemplate), typeof (CalendarDayItem));
        }

        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public DataTemplate ItemsTemplate
        {
            get { return (DataTemplate) GetValue(ItemsTemplateProperty); }
            set { SetValue(ItemsTemplateProperty, value); }
        }

    }
}