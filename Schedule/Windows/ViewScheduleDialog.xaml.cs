using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Schedule.Windows
{
    public partial class ViewScheduleDialog : Window
    {
        public static readonly DependencyProperty ItemsSourceProperty;

        static ViewScheduleDialog()
        {
            ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof(IEnumerable),
                typeof(ViewScheduleDialog));
        }

        public ViewScheduleDialog()
        {
            InitializeComponent();
            DataContext = this;
        }

        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

    }
}
