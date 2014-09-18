using System;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Schedule.Models;
using Schedule.Models.DataLayer;
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

        public static readonly DependencyProperty CalendarProperty;
        public static readonly DependencyProperty DateProperty;
        public static readonly DependencyProperty DayTypeProperty;


        static CalendarDay()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CalendarDay),
                new FrameworkPropertyMetadata(typeof(CalendarDay)));

            CalendarProperty = DependencyProperty.Register("Calendar", typeof(Calendar), typeof(CalendarDay));
            DateProperty = DependencyProperty.Register("Date", typeof(DateTime), typeof(CalendarDay),
                new FrameworkPropertyMetadata(DateChangedCallback));
            DayTypeProperty = DependencyProperty.Register("DayType", typeof(CalendarDayType), typeof(CalendarDay),
                new PropertyMetadata(CalendarDayType.CurrentMonth));
        }

        #region CLR Properties

        public Calendar Calendar
        {
            get { return (Calendar) GetValue(CalendarProperty); }
            set { SetValue(CalendarProperty, value); }
        }

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
            EditScheduleDialog dlg = new EditScheduleDialog(new Models.Schedule {StartDate = Date, EndDate = Date}) { ShowInTaskbar = true };
            dlg.Apply += delegate(object o, ApplyEventArgs eventArgs)
            {
                var item = eventArgs.Item;
                eventArgs.Handled = true;

                using (ScheduleDbContext ctx = new ScheduleDbContext())
                {
                    var properties = item.GetType().GetProperties().Where(p => p.PropertyType.IsSubclassOf(typeof(Entity)));
                    foreach (var rel in properties.Select(p => p.GetValue(item) as Entity))
                        ctx.Set(rel.GetType()).Attach(rel);
                    ctx.Entry(item).State = item.Id == 0 ? EntityState.Added : EntityState.Modified;
                    ctx.SaveChanges();
                }

                Calendar.UpdateView();
            };
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