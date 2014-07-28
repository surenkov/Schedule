using System;
using System.Data.Entity.Core.Metadata.Edm;
using System.Windows;
using System.Windows.Input;

namespace Schedule.Windows
{
    public partial class MainWindow : Window
    {
        private ConflictsWindow _conflicts;
        public MainWindow()
        {
            InitializeComponent();
            Application.Current.ShutdownMode = ShutdownMode.OnMainWindowClose;
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
            if (_conflicts == null)
                _conflicts = new ConflictsWindow { Owner = this };
            _conflicts.Closed += (o, args) => { _conflicts = null; };
            _conflicts.ShowDialog();
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
