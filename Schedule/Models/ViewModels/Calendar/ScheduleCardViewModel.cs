namespace Schedule.Models.ViewModels.Calendar
{
    sealed class ScheduleCardViewModel : BaseViewModel
    {
        private Schedule _item;
        private Controls.Calendar.Calendar _calendar;

        public Schedule Item
        {
            get { return _item; }
            set
            {
                _item = value;
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
