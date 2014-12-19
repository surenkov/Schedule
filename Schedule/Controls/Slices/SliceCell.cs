using System;
using System.Linq;
using System.Data.Entity;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Data.Entity.Validation;
using System.Collections.Generic;
using Schedule.Utils;
using Schedule.Models;
using Schedule.Models.DataLayer;
using Schedule.Models.ViewModels.Slices;
using Schedule.Windows;
using Schedule.Properties;

namespace Schedule.Controls.Slices
{
    public class SliceCell : ScheduleCell
    {
        public const int VisibleItemsCount = 2;

        public static readonly DependencyProperty IsExpandedProperty =
            DependencyProperty.Register("IsExpanded", typeof(bool), typeof(SliceCell));

        public bool IsExpanded
        {
            get { return (bool)GetValue(IsExpandedProperty); }
            set { SetValue(IsExpandedProperty, value); }
        }

        static SliceCell()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SliceCell), new FrameworkPropertyMetadata(typeof(SliceCell)));
        }

        public override void OnApplyTemplate()
        {
            var btn = GetTemplateChild("PART_ExpandButton") as ToggleButton;
            if (btn != null)
                btn.Click += (s, e) => OnIsExpandedChanged();
            base.OnApplyTemplate();
        }

        protected override void OnViewButtonClick(object sender, RoutedEventArgs args)
        {
            var entities = new HashSet<Entity>();
            var model = DataContext as SliceCellViewModel;

            if (model != null)
                entities.UnionWith(model.Items.Select(m => m.Item).Union(model.ExpanderItems.Select(m => m.Item)));

            var dlg = new EntityGridViewDialog();
            dlg.ItemsChanged += (s, e) => View.UpdateView();
            dlg.Show();
            dlg.ItemsSource = entities;
        }

        protected override void OnAddButtonClick(object sender, RoutedEventArgs args)
        {
            SliceView view = (SliceView)View;
            EditEntityDialog dlg = new EditEntityDialog(
                new Models.Schedule
                {
                    StartDate = view.StartDate,
                    EndDate = view.EndDate,
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
                        var properties =
                            item.GetType().GetProperties().Where(p => p.PropertyType.IsSubclassOf(typeof(Entity)));
                        foreach (var rel in properties.Select(p => p.GetValue(item) as Entity))
                            ctx.Set(rel.GetType()).Attach(rel);
                        ctx.Entry(item).State = item.Id == 0 ? EntityState.Added : EntityState.Modified;
                        ctx.SaveChanges();

                        dlg.Close();
                        view.UpdateView();
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

        public void OnIsExpandedChanged()
        {
            RoutedEventArgs args = new RoutedEventArgs(SliceList.ExpandEvent);
            RaiseEvent(args);
            IsExpanded = !IsExpanded;
        }
    }
}
