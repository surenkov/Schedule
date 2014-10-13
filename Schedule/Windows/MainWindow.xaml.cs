using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Schedule.Controls.Editors;
using Schedule.Models;
using Schedule.Models.DataLayer;
using Schedule.Models.ViewModels;
using Schedule.Models.ViewModels.Slices;
using Schedule.Utils.Filters;
using Schedule.Utils.Export;
using Microsoft.Win32;
using System.Text;
using System.Data.Entity;

namespace Schedule.Windows
{
    public partial class MainWindow : Window
    {
        public static readonly DependencyProperty IsCalendarShownProperty;
        public static readonly DependencyProperty SelectorViewModelsProperty;
        public ISet<SliceViewSelectorViewModel> SelectorViewModels
        {
            get { return (ISet<SliceViewSelectorViewModel>)GetValue(SelectorViewModelsProperty); }
            set { SetValue(SelectorViewModelsProperty, value); }
        }
        private FiltersFactory _filterFactory;
        private List<IExporter> _exporters;

        static MainWindow()
        {
            IsCalendarShownProperty = DependencyProperty.Register("IsCalendarShown", typeof(Boolean), typeof(MainWindow),
                new PropertyMetadata(false, OnIsCalendarShownChanged));
            SelectorViewModelsProperty = DependencyProperty.Register(
                "SelectorViewModels", typeof(ISet<SliceViewSelectorViewModel>), typeof(MainWindow),
                new PropertyMetadata(default(ISet<SliceViewSelectorViewModel>)));
        }

        private static void OnIsCalendarShownChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var window = d as MainWindow;
            if (window != null)
            {
                bool shown = window.IsCalendarShown;

                window.MonthSelector.Visibility = window.MainCalendar.Visibility =
                    shown ? Visibility.Visible : Visibility.Collapsed;

                window.MainSliceView.Visibility = window.SelectorsPanel.Visibility =
                    shown ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        public bool IsCalendarShown
        {
            get { return (bool)GetValue(IsCalendarShownProperty); }
            set { SetValue(IsCalendarShownProperty, value); }
        }

        public MainWindow()
        {
            InitializeComponent();
            InitializeCardMenu();
            InitializeSelectorViewModels();
            InitializeExporters();
            Application.Current.ShutdownMode = ShutdownMode.OnMainWindowClose;

            MonthSelector.DataContext = MainCalendar;
            DataContext = this;
            _filterFactory = new FiltersFactory(FiltersPanel, typeof(Models.Schedule), 30);
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
                new SliceViewSelectorViewModel {HeaderType = typeof(Teacher), Name = "Teachers"},
                new SliceViewSelectorViewModel {HeaderType = typeof(Group), Name = "Groups"},
                new SliceViewSelectorViewModel {HeaderType = typeof(DayOfWeek), Name = "Days"},
                new SliceViewSelectorViewModel {HeaderType = typeof(Course), Name = "Courses"},
                new SliceViewSelectorViewModel {HeaderType = typeof(CourseType), Name = "Types of course"},
                new SliceViewSelectorViewModel {HeaderType = typeof(DoubleClass), Name = "Periods"}
            };

            HorizontalSelector.SelectedIndex = 2;
            VerticalSelector.SelectedIndex = 5;
        }

        private void InitializeExporters()
        {
            _exporters = new List<IExporter> {
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

            foreach (var exp in _exporters)
                fmts.Append(exp.FormatString() + "|");
            fmts.Remove(fmts.Length - 1, 1); 
            dlg.Filter = fmts.ToString();
            dlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            if (dlg.ShowDialog() == true)
                _exporters[dlg.FilterIndex - 1].Save(dlg.FileName, MainSliceView, FiltersPanel.Children.OfType<Filter>());
        }

        private void Window_ShowConflicts(object sender, RoutedEventArgs e)
        {
            var conflicts = new ConflictsWindow { Owner = this };
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
                    var it = from s in ctx.Schedule.Include("Course").Include("Teacher").Include("Group").Include("Class.Building")
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
                var dlg = new EntityCardViewDialog();
                dlg.Show();

                dlg.UpdateEvent = delegate ()
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
                dlg.Closed += (s, evt) => UpdateViews(FiltersPanel.Children.OfType<Filter>());
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
            var filter = _filterFactory.CreateFilter();
            FiltersPanel.Children.Add(filter);
        }

        private void UpdateViews(IEnumerable<Filter> filters)
        {
            MainCalendar.Filters = MainSliceView.Filters = filters;
            if (MainCalendar.Visibility == Visibility.Visible)
                MainCalendar.UpdateView();
            if (MainSliceView.Visibility == Visibility.Visible)
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