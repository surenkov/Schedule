using System.Collections.Generic;
using System.Windows.Media;

namespace Schedule.Models.ViewModels.Calendar
{
    sealed class ScheduleItemViewModel : BaseViewModel
    {
        private SolidColorBrush _brush;
        private ICollection<ScheduleCardViewModel> _items;
        private DoubleClass _period;
        private Controls.Calendar.Calendar _calendar;

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

        public Controls.Calendar.Calendar Calendar
        {
            get { return _calendar; }
            set
            {
                _calendar = value;
                OnPropertyChanged();
            }
        }
    }
}