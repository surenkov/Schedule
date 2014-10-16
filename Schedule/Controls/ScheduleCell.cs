using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Schedule.Controls
{

    [TemplatePart(Name = "PART_AddButton", Type = typeof(Button))]
    [TemplatePart(Name = "PART_ViewButton", Type = typeof(Button))]
    public abstract class ScheduleCell : HeaderedItemsControl
    {
        private Button addButton;
        private Button viewButton;

        public static readonly DependencyProperty ViewProperty =
            DependencyProperty.Register("View", typeof(IScheduleView), typeof(ScheduleCell));

        public IScheduleView View
        {
            get { return (IScheduleView)GetValue(ViewProperty); }
            set { SetValue(ViewProperty, value); }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            addButton = GetTemplateChild("PART_AddButton") as Button;
            viewButton = GetTemplateChild("PART_ViewButton") as Button;

            if (addButton != null)
                addButton.Click += OnAddButtonClick;
            if (viewButton != null)
                viewButton.Click += OnViewButtonClick;
        }

        protected abstract void OnAddButtonClick(object sender, RoutedEventArgs args);

        protected abstract void OnViewButtonClick(object sender, RoutedEventArgs args);
    }
}
