using System;
using System.Windows;
using System.Windows.Controls;
using Schedule.Windows;

namespace Schedule.Controls
{
    public enum CalendarDayType
    {
        CurrentMonth,
        CurrentDay,
        OtherMonth
    }

    [TemplatePart(Name = "PART_AddButton", Type = typeof(Button))]
    [TemplatePart(Name = "PART_ViewButton", Type = typeof(Button))]
    public class CalendarDay : HeaderedItemsControl
    {
        private Button _addButton;
        private Button _viewButton;

        public static readonly DependencyProperty DateProperty;
        public static readonly DependencyProperty DayTypeProperty;

        static CalendarDay()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CalendarDay),
                new FrameworkPropertyMetadata(typeof(CalendarDay)));

            DateProperty = DependencyProperty.Register("Date", typeof(DateTime), typeof(CalendarDay),
                new FrameworkPropertyMetadata(DateChangedCallback));
            DayTypeProperty = DependencyProperty.Register("DayType", typeof(CalendarDayType), typeof(CalendarDay),
                new PropertyMetadata(CalendarDayType.CurrentMonth));
        }

        #region CLR Properties
        public DateTime Date
        {
            get { return (DateTime)GetValue(DateProperty); }
            set { SetValue(DateProperty, value); }
        }

        public CalendarDayType DayType
        {
            get { return (CalendarDayType)GetValue(DayTypeProperty); }
            set { SetValue(DayTypeProperty, value); }
        }
        #endregion

        #region Custom logic
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _addButton = GetTemplateChild("PART_AddButton") as Button;
            _viewButton = GetTemplateChild("PART_ViewButton") as Button;

            if (_addButton != null)
                _addButton.Click += OnAddButtonClick;
            if (_viewButton != null)
                _viewButton.Click += OnViewButtonClick;
        }

        private void OnAddButtonClick(object sender, RoutedEventArgs args)
        {
            EditScheduleDialog dlg = new EditScheduleDialog { ShowInTaskbar = true };
            dlg.Show();
        }

        private void OnViewButtonClick(object sender, RoutedEventArgs args)
        {
            ViewScheduleDialog dlg = new ViewScheduleDialog { ShowInTaskbar = true };
            dlg.Show();
        }

        private static void DateChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var day = d as CalendarDay;
            if (day == null) return;

            var date = (DateTime)e.NewValue;

            if (date.Year != DateTime.Now.Year || date.Month != DateTime.Now.Month)
                day.DayType = CalendarDayType.OtherMonth;
            else if (date.Date == DateTime.Now.Date)
                day.DayType = CalendarDayType.CurrentDay;
        }
    }
        #endregion
}