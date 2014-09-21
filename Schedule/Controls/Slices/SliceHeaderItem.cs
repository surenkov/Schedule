using System.Windows;
using System.Windows.Controls;

namespace Schedule.Controls.Slices
{
    public class SliceHeaderItem : Control
    {
        public static readonly DependencyProperty TextProperty;

        static SliceHeaderItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SliceHeaderItem),
                new FrameworkPropertyMetadata(typeof(SliceHeaderItem)));

            TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(SliceHeaderItem));
        }

        public string Text
        {
            get { return (string) GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
    }
}
