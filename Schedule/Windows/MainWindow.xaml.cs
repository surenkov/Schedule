using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Win32;
using Schedule.Controls.Editors;
using Schedule.Controls.Slices;
using Schedule.Models;
using Schedule.Models.DataLayer;
using Schedule.Models.ViewModels;
using Schedule.Models.ViewModels.Slices;
using Schedule.Utils.Export;
using Schedule.Utils.Filters;

namespace Schedule.Windows
{
    public partial class MainWindow : Window
    {
        private FiltersFactory filterFactory;
        private List<IExporter> exporters;

        public static readonly DependencyProperty CalendarVisibilityProperty = 
            DependencyProperty.Register("CalendarVisibility", typeof(bool), typeof(MainWindow),
                new PropertyMetadata(false, OnCalendarVisibilityChanged));

        public static readonly DependencyProperty SliceViewVisibilityProperty =
            DependencyProperty.Register("SliceViewVisibility", typeof(bool), typeof(MainWindow),
                new PropertyMetadata(true, OnSliceViewVisibilityChanged));

        public static readonly DependencyProperty SelectorViewModelsProperty = 
            DependencyProperty.Register("SelectorViewModels", typeof(ISet<SliceViewSelectorViewModel>), typeof(MainWindow),
                new PropertyMetadata(default(ISet<SliceViewSelectorViewModel>)));

        public bool CalendarVisibility
        {
            get { return (bool)GetValue(CalendarVisibilityProperty); }
            set { SetValue(CalendarVisibilityProperty, value); }
        }

        public bool SliceViewVisibility
        {
            get { return (bool)GetValue(SliceViewVisibilityProperty); }
            set { SetValue(SliceViewVisibilityProperty, value); }
        }

        public ISet<SliceViewSelectorViewModel> SelectorViewModels
        {
            get { return (ISet<SliceViewSelectorViewModel>)GetValue(SelectorViewModelsProperty); }
            set { SetValue(SelectorViewModelsProperty, value); }
        }

        private static void OnCalendarVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var window = d as MainWindow;
            if (window != null)
                window.SliceViewVisibility = !window.CalendarVisibility;
        }

        private static void OnSliceViewVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var window = d as MainWindow;
            if (window != null)
            {
                window.CalendarVisibility = !window.SliceViewVisibility;
                window.UpdateViews(window.FiltersPanel.Children.OfType<Filter>());
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            SliceViewVisibility = true;

            InitializeCardMenu();
            InitializeSelectorViewModels();
            InitializeExporters();
            Application.Current.ShutdownMode = ShutdownMode.OnMainWindowClose;

            filterFactory = new FiltersFactory(FiltersPanel, typeof(Models.Schedule), 30);
        }

        private void InitializeCardMenu()
        {
            CardsMenuItem.ItemsSource = new List<CardsMenuViewModel>
            {
                new CardsMenuViewModel { Header = "Courses", ItemsType = typeof(Course) },
                new CardsMenuViewModel { Header = "Teachers", ItemsType = typeof(Teacher) },
                new CardsMenuViewModel { Header = "Students", ItemsType = typeof(Student) },
                new CardsMenuViewModel { Header = "Groups", ItemsType = typeof(Group) },
                new CardsMenuViewModel { Header = "Faculties", ItemsType = typeof(Faculty) },
                new CardsMenuViewModel { Header = "Schools", ItemsType = typeof(School) },
                new CardsMenuViewModel { Header = "Buildings", ItemsType = typeof(Building) },
                new CardsMenuViewModel { Header = "Classes", ItemsType = typeof(Classroom) },
                new CardsMenuViewModel { Header = "Schedule", ItemsType = typeof(Models.Schedule) },
            };
        }

        private void InitializeSelectorViewModels()
        {
            SelectorViewModels = new HashSet<SliceViewSelectorViewModel>
            {
                new SliceViewSelectorViewModel { HeaderType = typeof(Teacher), Name = "Teachers" },
                new SliceViewSelectorViewModel { HeaderType = typeof(Group), Name = "Groups" },
                new SliceViewSelectorViewModel { HeaderType = typeof(DayOfWeek), Name = "Days of week" },
                new SliceViewSelectorViewModel { HeaderType = typeof(DateTime), Name = "Dates" },
                new SliceViewSelectorViewModel { HeaderType = typeof(Course), Name = "Courses" },
                new SliceViewSelectorViewModel { HeaderType = typeof(CourseType), Name = "Types of course" },
                new SliceViewSelectorViewModel { HeaderType = typeof(DoubleClass), Name = "Periods" }
            };

            HorizontalSelector.SelectedIndex = 2;
            VerticalSelector.SelectedIndex = 6;
        }

