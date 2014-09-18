using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media;
using Schedule.Models;
using Schedule.Models.ViewModels;

namespace Schedule.Utils
{
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

        public static IEnumerable<ScheduleItem> Map(IEnumerable<Models.Schedule> items)
        {
            if (items == null) return null;

            List<ScheduleItem> list = new List<ScheduleItem>();
            var groupByPairQuery = items.GroupBy(schedule => schedule.DoubleClass);

            foreach (IGrouping<DoubleClass, Models.Schedule> schedules in groupByPairQuery)
            {
                ScheduleItem item = new ScheduleItem
                {
                    BorderBrush = PairColors[schedules.Key],
                    Period = schedules.Key,
                    Items = new Collection<Models.Schedule>()
                };

                foreach (var schedule in schedules)
                    item.Items.Add(schedule);

                list.Add(item);
            }
            return list;
        }
    }
}
