using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Data;
using System.Windows.Media;
using Schedule.Annotations;

namespace Schedule.Models.ViewModels
{
    class ScheduleDay : INotifyPropertyChanged
    {
        private DateTime _date;
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

    static class ScheduleMapper
    {
        private static readonly Dictionary<DoubleClass, SolidColorBrush> PairColors = new Dictionary<DoubleClass, SolidColorBrush>
        {
            {DoubleClass.First, Brushes.LimeGreen},
            {DoubleClass.Second, Brushes.DodgerBlue},
            {DoubleClass.Third, Brushes.MediumPurple},
            {DoubleClass.Fourth, Brushes.Gold},
            {DoubleClass.Fifth, Brushes.Tomato},
            {DoubleClass.Sixth, Brushes.CornflowerBlue},
            {DoubleClass.Seventh, Brushes.HotPink},
            {DoubleClass.Eighth, Brushes.Sienna},
        };

        public static IEnumerable<ScheduleItem> Map(IEnumerable<Schedule> items)
        {
            if (items == null) return null;

            List<ScheduleItem> list = new List<ScheduleItem>();
            var groupByPairQuery = items.GroupBy(schedule => schedule.DoubleClass);

            foreach (IGrouping<DoubleClass, Schedule> schedules in groupByPairQuery)
            {
                ScheduleItem item = new ScheduleItem
                {
                    BorderBrush = PairColors[schedules.Key],
                    Period = schedules.Key,
                    Items = new Collection<Schedule>()
                };

                foreach (var schedule in schedules)
                    item.Items.Add(schedule);

                list.Add(item);
            }
            return list;
        }
    }
}
