using System.Windows;

namespace Schedule.Windows
{
    public partial class ConflictsWindow : Window
    {
        public ConflictsWindow()
        {
            InitializeComponent();
        }

        private void ConflictsWindow_Close(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}