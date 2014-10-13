using System;
using System.Linq;
using System.Windows;
using System.Reflection;
using System.Data.Entity;
using System.Windows.Input;
using System.ComponentModel;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using Schedule.Utils;
using Schedule.Models;
using Schedule.Attributes;
using Schedule.Utils.Filters;
using Schedule.Models.DataLayer;
using Schedule.Controls.Editors;

namespace Schedule.Windows
{
    public delegate IEnumerable<Entity> EntityCardViewDialogUpdateEvent();

    public partial class EntityCardViewDialog : Window
    {
        private FiltersFactory _filtersFactory;

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(IEnumerable<Entity>), typeof(EntityCardViewDialog),
                new PropertyMetadata(null, ItemsSourceChanged));

        public static readonly DependencyProperty ShowHiddenPropertiesProperty =
            DependencyProperty.Register("ShowHiddenProperties", typeof(bool), typeof(EntityCardViewDialog),
                new PropertyMetadata(false, ShowHiddenPropertiesPropertyChanged));

        public static readonly DependencyProperty UpdateEventProperty =
            DependencyProperty.Register("UpdateEvent", typeof(EntityCardViewDialogUpdateEvent), typeof(EntityCardViewDialog),
                new PropertyMetadata(null, UpdateEventPropertyChanged));

        public IEnumerable<Entity> ItemsSource
        {
            get { return (IEnumerable<Entity>)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public bool ShowHiddenProperties
        {
            get { return (bool)GetValue(ShowHiddenPropertiesProperty); }
            set { SetValue(ShowHiddenPropertiesProperty, value); }
        }

        public EntityCardViewDialogUpdateEvent UpdateEvent
        {
            get { return (EntityCardViewDialogUpdateEvent)GetValue(UpdateEventProperty); }
            set { SetValue(UpdateEventProperty, value); }
        }

        private static void ShowHiddenPropertiesPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var dlg = d as EntityCardViewDialog;
            if (dlg != null)
                dlg.ItemsGrid_UpdateColumnsVisibility(dlg.ItemsGrid, null);
        }

        private static void UpdateEventPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var dlg = d as EntityCardViewDialog;
            if (dlg != null)
            {
                dlg.SettingsPanel.IsEnabled = e.NewValue != null;
                if (e.NewValue != null)
                    dlg.ItemsSource = dlg.UpdateEvent();
            }
        }

        private static void ItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var dlg = d as EntityCardViewDialog;
            if (dlg != null)
                dlg.UpdateGrid();
        }


        public EntityCardViewDialog()
        {
            InitializeComponent();
            DataContext = this;

            var dpd = DependencyPropertyDescriptor.FromProperty(ItemsControl.ItemsSourceProperty, typeof(DataGrid));
            if (dpd != null) dpd.AddValueChanged(ItemsGrid, ItemsGrid_UpdateColumnsVisibility);
        }

        private void ItemsGrid_UpdateColumnsVisibility(object sender, EventArgs eventArgs)
        {
            var grid = sender as DataGrid;
            if (grid != null && grid.ItemsSource.OfType<Entity>().Count() > 0)
            {
                var properties = grid.ItemsSource.GetType().GenericTypeArguments[0].GetProperties();
                for (int i = 0; i < properties.Length; i++)
                {

                    var descr = properties[i].GetCustomAttribute(typeof(DescriptionAttribute)) as DescriptionAttribute;
                    var shown = properties[i].GetCustomAttribute(typeof(NotShownAttribute)) as NotShownAttribute;

                    bool show = (properties[i].PropertyType == typeof(string) ||
                                properties[i].PropertyType.GetInterface("IEnumerable") == null) &&
                                (shown == null || shown.Shown) || ShowHiddenProperties;

                    grid.Columns[i].Visibility = show ? Visibility.Visible : Visibility.Hidden;
                    if (descr != null)
                        grid.Columns[i].Header = descr.Description;
                }
            }
            else if (grid != null)
            {
                foreach (var column in grid.Columns)
                {
                    column.Visibility = Visibility.Hidden;
                }
            }
        }

        private void UpdateGrid()
        {
            if (ItemsSource != null)
            {
                var type = ItemsSource.GetType().GenericTypeArguments[0];
                _filtersFactory = new FiltersFactory(FiltersPanel, type);

                ItemsGrid.ItemsSource = ItemsSource.ApplyFilters(FiltersPanel.Children.OfType<Filter>());
            }
        }

        private void DataGrid_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var grid = sender as DataGrid;
            if (grid == null) return;

            var entity = grid.SelectedItem as Entity;
            if (entity == null) return;

            var dlg = new EditScheduleDialog(entity, true) { ShowInTaskbar = true };
            dlg.Apply += delegate (object o, ApplyEventArgs args)
            {
                var item = args.Item;
                args.Handled = true;
                bool noExcept = true;

                using (ScheduleDbContext ctx = new ScheduleDbContext())
                {
                    try
                    {
                        var properties = item.GetType().GetProperties().Where(p => p.PropertyType.IsSubclassOf(typeof(Entity)));
                        foreach (var rel in properties.Select(p => p.GetValue(item) as Entity))
                            ctx.Set(rel.GetType()).Attach(rel);
                        ctx.Entry(item).State = EntityState.Modified;
                        ctx.SaveChanges();
                    }
                    catch (DbEntityValidationException ex)
                    {
                        ex.ShowMessage();
                        noExcept = false;
                    }
                }
                if (noExcept)
                {
                    dlg.Close();
                    UpdateGrid();
                }
            };
            dlg.Show();
        }

        private void AddFilterButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (_filtersFactory != null)
            {
                var filter = _filtersFactory.CreateFilter();
                FiltersPanel.Children.Add(filter);
            }
        }

        private void RemoveFiltersButton_OnClick(object sender, RoutedEventArgs e)
        {
            FiltersPanel.Children.Clear();
            ApplyFiltersButton_OnClick(sender, e);
        }

        private void ApplyFiltersButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (UpdateEvent != null)
                ItemsSource = UpdateEvent();

            UpdateGrid();
        }

        private void RemoveRecordButton_OnClick(object sender, RoutedEventArgs e)
        {
            var entity = ItemsGrid.SelectedItem as Entity;
            if (entity != null)
            {
                try
                {
                    using (ScheduleDbContext ctx = new ScheduleDbContext())
                    {
                        ctx.Entry(entity).State = EntityState.Deleted;
                        ctx.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    if (UpdateEvent != null)
                        ItemsSource = UpdateEvent();

                    UpdateGrid();
                }
            }
        }

        private void AddRecordButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (ItemsSource != null)
            {
                var type = ItemsSource.GetType().GenericTypeArguments[0];
                var dlg = new EditScheduleDialog(Activator.CreateInstance(type) as Entity);
                dlg.Apply += delegate (object o, ApplyEventArgs args)
                {
                    args.Handled = true;
                    var item = args.Item;
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
                        catch (DbEntityValidationException ex)
                        {
                            ex.ShowMessage();
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
                        ItemsSource = UpdateEvent();
                    }
                };
                dlg.Show();
            }
        }
    }
}
