using System;
using System.Collections;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Collections.Generic;

namespace Schedule.Controls.Slices
{
    [TemplatePart(Name = "PART_ExpandButton", Type = typeof(ToggleButton))]
    public class SliceCell : ItemsControl
    {
        public const int VisibleItemsCount = 2;

        public static readonly DependencyProperty IsExpandedProperty =
            DependencyProperty.Register("IsExpanded", typeof(bool), typeof(SliceCell));

        public bool IsExpanded
        {
            get { return (bool)GetValue(IsExpandedProperty); }
            set { SetValue(IsExpandedProperty, value); }
        }

        static SliceCell()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SliceCell), new FrameworkPropertyMetadata(typeof(SliceCell)));
        }

        public override void OnApplyTemplate()
        {
            var btn = GetTemplateChild("PART_ExpandButton") as ToggleButton;
            if (btn != null)
                btn.Click += (s, e) => OnIsExpandedChanged();
        }

        public void OnIsExpandedChanged()
        {
            RoutedEventArgs args = new RoutedEventArgs(SliceList.ExpandEvent);
            RaiseEvent(args);
            IsExpanded = !IsExpanded;
        }
    }
}
