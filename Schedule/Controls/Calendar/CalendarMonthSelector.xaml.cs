using System;
using System.Windows;
using System.Windows.Controls;

namespace Schedule.Controls.Calendar
{
    public partial class CalendarMonthSelector : UserControl
    {
        public static readonly DependencyProperty DateProperty =
            DependencyProperty.Register("Date", typeof (DateTime), typeof (CalendarMonthSelector),
                new PropertyMetadata(OnDatePropertyChanged));

        public CalendarMonthSelector()
        {
            InitializeComponent();
        }

        private static void OnDatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CalendarMonthSelector selector = (CalendarMonthSelector) d;
            if (selector == null) return;
            DateTime time = (DateTime)e.NewValue;
            selector.MonthNameBlock.Text = time.ToString("MMMM yyyy");
        }

        public DateTime Date
        {
            get { return (DateTime)GetValue(DateProperty); }
            set { SetValue(DateProperty, value); }
        }

        private void MonthIncrement_Click(object sender, RoutedEventArgs e)
        {
            Date = Date.AddMonths(1);
        }

        private void MonthDecrement_Click(object sender, RoutedEventArgs e)
        {
            Date = Date.AddMonths(-1);
        }
    }
}
