using System;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Windows;
using Schedule.Models;
using Schedule.Models.DataLayer;
using Schedule.Utils;
using Schedule.Windows;
using System.Collections.Generic;
using Schedule.Models.ViewModels.Calendar;
using Schedule.Properties;

namespace Schedule.Controls.Calendar
{
    public enum CalendarDayType
    {
        CurrentMonth,
        CurrentDay,
        OtherMonth
    }

    public class CalendarDay : ScheduleCell
    {
        public static readonly DependencyProperty DateProperty =
            DependencyProperty.Register("Date", typeof(DateTime), typeof(CalendarDay),
                new FrameworkPropertyMetadata(DateChangedCallback));

        public static readonly DependencyProperty DayTypeProperty =
            DependencyProperty.Register("DayType", typeof(CalendarDayType), typeof(CalendarDay),
                new PropertyMetadata(CalendarDayType.CurrentMonth));



        public Visibility HeaderVisibility
        {
            get { return (Visibility)GetValue(HeaderVisibilityProperty); }
            set { SetValue(HeaderVisibilityProperty, value); }
        }

        public static readonly DependencyProperty HeaderVisibilityProperty =
            DependencyProperty.Register("HeaderVisibility", typeof(Visibility), typeof(CalendarDay), new PropertyMetadata(Visibility.Hidden));




        static CalendarDay()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CalendarDay), new FrameworkPropertyMetadata(typeof(CalendarDay)));
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

        protected override void OnAddButtonClick(object sender, RoutedEventArgs args)
        {
            EditEntityDialog dlg = new EditEntityDialog(new Models.Schedule
            {
                StartDate = Date,
                EndDate = Date.AddMonths(1),
                Interval = Settings.Default.DefaultInterval
            })
            {
                ShowInTaskbar = true,
                Title = "Add new schedule record"
            };
            dlg.Apply += delegate (object o, ApplyEventArgs eventArgs)
            {
                var item = eventArgs.Item;
                eventArgs.Handled = true;

                using (ScheduleDbContext ctx = new ScheduleDbContext())
                {
                    try
                    {
                        var properties = item
                            .GetType()
                            .GetProperties()
                            .Where(p => p.PropertyType.IsSubclassOf(typeof(Entity)));

                        foreach (var rel in properties.Select(p => p.GetValue(item) as Entity))
                            ctx.Set(rel.GetType()).Attach(rel);
                        ctx.Entry(item).State = item.Id == 0 ? EntityState.Added : EntityState.Modified;
                        ctx.SaveChanges();

                        dlg.Close();
                        View.UpdateView();
                    }
                    catch (DbEntityValidationException e)
                    {
                        e.ShowMessage();
                    }
                    catch (NullReferenceException)
                    {
                        MessageBox.Show("Please fill all required fields", "Error");
                    }
                }
            };
            dlg.Show();
        }

        protected override void OnViewButtonClick(object sender, RoutedEventArgs args)
        {
            var entities = new HashSet<Entity>();
            var items = ItemsSource as IEnumerable<CalendarItemViewModel>;

            if (items != null)
                foreach (var item in items)
                    foreach (var card in item.Items)
                        entities.Add(card.Item);

            var dlg = new EntityGridViewDialog();
            dlg.Show();
            dlg.ItemsSource = entities;
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
}