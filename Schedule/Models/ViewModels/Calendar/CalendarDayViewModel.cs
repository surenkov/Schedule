using System;
using System.Collections.Generic;

namespace Schedule.Models.ViewModels.Calendar
{
    sealed class CalendarDayViewModel : BaseViewModel
    {
        private DateTime _date;
        private Controls.Calendar.Calendar _calendar;
        private IEnumerable<CalendarItemViewModel> _items;

        public DateTime Date
        {
            get { return _date; }
            set
            {
                _date = value;
                OnPropertyChanged();
            }
        }

        public Controls.Calendar.Calendar Calendar
        {
            get { return _calendar; }
            set
            {
                _calendar = value;
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
    }
}
