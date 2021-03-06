﻿using System;
using System.Windows;
using System.Windows.Controls;

namespace Schedule.Controls.Editors
{
    public partial class ScheduleDatesSelector : UserControl
    {
        public DateTime StartDate
        {
            get { return (DateTime)GetValue(StartDateProperty); }
            set { SetValue(StartDateProperty, value); }
        }

        public static readonly DependencyProperty StartDateProperty =
            DependencyProperty.Register("StartDate", typeof(DateTime), typeof(ScheduleDatesSelector), 
                new PropertyMetadata(DateTime.Now.Date));

        public DateTime EndDate
        {
            get { return (DateTime)GetValue(EndDateProperty); }
            set { SetValue(EndDateProperty, value); }
        }

        public static readonly DependencyProperty EndDateProperty =
            DependencyProperty.Register("EndDate", typeof(DateTime), typeof(ScheduleDatesSelector), 
                new PropertyMetadata(DateTime.Now.Date.AddMonths(1)));


        public ScheduleDatesSelector()
        {
            InitializeComponent();

            StartDatePicker.DataContext = this;
            EndDatePicker.DataContext = this;
        }
    }
}
