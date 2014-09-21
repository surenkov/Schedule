using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Schedule.Models;
using Schedule.Models.DataLayer;
using Schedule.Utils;
using Schedule.Windows;

namespace Schedule.Controls.Calendar
{
    [TemplatePart(Name = "PART_EditButton", Type = typeof(Button))]
    [TemplatePart(Name = "PART_DeleteButton", Type = typeof(Button))]
    public class CalendarCardItem : ContentControl
    {
        private Button _editButton;
        private Button _deleteButton;
        public static readonly DependencyProperty ItemProperty;
        public static readonly DependencyProperty CalendarProperty;

        static CalendarCardItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CalendarCardItem),
                new FrameworkPropertyMetadata(typeof(CalendarCardItem)));

            ItemProperty = DependencyProperty.Register("Item", typeof(Models.Schedule), typeof(CalendarCardItem));
            CalendarProperty = DependencyProperty.Register("Calendar", typeof(Calendar), typeof(CalendarCardItem));
        }

        public Models.Schedule Item
        {
            get { return (Models.Schedule)GetValue(ItemProperty); }
            set { SetValue(ItemProperty, value); }
        }

        public Calendar Calendar
        {
            get { return (Calendar)GetValue(CalendarProperty); }
            set { SetValue(CalendarProperty, value); }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _editButton = GetTemplateChild("PART_EditButton") as Button;
            _deleteButton = GetTemplateChild("PART_DeleteButton") as Button;

            if (_editButton != null)
                _editButton.Click += EditButtonOnClick;
            if (_deleteButton != null)
                _deleteButton.Click += DeleteButtonOnClick;
        }

        private void DeleteButtonOnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            using (ScheduleDbContext ctx = new ScheduleDbContext())
            {

                var properties = Item.GetType().GetProperties().Where(p => p.PropertyType.IsSubclassOf(typeof(Entity)));
                foreach (var rel in properties.Select(p => p.GetValue(Item) as Entity))
                    ctx.Entry(rel).State = EntityState.Unchanged;

                ctx.Entry(Item).State = EntityState.Deleted;
                ctx.SaveChanges();
            }
            Calendar.UpdateView();
        }

        private void EditButtonOnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            var dlg = new EditScheduleDialog(Item, true) { ShowInTaskbar = true };
            dlg.Apply += delegate(object o, ApplyEventArgs args)
            {
                bool noExcept = true;
                var item = args.Item;
                using (ScheduleDbContext ctx = new ScheduleDbContext())
                {
                    StringBuilder b = new StringBuilder();
                    ctx.Database.Log = s => b.Append(s);

                    ctx.Entry(Item).State = EntityState.Detached;
                    ctx.Entry(item).State = EntityState.Modified;
                    try
                    {
                        ctx.SaveChanges();
                        MessageBox.Show(b.ToString());
                    }
                    catch (DbEntityValidationException e)
                    {
                        e.ShowMessage();
                        noExcept = false;
                    }
                }
                if (noExcept)
                {
                    dlg.Close();
                    Calendar.UpdateView();
                }
            };
            dlg.Show();
        }
    }
}
