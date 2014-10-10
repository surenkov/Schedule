﻿using System.Collections.Generic;
using System.Windows.Media;
using Schedule.Controls;

namespace Schedule.Models.ViewModels.Calendar
{
    sealed class CalendarItemViewModel : BaseViewModel
    {
        private SolidColorBrush _brush;
        private ICollection<ScheduleCardViewModel> _items;
        private DoubleClass _period;
        private IScheduleView _scheduleView;

        public DoubleClass Period
        {
            get { return _period; }
            set
            {
                _period = value;
                OnPropertyChanged();
            }
        }
        
        public SolidColorBrush BorderBrush
        {
            get { return _brush; }
            set 
            { 
                _brush = value;
                OnPropertyChanged();
            }
        }

        public ICollection<ScheduleCardViewModel> Items
        {
            get { return _items; }
            set
            {
                _items = value;
                OnPropertyChanged();
            }
        }

        public IScheduleView ScheduleView
        {
            get { return _scheduleView; }
            set
            {
                _scheduleView = value;
                OnPropertyChanged();
            }
        }
    }
}