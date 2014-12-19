using Schedule.Controls;
using System;
using System.Collections.Generic;

namespace Schedule.Models.ViewModels.Calendar
{
    sealed class CalendarDayViewModel : BaseViewModel
    {
        private DateTime _date;
        private ScheduleView _view;
        private IEnumerable<CalendarItemViewModel> _items;
        private bool _headerVisibility;

        public DateTime Date
        {
            get { return _date; }
            set
            {
                _date = value;
                OnPropertyChanged();
            }
        }

        public ScheduleView View
        {
            get { return _view; }
            set
            {
                _view = value;
                OnPropertyChanged();
            }
        }

        public IEnumerable<CalendarItemViewModel> Items
        {
            get { return _items; }
            set
            {
                _items = value;
                OnPropertyChanged();
            }
        }

        public bool HeaderVisibility
        {
            get { return _headerVisibility; }
            set
            {
                _headerVisibility = value;
                OnPropertyChanged();
            }
        }
    }
}