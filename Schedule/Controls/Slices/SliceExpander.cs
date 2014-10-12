using System.Collections;
using System.Windows;
using System.Windows.Controls;

namespace Schedule.Controls.Slices
{
    class SliceExpander : Expander
    {

        public static readonly DependencyProperty ItemsSourceProperty = 
            DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(SliceExpander));

        public static readonly DependencyProperty ItemTemplateProperty = 
            DependencyProperty.Register("ItemTemplate", typeof(DataTemplate), typeof(SliceExpander));

        static SliceExpander()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SliceExpander),
                new FrameworkPropertyMetadata(typeof(SliceExpander)));
        }

        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public DataTemplate ItemTemplate
        {
            get { return (DataTemplate)GetValue(ItemTemplateProperty); }
            set { SetValue(ItemTemplateProperty, value); }
        }
    }
}
