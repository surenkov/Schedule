using System.Collections.Generic;
using Schedule.Controls;

namespace Schedule.Models.ViewModels.Slices
{
    public class SliceCellViewModel : BaseViewModel
    {
        private IEnumerable<ScheduleCardViewModel> _items;
        private IEnumerable<ScheduleCardViewModel> _expanderItems;
        private IScheduleView _scheduleView;
        private object _horizontalValue;
        private object _verticalValue;
        private bool _isExpanded;
        private bool _expanderVisibility;

        public IEnumerable<ScheduleCardViewModel> Items
        {
            get { return _items; }
            set
            {
                _items = value;
                OnPropertyChanged();
            }
        }

        public IEnumerable<ScheduleCardViewModel> ExpanderItems
        {
            get { return _expanderItems; }
            set
            {
                _expanderItems = value;
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

        public bool IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                _isExpanded = value;
                OnPropertyChanged();
            }
        }

        public bool ExpanderVisibility
        {
            get { return _expanderVisibility; }
            set
            {
                _expanderVisibility = value;
                OnPropertyChanged();
            }
        }
    }
}
