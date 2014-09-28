using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Schedule.Attributes;
using Schedule.Controls.Editors;
using Schedule.Models;
using Schedule.Utils.Filters;

namespace Schedule.Windows
{
    public partial class EntityCardViewDialog : Window
    {
        public static readonly DependencyProperty ItemsProperty;

        private FiltersFactory _filtersFactory;

        static EntityCardViewDialog()
        {
            ItemsProperty = DependencyProperty.Register("Items", typeof(IEnumerable<Entity>),
                typeof(EntityCardViewDialog));
        }

        public EntityCardViewDialog()
        {
            InitializeComponent();
            DataContext = this;

            var dpd = DependencyPropertyDescriptor.FromProperty(ItemsControl.ItemsSourceProperty, typeof(DataGrid));
            if (dpd != null) dpd.AddValueChanged(ItemsGrid, DataGrid_UpdateColumnsVisibility);
        }

        private void DataGrid_UpdateColumnsVisibility(object sender, EventArgs eventArgs)
        {
            var grid = sender as DataGrid;
            if (grid != null)
            {
                _filtersFactory = new FiltersFactory(FilterPanel, grid.ItemsSource.GetType().GenericTypeArguments[0]);
                var properties = grid.ItemsSource.GetType().GenericTypeArguments[0].GetProperties();

                for (int i = 0; i < properties.Length; i++)
                {
                    var property = properties[i];
                    var show = property.GetCustomAttribute(typeof(NotShownAttribute)) as NotShownAttribute;

                    try
                    {
                        if (property.PropertyType != typeof(string) &&
                            property.PropertyType.GetInterface("IEnumerable") != null ||
                            show != null && !show.Shown)
                            grid.Columns[i].Visibility = Visibility.Hidden;
                    }
                    catch {}
                }
            }
        }

        public IEnumerable<Entity> Items
        {
            get { return (IEnumerable<Entity>) GetValue(ItemsProperty); }
            set { SetValue(ItemsProperty, value); }
        }

        private void DataGrid_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var grid = sender as DataGrid;
            if (grid == null) return;

            var entity = grid.SelectedItem as Entity;
            if (entity == null) return;

            var dlg = new EditScheduleDialog(entity, true) { ShowInTaskbar = true };
            dlg.Show();
        }

        private void AddFilterButton_OnClick(object sender, RoutedEventArgs e)
        {
            var filter = _filtersFactory.CreateFilter();
            FilterPanel.Children.Add(filter);
        }

        private void RemoveFiltersButton_OnClick(object sender, RoutedEventArgs e)
        {
            FilterPanel.Children.Clear();
            ApplyFiltersButton_OnClick(sender, e);
        }

        private void ApplyFiltersButton_OnClick(object sender, RoutedEventArgs e)
        {
            ItemsGrid.ItemsSource = Items.ApplyFilters(FilterPanel.Children.OfType<Filter>());
        }
    }
}
