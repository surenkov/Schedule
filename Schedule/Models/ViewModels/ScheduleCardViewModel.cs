using Schedule.Controls;

namespace Schedule.Models.ViewModels
{
    public class ScheduleCardViewModel : BaseViewModel
    {
        private Schedule _item;
        private IScheduleView _scheduleView;

        public Schedule Item
        {
            get { return _item; }
            set
            {
                _item = value;
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
    }
}
