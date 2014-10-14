using System;
using System.Linq;
using System.Data.Entity;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Data.Entity.Validation;
using System.Collections.Generic;
using Schedule.Utils;
using Schedule.Models;
using Schedule.Models.DataLayer;
using Schedule.Models.ViewModels.Slices;
using Schedule.Windows;

namespace Schedule.Controls.Slices
{
    [TemplatePart(Name = "PART_AddButton", Type = typeof(Button))]
    [TemplatePart(Name = "PART_ViewButton", Type = typeof(Button))]
    public class SliceCell : ItemsControl
    {
        private Button addButton;
        private Button viewButton;

        public const int VisibleItemsCount = 2;

        public static readonly DependencyProperty IsExpandedProperty =
            DependencyProperty.Register("IsExpanded", typeof(bool), typeof(SliceCell));

        public static readonly DependencyProperty ViewProperty =
            DependencyProperty.Register("View", typeof(IScheduleView), typeof(SliceCell));

        public IScheduleView View
        {
            get { return (IScheduleView)GetValue(ViewProperty); }
            set { SetValue(ViewProperty, value); }
        }

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


            addButton = GetTemplateChild("PART_AddButton") as Button;
            viewButton = GetTemplateChild("PART_ViewButton") as Button;

            if (addButton != null)
                addButton.Click += OnAddButtonClick;
            if (viewButton != null)
                viewButton.Click += OnViewButtonClick;
        }

        private void OnViewButtonClick(object sender, RoutedEventArgs args)
        {
            var entities = new HashSet<Entity>();
            var model = DataContext as SliceCellViewModel;

            if (model != null)
                entities.UnionWith(model.Items.Select(m => m.Item).Union(model.ExpanderItems.Select(m => m.Item)));

            var dlg = new EntityCardViewDialog();
            dlg.Show();
            dlg.ItemsSource = entities;
        }

        private void OnAddButtonClick(object sender, RoutedEventArgs args)
        {
            EditScheduleDialog dlg = new EditScheduleDialog(new Models.Schedule { StartDate = DateTime.Now.Date, EndDate = DateTime.Now.Date }) { ShowInTaskbar = true };
            dlg.Apply += delegate (object o, ApplyEventArgs eventArgs)
            {
                var item = eventArgs.Item;
                eventArgs.Handled = true;
                bool noExcept = true;

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
                    }
                    catch (DbEntityValidationException e)
                    {
                        e.ShowMessage();
                        noExcept = false;
                    }
                    catch (NullReferenceException)
                    {
                        MessageBox.Show("Please fill all required fields", "Error");
                        noExcept = false;
                    }
                }
                if (noExcept)
                {
                    dlg.Close();
                    View.UpdateView();
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
