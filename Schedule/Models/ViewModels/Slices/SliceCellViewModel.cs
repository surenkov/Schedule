using System.Collections.Generic;
using Schedule.Controls;

namespace Schedule.Models.ViewModels.Slices
{
    public class SliceCellViewModel : BaseViewModel
    {
        private IEnumerable<ScheduleCardViewModel> items;
        private IEnumerable<ScheduleCardViewModel> expanderItems;
        private ScheduleView scheduleView;
        private object horizontalValue;
        private object verticalValue;
        private bool isExpanded;
        private bool expanderVisibility;
        private string header;

        public IEnumerable<ScheduleCardViewModel> Items
        {
            get { return items; }
            set
            {
                items = value;
                OnPropertyChanged();
            }
        }

        public IEnumerable<ScheduleCardViewModel> ExpanderItems
        {
            get { return expanderItems; }
            set
            {
                expanderItems = value;
                OnPropertyChanged();
            }
        }

        public ScheduleView ScheduleView
        {
            get { return scheduleView; }
            set
            {
                scheduleView = value;
                OnPropertyChanged();
            }
        }

        public object HorizontalValue
        {
            get { return horizontalValue; }
            set
            {
                horizontalValue = value;
                OnPropertyChanged();
            }
        }

        public object VerticalValue
        {
            get { return verticalValue; }
            set
            {
                verticalValue = value;
                OnPropertyChanged();
            }
        }

        public bool IsExpanded
        {
            get { return isExpanded; }
            set
            {
                isExpanded = value;
                OnPropertyChanged();
            }
        }

        public bool ExpanderVisibility
        {
            get { return expanderVisibility; }
            set
            {
                expanderVisibility = value;
                OnPropertyChanged();
            }
        }

        public string Header
        {
            get { return header; }
            set
            {
                header = value;
                OnPropertyChanged();
            }
        }
    }
}
