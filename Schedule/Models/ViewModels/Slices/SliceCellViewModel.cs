using System.Collections.Generic;
using Schedule.Controls;

namespace Schedule.Models.ViewModels.Slices
{
    public class SliceCellViewModel : BaseViewModel
    {
        private IEnumerable<ScheduleCardViewModel> _items;
        private IScheduleView _scheduleView;
        private object _horizontalValue;
        private object _verticalValue;

        public IEnumerable<ScheduleCardViewModel> Items
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

        public object HorizontalValue
        {
            get { return _horizontalValue; }
            set
            {
                _horizontalValue = value;
                OnPropertyChanged();
            }
        }

        public object VerticalValue
        {
            get { return _verticalValue; }
            set
            {
                _verticalValue = value;
                OnPropertyChanged();
            }
        }
    }
}
