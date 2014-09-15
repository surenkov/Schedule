using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Threading;
using Schedule.Annotations;
using Schedule.Models.ViewModels;
using Control = System.Windows.Controls.Control;

namespace Schedule.Controls
{
    public delegate IEnumerable<Models.Schedule> UpdateScheduleSourceTrigger(DateTime starTime, DateTime endTime);
    public class Calendar : Control
    {
        public static readonly DependencyProperty DateProperty;
        public static readonly DependencyProperty UpdateSourceProperty;

        private const int MaxDays = 42;
        private readonly DispatcherTimer _timer;

        static Calendar()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Calendar),
                new FrameworkPropertyMetadata(typeof(Calendar)));

            DateProperty = DependencyProperty.Register("Date",
                typeof(DateTime), typeof(Calendar), new FrameworkPropertyMetadata(DateTime.Now, DateChangedCallback));
            UpdateSourceProperty = DependencyProperty.Register("UpdateSource", typeof(UpdateScheduleSourceTrigger),
                typeof(Calendar), new PropertyMetadata(null));
        }

        public Calendar()
        {
            _timer = new DispatcherTimer();
            _timer.Tick += new EventHandler(delegate
            {
                _timer.Interval = DateTime.Now.Date.AddDays(1) - DateTime.Now;
                Date = DateTime.Now.Date;
            });
        }

        private static void DateChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var c = d as Calendar;
            if (c != null) c.OnDateChanged(e);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            Update();
            _timer.Start();
        }

        private void OnDateChanged(DependencyPropertyChangedEventArgs e)
        {
            var startDay = new DateTime(Date.Year, Date.Month, 1);
            while (startDay.DayOfWeek != CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek)
                startDay = startDay.AddDays(-1);
            var endDay = startDay.AddDays(MaxDays);

            Update(UpdateSource(startDay, endDay));
        }

        private void Update(IEnumerable<Models.Schedule> items = null)
        {
            var firstDayOfMonth = new DateTime(Date.Year, Date.Month, 1);
            var startDay = firstDayOfMonth;
            while (startDay.DayOfWeek != CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek)
                startDay = startDay.AddDays(-1);
            var endDay = startDay.AddDays(MaxDays);

            var dict = new Dictionary<DateTime, List<Models.Schedule>>();
            if (items != null)
                foreach (var item in items)
                {
                    DateTime itemDate = item.StartDate.Date;
            
                    while (itemDate < startDay)
                        itemDate = itemDate.AddDays(item.Interval);
                
                    var endDate = endDay < item.EndDate ? endDay : item.EndDate;
                    while (itemDate <= endDate)
                    {
                        if (!dict.ContainsKey(itemDate))
                            dict.Add(itemDate, new List<Models.Schedule>());
                
                        dict[itemDate].Add(item);
                        itemDate = itemDate.AddDays(item.Interval);
                    }
                }

            var listsOfDays = FindVisualChildren<CalendarDaysList>(GetTemplateChild("ListGrid")).ToList();
            var weeks = new List<ScheduleDay>[listsOfDays.Count];
            for (int i = 0; i < weeks.Length; i++)
                weeks[i] = new List<ScheduleDay>(MaxDays / weeks.Length);

            for (int i = 0; i < MaxDays; i++)
                weeks[i/(MaxDays/weeks.Length)].Add(new ScheduleDay
                {
                    Date = startDay.AddDays(i),
                    Items = dict.ContainsKey(startDay.AddDays(i)) ? ScheduleMapper.Map(dict[startDay.AddDays(i)]) : null
                });
            for (int i = 0; i < weeks.Length; i++)
                listsOfDays[i].ItemsSource = weeks[i];
        }

        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj == null) yield break;
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                var child = VisualTreeHelper.GetChild(depObj, i);
                if (child is T) yield return child as T;
            }
        }

        public DateTime Date
        {
            get { return (DateTime)GetValue(DateProperty); }
            set { SetValue(DateProperty, value); }
        }

        public UpdateScheduleSourceTrigger UpdateSource
        {
            get { return (UpdateScheduleSourceTrigger)GetValue(UpdateSourceProperty); }
            set { SetValue(UpdateSourceProperty, value); }
        }
    }
}