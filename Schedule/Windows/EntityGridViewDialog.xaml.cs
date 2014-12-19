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
using System.Windows.Data;
using Schedule.Utils.Attributes;
using Schedule.Controls.Editors;
using Schedule.Models;
using Schedule.Models.DataLayer;
using Schedule.Utils;
using Schedule.Utils.Filters;

namespace Schedule.Windows
{
    public delegate IEnumerable<Entity> EntityCardViewDialogUpdateEvent();

    public partial class EntityGridViewDialog : Window
    {
        private FiltersFactory filtersFactory;
        private Dictionary<int, EditEntityDialog> editDialogs;

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(IEnumerable<Entity>), typeof(EntityGridViewDialog),
                new PropertyMetadata(null, ItemsSourceChangedCallback));

        public static readonly DependencyProperty ShowHiddenPropertiesProperty =
            DependencyProperty.Register("ShowHiddenProperties", typeof(bool), typeof(EntityGridViewDialog),
                new PropertyMetadata(false, ShowHiddenPropertiesPropertyChanged));

        public static readonly DependencyProperty LoadEventProperty =
            DependencyProperty.Register("LoadEvent", typeof(EntityCardViewDialogUpdateEvent), typeof(EntityGridViewDialog),
                new PropertyMetadata(null, LoadEventPropertyChangedCallback));

        public static readonly RoutedEvent ItemsChangedEvent =
            EventManager.RegisterRoutedEvent("ItemsChanged", RoutingStrategy.Direct,
                typeof(RoutedEventHandler), typeof(EntityGridViewDialog));

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

        public EntityCardViewDialogUpdateEvent LoadEvent
        {
            get { return (EntityCardViewDialogUpdateEvent)GetValue(LoadEventProperty); }
            set { SetValue(LoadEventProperty, value); }
        }

        public event RoutedEventHandler ItemsChanged
        {
            add { AddHandler(ItemsChangedEvent, value); }
            remove { RemoveHandler(ItemsChangedEvent, value); }
        }

        private static void ShowHiddenPropertiesPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var dlg = d as EntityGridViewDialog;
            if (dlg != null)
                dlg.UpdateGrid();
        }

        private static void LoadEventPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var dlg = d as EntityGridViewDialog;
            if (dlg != null)
            {
                dlg.RecordsPanel.IsEnabled = e.NewValue != null;
                dlg.LoadData();
            }
        }

        private static void ItemsSourceChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var dlg = d as EntityGridViewDialog;
            if (dlg != null)
            {
                dlg.UpdateGrid();
                dlg.RaiseEvent(new RoutedEventArgs(ItemsChangedEvent));
            }
        }


        public EntityGridViewDialog()
        {
            InitializeComponent();
            editDialogs = new Dictionary<int, EditEntityDialog>();
            DataContext = this;
        }

        private void UpdateGrid()
        {
            if (ItemsSource != null)
            {
                ItemsGrid.Columns.Clear();
                var enumerator = ItemsSource.GetEnumerator();
                bool contains = enumerator.MoveNext();

                if (!contains) return;
                filtersFactory = new FiltersFactory(FiltersPanel, enumerator.Current.GetType());
                var properties = enumerator.Current.GetType().GetProperties();
                foreach (var property in properties)
                {
                    var descr = property.GetCustomAttribute(typeof(DescriptionAttribute)) as DescriptionAttribute;
                    var hide = property.GetCustomAttribute(typeof(HiddenAttribute)) as HiddenAttribute;

                    bool show = hide == null || !hide.Hidden || ShowHiddenProperties;
                    string header = descr != null ? descr.Description : property.Name;

                    if (!show) continue;
                    ItemsGrid.Columns.Add(new DataGridTextColumn()
                    {
                        Header = header,
                        Binding = new Binding(property.Name)
                    });
                }

                ItemsGrid.ItemsSource = ItemsSource.ApplyFilters(FiltersPanel.Children.OfType<Filter>());
            }
        }

        private void LoadData()
        {
            if (LoadEvent != null)
                ItemsSource = LoadEvent();
        }

        private void DataGrid_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var grid = sender as DataGrid;
            if (grid == null) return;

            var entity = grid.SelectedItem as Entity;
            if (entity == null) return;

            Cursor = Cursors.AppStarting;

            if (!editDialogs.ContainsKey(entity.Id))
            {
                editDialogs[entity.Id] = new EditEntityDialog(entity, true)
                {
                    ShowInTaskbar = true,
                    Title = "Edit " + entity.GetType().BaseType.Name + " entity"
                };
            }

            var dlg = editDialogs[entity.Id];

            dlg.Apply += delegate (object o, ApplyEventArgs args)
            {
                var item = args.Item;
                args.Handled = true;

                using (ScheduleDbContext ctx = new ScheduleDbContext())
                {
                    try
                    {
                        var properties = item.GetType().GetProperties().Where(p => p.PropertyType.IsSubclassOf(typeof(Entity)));
                        foreach (var rel in properties.Select(p => p.GetValue(item) as Entity))
                            ctx.Set(rel.GetType()).Attach(rel);
                        ctx.Entry(item).State = EntityState.Modified;
                        ctx.SaveChanges();

                        dlg.Close();
                        LoadData();
                        RaiseEvent(new RoutedEventArgs(ItemsChangedEvent));
                    }
                    catch (DbEntityValidationException ex)
                    {
                        ex.ShowMessage();
                    }
                }
            };

            dlg.Closed += (s, args) => editDialogs.Remove(entity.Id);

            dlg.Show();
            dlg.Activate();

            Cursor = Cursors.Arrow;
        }

        private void AddFilterButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (filtersFactory != null)
            {
                var filter = filtersFactory.CreateFilter();
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
            LoadData();
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
                    if (LoadEvent != null)
                        ItemsSource = LoadEvent();

                    UpdateGrid();
                }
            }
        }

        private void AddRecordButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (ItemsSource == null) return;

            var type = ItemsSource.GetType().GenericTypeArguments[0];
            var dlg = new EditEntityDialog(Activator.CreateInstance(type) as Entity)
            {
                ShowInTaskbar = true,
                Title = "Add " + type.Name + " record"
            };
            dlg.Apply += delegate (object o, ApplyEventArgs args)
            {
                args.Handled = true;
                var item = args.Item;

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
                        LoadData();
                    }
                    catch (DbEntityValidationException ex)
                    {
                        ex.ShowMessage();
                    }
                    catch (NullReferenceException)
                    {
                        MessageBox.Show("Please fill all required fields", "Error");
                    }
                }
            };
            dlg.Show();
        }
    }
}
