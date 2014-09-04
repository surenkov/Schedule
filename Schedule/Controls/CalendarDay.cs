using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Schedule.Controls
{
    [TemplateVisualState(GroupName = "CheckStates", Name = "Unchecked")]
    [TemplateVisualState(GroupName = "CheckStates", Name = "Checked")]
    public class CalendarDay : HeaderedItemsControl
    {
        public static readonly DependencyProperty IsCheckedProperty =
            DependencyProperty.Register("IsChecked", typeof(Boolean), typeof(CalendarDay), new UIPropertyMetadata(IsCheckedPropertyChangedCallback));
        
        static CalendarDay()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CalendarDay),
                new FrameworkPropertyMetadata(typeof(CalendarDay)));
        }

        public Boolean IsChecked
        {
            get { return (Boolean)GetValue(IsCheckedProperty); }
            set { SetValue(IsCheckedProperty, value); }
        }

        static void IsCheckedPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var day = d as CalendarDay;
            if (day != null) day.OnIsCheckedPropertyChanged(d, e);
        }

        private void OnIsCheckedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            VisualStateManager.GoToState(this, Convert.ToBoolean(e.NewValue) ? "Checked" : "Unchecked", true);
        }

        private void ChangeCheckState(object sender, RoutedEventArgs args)
        {
            IsChecked = !IsChecked;
        }
    }
}