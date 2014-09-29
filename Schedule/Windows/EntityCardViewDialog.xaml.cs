using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Schedule.Attributes;
using Schedule.Controls.Editors;
using Schedule.Models;
using Schedule.Models.DataLayer;
using Schedule.Utils;
using Schedule.Utils.Filters;

namespace Schedule.Windows
{
    public partial class EntityCardViewDialog : Window
    {
        public static readonly DependencyProperty ItemsTypeProperty;

        private FiltersFactory _filtersFactory;

        static EntityCardViewDialog()
        {
            ItemsTypeProperty = DependencyProperty.Register("ItemsType", typeof(Type),
                typeof(EntityCardViewDialog));
        }

        public EntityCardViewDialog(Type type)
        {
            InitializeComponent();

            ItemsType = type;
            var dpd = DependencyPropertyDescriptor.FromProperty(ItemsControl.ItemsSourceProperty, typeof(DataGrid));
            if (dpd != null) dpd.AddValueChanged(ItemsGrid, ItemsGrid_UpdateColumnsVisibility);
            InitializeGrid();

            DataContext = this;
        }

        private void ItemsGrid_UpdateColumnsVisibility(object sender, EventArgs eventArgs)
        {
            var grid = sender as DataGrid;
            if (grid != null)
            {
                var properties = ItemsType.GetProperties();
                for (int i = 0; i < properties.Length; i++)
                {

                    var descr = properties[i].GetCustomAttribute(typeof(DescriptionAttribute)) as DescriptionAttribute;
                    var shown = properties[i].GetCustomAttribute(typeof(NotShownAttribute)) as NotShownAttribute;

                    bool show = properties[i].PropertyType == typeof(string) ||
                                properties[i].PropertyType.GetInterface("IEnumerable") == null || 
                                shown != null && shown.Shown;

                    grid.Columns[i].Visibility = show ? Visibility.Visible : Visibility.Hidden;
                    if (descr != null) 
                        grid.Columns[i].Header = descr.Description;
                }
            }
        }

        private void InitializeGrid()
        {
            _filtersFactory = new FiltersFactory(FiltersPanel, ItemsType);

            if (ItemsType != null)
                UpdateGrid();
        }

        private async void UpdateGrid()
        {
            using (ScheduleDbContext ctx = new ScheduleDbContext())
            {
                ctx.Configuration.LazyLoadingEnabled = false;
                var entityProps = ItemsType.GetProperties().Where(p => p.PropertyType.IsSubclassOf(typeof(Entity))).ToList();
                await ctx.Set(ItemsType).LoadAsync();

                foreach (var entity in ctx.Set(ItemsType).Local)
                    foreach (var prop in entityProps)
                        await ctx.Entry(entity).Reference(prop.Name).LoadAsync();

                ItemsGrid.ItemsSource = (ctx.Set(ItemsType).Local as IEnumerable<Entity>).ApplyFilters(FiltersPanel.Children.OfType<Filter>());
            }
        }

        public Type ItemsType
        {
            get { return (Type) GetValue(ItemsTypeProperty); }
            set { SetValue(ItemsTypeProperty, value); }
        }

        private void DataGrid_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var grid = sender as DataGrid;
            if (grid == null) return;

            var entity = grid.SelectedItem as Entity;
            if (entity == null) return;

            var dlg = new EditScheduleDialog(entity, true) { ShowInTaskbar = true };
            dlg.Apply += delegate(object o, ApplyEventArgs args)
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
            var filter = _filtersFactory.CreateFilter();
            FiltersPanel.Children.Add(filter);
        }

        private void RemoveFiltersButton_OnClick(object sender, RoutedEventArgs e)
        {
            FiltersPanel.Children.Clear();
            ApplyFiltersButton_OnClick(sender, e);
        }

        private void ApplyFiltersButton_OnClick(object sender, RoutedEventArgs e)
        {
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
                    UpdateGrid();
                }
            }
        }

        private void AddRecordButton_OnClick(object sender, RoutedEventArgs e)
        {
            var dlg = new EditScheduleDialog(Activator.CreateInstance(ItemsType) as Entity);
            dlg.Apply += delegate(object o, ApplyEventArgs args)
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
                    UpdateGrid();
                }
            };
            dlg.Show();
        }
    }
}
