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
using Schedule.Utils.Filters;

namespace Schedule.Windows
{
    public partial class MainWindow : Window
    {
        public static readonly DependencyProperty IsCalendarShownProperty;

        private FiltersFactory _filterFactory;

        static MainWindow()
        {
            IsCalendarShownProperty = DependencyProperty.Register("IsCalendarShown", typeof(Boolean), typeof(MainWindow),
                new PropertyMetadata(false, OnIsCalendarShownChanged));
        }

        private static void OnIsCalendarShownChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var window = d as MainWindow;
            if (window != null)
            {
                bool shown = window.IsCalendarShown;
                window.MainCalendar.Visibility = shown ? Visibility.Visible : Visibility.Collapsed;
                window.MonthSelector.Visibility = shown ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public bool IsCalendarShown
        {
            get { return (bool) GetValue(IsCalendarShownProperty); }
            set { SetValue(IsCalendarShownProperty, value); }
        }

        public MainWindow()
        {
            InitializeComponent();
            InitializeCardMenu();
            Application.Current.ShutdownMode = ShutdownMode.OnMainWindowClose;

            MonthSelector.DataContext = MainCalendar;
            ShowItem.DataContext = this;
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
            throw new NotImplementedException();
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
            MainSliceView.HorizontalHeaderType = typeof(Student);
            MainSliceView.VerticalHeaderType = typeof(Teacher);
            MainCalendar.UpdateSource = delegate(DateTime startTime, DateTime endTime)
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
                var dlg = new EntityCardViewDialog(item.Tag as Type);
                dlg.Show();
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
    }

    public static class ScheduleCommands
    {
        public static readonly RoutedUICommand Export = new RoutedUICommand(
            "Export as",
            "Export",
             typeof(ScheduleCommands),
             new InputGestureCollection
             {
                 new KeyGesture(Key.E, ModifierKeys.Control)
             });
    }
}