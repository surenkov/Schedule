using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Schedule.Annotations;
using Schedule.Controls;

namespace Schedule.Models.ViewModels
{
    sealed class ScheduleCardViewModel : INotifyPropertyChanged
    {
        private Schedule _item;
        private Calendar _calendar;

        public Schedule Item
        {
            get { return _item; }
            set
            {
                _item = value;
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
