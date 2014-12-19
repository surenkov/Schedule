using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media;
using Schedule.Controls.Calendar;
using Schedule.Models;
using Schedule.Models.ViewModels;
using Schedule.Models.ViewModels.Calendar;
using Schedule.Utils.Conflicts;

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

        public static IEnumerable<CalendarItemViewModel> Map(this Calendar calendar, IEnumerable<Models.Schedule> items, IEnumerable<Conflict> conflicts = null)
        {
            if (items == null || conflicts == null) return null;

            List<CalendarItemViewModel> list = new List<CalendarItemViewModel>();
            var groupedByTime = items.GroupBy(schedule => schedule.DoubleClass);

            foreach (var pairs in groupedByTime)
            {
                CalendarItemViewModel itemViewModel = new CalendarItemViewModel
                {
                    BorderBrush = PairColors[pairs.Key],
                    Period = pairs.Key,
                    Items = new Collection<ScheduleCardViewModel>(),
                    View = calendar
                };

                foreach (var item in pairs)
                    itemViewModel.Items.Add(new ScheduleCardViewModel { ScheduleView = calendar, Item = item, HasConflict = conflicts.Where(c => c.Schedule.CompareTo(item) == 0).Count() > 0 });

                list.Add(itemViewModel);
            }
            return list;
        }
    }
}
