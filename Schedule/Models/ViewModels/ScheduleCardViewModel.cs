﻿using Schedule.Controls;

namespace Schedule.Models.ViewModels
{
    public class ScheduleCardViewModel : BaseViewModel
    {
        private Schedule item;
        private ScheduleView scheduleView;
        private bool hasConflict;

        public Schedule Item
        {
            get { return item; }
            set
            {
                item = value;
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

        public bool HasConflict
        {
            get { return hasConflict; }
            set
            {
                hasConflict = value;
                OnPropertyChanged();
            }
        }
    }
}
