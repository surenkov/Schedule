using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using Schedule.Annotations;
using Schedule.Controls;

namespace Schedule.Models.ViewModels
{
    sealed class ScheduleItemViewModel : INotifyPropertyChanged
    {
        private SolidColorBrush _brush;
        private ICollection<ScheduleCardViewModel> _items;
        private DoubleClass _period;
        private Calendar _calendar;

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

        public Calendar Calendar
        {
            get { return _calendar; }
            set
            {
                _calendar = value;
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