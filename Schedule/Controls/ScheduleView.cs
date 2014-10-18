using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Schedule.Controls.Editors;

namespace Schedule.Controls
{
    abstract public class ScheduleView : HeaderedItemsControl
    {
        public static readonly DependencyProperty FiltersProperty = 
            DependencyProperty.Register("Filters", typeof(IEnumerable<Filter>), typeof(ScheduleView),
                new PropertyMetadata(default(IEnumerable<Filter>)));

        public IEnumerable<Filter> Filters
        {
            get { return (IEnumerable<Filter>)GetValue(FiltersProperty); }
            set { SetValue(FiltersProperty, value); }
        }

        public abstract void UpdateView();
    }
}
