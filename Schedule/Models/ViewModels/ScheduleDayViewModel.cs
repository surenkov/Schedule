using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Schedule.Annotations;
using Schedule.Controls;

namespace Schedule.Models.ViewModels
{
    sealed class ScheduleDayViewModel : INotifyPropertyChanged
    {
        private DateTime _date;
        private Calendar _calendar;
        private IEnumerable<ScheduleItemViewModel> _items;

        public DateTime Date
        {
            get { return _date; }
            set
            {
                _date = value;
                OnPropertyChanged();
            }
        }

        public Calendar Calendar
        {
            get { return _calendar; }
            set
            {
                _calendar = value;
                OnPropertyChanged();
            }
        }

        public IEnumerable<ScheduleItemViewModel> Items
        {
            get { return _items; }
            set
            {
                _items = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
