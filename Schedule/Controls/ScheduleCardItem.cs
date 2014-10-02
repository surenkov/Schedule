using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Schedule.Models;
using Schedule.Models.DataLayer;
using Schedule.Utils;
using Schedule.Windows;

namespace Schedule.Controls
{
    [TemplatePart(Name = "PART_EditButton", Type = typeof(Button))]
    [TemplatePart(Name = "PART_DeleteButton", Type = typeof(Button))]
    public class ScheduleCardItem : ContentControl
    {
        private Button _editButton;
        private Button _deleteButton;
        public static readonly DependencyProperty ItemProperty;
        public static readonly DependencyProperty ScheduleViewProperty;

        static ScheduleCardItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ScheduleCardItem),
                new FrameworkPropertyMetadata(typeof(ScheduleCardItem)));

            ItemProperty = DependencyProperty.Register("Item", typeof(Models.Schedule), typeof(ScheduleCardItem));
            ScheduleViewProperty = DependencyProperty.Register("ScheduleView", typeof(IScheduleView), typeof(ScheduleCardItem));
        }

        public Models.Schedule Item
        {
            get { return (Models.Schedule) GetValue(ItemProperty); }
            set { SetValue(ItemProperty, value); }
        }

        public IScheduleView ScheduleView
        {
            get { return (IScheduleView) GetValue(ScheduleViewProperty); }
            set { SetValue(ScheduleViewProperty, value); }
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

                try
                {
                    var properties = Item.GetType().GetProperties().Where(p => p.PropertyType.IsSubclassOf(typeof(Entity)));
                    foreach (var rel in properties.Select(p => p.GetValue(Item) as Entity))
                        ctx.Entry(rel).State = EntityState.Unchanged;

                    ctx.Entry(Item).State = EntityState.Deleted;
                    ctx.SaveChanges();
                }
                catch (DbUpdateException)
                {
                    MessageBox.Show("Can't delete this record. Maybe it's already deleted?");
                }
            }
            ScheduleView.UpdateView();
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

                    var properties = item.GetType().GetProperties().Where(p => p.PropertyType.IsSubclassOf(typeof(Entity)));
                    foreach (var rel in properties.Select(p => p.GetValue(item) as Entity))
                        ctx.Set(rel.GetType()).Attach(rel);
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
                    ScheduleView.UpdateView();
                }
            };
            dlg.Show();
        }
    }
}
