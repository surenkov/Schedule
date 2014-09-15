using System.Windows;

namespace Schedule.Windows
{
    public partial class EditScheduleDialog : Window
    {
        public static readonly DependencyProperty ItemProperty = 
            DependencyProperty.Register("Item", typeof(Models.Schedule), typeof(EditScheduleDialog));

        public Models.Schedule Item
        {
            get { return (Models.Schedule)GetValue(ItemProperty); }
            set { SetValue(ItemProperty, value); }
        }

        public EditScheduleDialog()
        {
            InitializeComponent();
            CloseButton.Click += (s, a) => Close();
        }
    }
}
