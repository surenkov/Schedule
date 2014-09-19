using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media;
using Schedule.Controls;
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

        public static IEnumerable<ScheduleItemViewModel> Map(IEnumerable<Models.Schedule> items, Calendar calendar)
        {
            if (items == null) return null;

            List<ScheduleItemViewModel> list = new List<ScheduleItemViewModel>();
            var groupByPairQuery = items.GroupBy(schedule => schedule.DoubleClass);

            foreach (IGrouping<DoubleClass, Models.Schedule> schedules in groupByPairQuery)
            {
                ScheduleItemViewModel itemViewModel = new ScheduleItemViewModel
                {
                    BorderBrush = PairColors[schedules.Key],
                    Period = schedules.Key,
                    Items = new Collection<ScheduleCardViewModel>(),
                    Calendar = calendar
                };

                foreach (var schedule in schedules)
                    itemViewModel.Items.Add(new ScheduleCardViewModel { Calendar = calendar, Item = schedule });

                list.Add(itemViewModel);
            }
            return list;
        }
    }
}
