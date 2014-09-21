using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Schedule.Models.DataLayer;

namespace Schedule.Windows
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Application.Current.ShutdownMode = ShutdownMode.OnMainWindowClose;
            //MonthSelector.DataContext = MainCalendar;
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
            //MainCalendar.UpdateSource = delegate(DateTime startTime, DateTime endTime)
            //{
            //    using (ScheduleDbContext ctx = new ScheduleDbContext())
            //    {
            //        var it = from s in ctx.Schedule.Include("Course").Include("Teacher").Include("Group").Include("Class.Building")
            //                 where s.EndDate >= startTime && s.StartDate <= endTime
            //                 orderby s.DoubleClass
            //                 select s;
            //        return it.ToList();
            //    }
            //};
        }
    }

    public static class ScheduleCommands
    {
        public static readonly RoutedUICommand Export = new RoutedUICommand(
            "Export in...",
            "Export",
             typeof(ScheduleCommands),
             new InputGestureCollection
             {
                 new KeyGesture(Key.E, ModifierKeys.Control)
             });
    }
}