using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using Schedule.Annotations;
using Schedule.Controls;

namespace Schedule.Models.ViewModels
{
    class ScheduleDay : INotifyPropertyChanged
    {
        private DateTime _date;
        private Calendar _calendar;
        private IEnumerable<ScheduleItem> _items;

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

        public IEnumerable<ScheduleItem> Items
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
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    class ScheduleItem : INotifyPropertyChanged
    {
        private SolidColorBrush _brush;
        private ICollection<Schedule> _items;
        private DoubleClass _period;

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

        public ICollection<Schedule> Items
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
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