        private void InitializeExporters()
        {
            exporters = new List<IExporter> {
                new HtmlExporter(),
                new OpenXmlSpreadsheetExporter()
            };
        }

        private void Command_AlwaysExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void Window_Close(object sender, ExecutedRoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Window_ExportSchedule(object sender, ExecutedRoutedEventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            StringBuilder fmts = new StringBuilder();

            foreach (var exp in exporters)
                fmts.Append(exp.FormatString() + "|");
            dlg.Filter = fmts.Remove(fmts.Length - 1, 1).ToString();
            dlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            Dictionary<Type, object> sources = new Dictionary<Type, object>
            {
                { typeof(SliceView), MainSliceView },
                { typeof(Calendar), MainCalendar }
            };

            if (dlg.ShowDialog() == true)
            {
                var exporter = exporters[dlg.FilterIndex - 1];
                exporter.Save(dlg.FileName, sources[exporter.SourceType()]);
            }
        }

        private void Window_ShowConflicts(object sender, RoutedEventArgs e)
        {
            var conflicts = new ConflictsWindow { Owner = this };
            conflicts.Update += (s, evt) => UpdateViews(FiltersPanel.Children.OfType<Filter>());
            conflicts.Show();
        }

        private void Window_AboutAuthor(object sender, RoutedEventArgs e)
        {
            var author = new AboutWindow();
            author.ShowDialog();
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            MainCalendar.UpdateSource = delegate (DateTime startTime, DateTime endTime)
            {
                using (ScheduleDbContext ctx = new ScheduleDbContext())
                {
                    var it = from s in ctx.Schedule.Include("Course").Include("Teacher").Include("Group.Students").Include("Class.Building")
                             where s.EndDate >= startTime && s.StartDate <= endTime
                             orderby s.DoubleClass
                             select s;
                    return it.ToList();
                }
            };
        }

        private void CardMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var item = sender as MenuItem;
            if (item != null)
            {
                var dlg = new EntityGridViewDialog();
                dlg.Show();

                dlg.LoadEvent = delegate ()
                {
                    var type = item.Tag as Type;
                    using (ScheduleDbContext ctx = new ScheduleDbContext())
                    {
                        ctx.Configuration.LazyLoadingEnabled = false;
                        var entityProps = type.GetProperties().Where(p => p.PropertyType.IsSubclassOf(typeof(Entity))).ToList();
                        ctx.Set(type).Load();

                        foreach (var entity in ctx.Set(type).Local)
                            foreach (var prop in entityProps)
                                ctx.Entry(entity).Reference(prop.Name).Load();

                        return ctx.Set(type).Local as IEnumerable<Entity>;
                    }
                };

                dlg.ItemsChanged += delegate (object s, RoutedEventArgs evt)
                {
                    e.Handled = true;
                    UpdateViews(FiltersPanel.Children.OfType<Filter>());
                };
            }
        }

        private void ApplyFiltersButton_OnClick(object sender, RoutedEventArgs e)
        {
            UpdateViews(FiltersPanel.Children.OfType<Filter>());
        }

        private void RemoveFiltersButton_OnClick(object sender, RoutedEventArgs e)
        {
            FiltersPanel.Children.Clear();
            UpdateViews(null);
        }

        private void AddFilterButton_OnClick(object sender, RoutedEventArgs e)
        {
            var filter = filterFactory.CreateFilter();
            FiltersPanel.Children.Add(filter);
        }

        private void UpdateViews(IEnumerable<Filter> filters)
        {
            MainCalendar.Filters = MainSliceView.Filters = filters;
            if (CalendarVisibility)
                MainCalendar.UpdateView();
            if (SliceViewVisibility)
                MainSliceView.UpdateView();
        }

        private void HeaderSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            e.Handled = true;
            if (sender.Equals(HorizontalSelector))
            {
                var item = HorizontalSelector.SelectedItem as SliceViewSelectorViewModel;
                if (item != null) MainSliceView.HorizontalHeaderType = item.HeaderType;
            }
            else if (sender.Equals(VerticalSelector))
            {
                var item = VerticalSelector.SelectedItem as SliceViewSelectorViewModel;
                if (item != null) MainSliceView.VerticalHeaderType = item.HeaderType;
            }
        }
    }

    public static class ScheduleCommands
    {
        public static readonly RoutedUICommand Export = new RoutedUICommand(
            "Export as...",
            "Export",
             typeof(ScheduleCommands),
             new InputGestureCollection
             {
                 new KeyGesture(Key.E, ModifierKeys.Control)
             });
    }
}