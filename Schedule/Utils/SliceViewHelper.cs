using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Collections;
using Schedule.Controls.Slices;
using Schedule.Models;
using Schedule.Models.DataLayer;
using Schedule.Models.ViewModels;
using Schedule.Models.ViewModels.Slices;
using Schedule.Utils.Conflicts;

namespace Schedule.Utils
{
    public delegate IEnumerable SliceHeadersFillCallback(SliceView view, Type type);

    public delegate bool SliceViewItemsComparer(object header, Models.Schedule item);

    static class SliceViewHelper
    {
        private static readonly Dictionary<Type, SliceHeadersFillCallback> FillCallbacks =
            new Dictionary<Type, SliceHeadersFillCallback>
            {
                { typeof(DayOfWeek), FillDayHeader },
                { typeof(Entity), FillEntityHeader },
                { typeof(Enum), FillEnumHeader },
                { typeof(DateTime),  FillDateTimeHeader}
            };

        public static readonly Dictionary<Type, SliceViewItemsComparer> Comparers =
            new Dictionary<Type, SliceViewItemsComparer>
            {
                { typeof(DoubleClass), (header, item) => item.DoubleClass == (DoubleClass) header },
                { typeof(CourseType), (header, item) => item.Type == (CourseType) header },
                { typeof(DayOfWeek), (header, item) => item.DaysOfWeek().Contains((DayOfWeek)header) },
                { typeof(DateTime), CompareDateTimeHeader },
                { typeof(Teacher), (header, item) => item.Teacher.CompareTo(header as Entity) == 0 },
                { typeof(Course), (header, item) => item.Course.CompareTo(header as Entity) == 0 },
                { typeof(Group), (header, item) => item.Group.CompareTo(header as Entity) == 0 },
                { typeof(Classroom), (header, item) => item.Class.CompareTo(header as Entity) == 0 },
            };

        private static bool CompareDateTimeHeader(object header, Models.Schedule item)
        {
            var date = (DateTime)header;
            return date >= item.StartDate && (date - item.StartDate).Days % item.Interval == 0 && date <= item.EndDate;
        }

        private static IEnumerable FillEnumHeader(SliceView view, Type type)
        {
            var enums = new List<Enum>(type.GetEnumValues().Cast<Enum>());
            return enums;
        }

        private static IEnumerable FillEntityHeader(SliceView view, Type type)
        {
            using (ScheduleDbContext ctx = new ScheduleDbContext())
            {
                ctx.Set(type).Load();
                return ctx.Set(type).Local;
            }
        }

        private static IEnumerable FillDayHeader(SliceView view, Type type)
        {
            List<DayOfWeek> daysOfWeek = new List<DayOfWeek>(7)
            {
                CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek
            };

            for (int i = (int)daysOfWeek[0] + 1; i < (int)daysOfWeek[0] + 7; i++)
                daysOfWeek.Add((DayOfWeek)(i % 7));
            return daysOfWeek;
        }

        public static IEnumerable FillDateTimeHeader(SliceView view, Type type)
        {
            var startDate = view.StartDate.Date;
            var endDate = view.EndDate.Date;
            var span = endDate - startDate;
            var dates = new List<DateTime>(span.Days > 0 ? span.Days : 0);

            do
            {
                dates.Add(startDate);
                startDate = startDate.AddDays(1);
            } while (startDate <= endDate);

            return dates;
        }

        private static SliceHeadersFillCallback FillDelegate(Type type)
        {
            while (type != null)
            {
                if (FillCallbacks.ContainsKey(type)) break;
                type = type.BaseType;
            }
            return type != null ? FillCallbacks[type] : null;
        }

        private static SliceViewItemsComparer ItemsComparer(Type type)
        {
            while (type != null)
            {
                if (Comparers.ContainsKey(type)) break;
                type = type.BaseType;
            }
            return type != null ? Comparers[type] : null;
        }

        public static void FillHorizontalHeader(this SliceView view)
        {
            var type = view.HorizontalHeaderType;
            var d = FillDelegate(type);
            if (d != null) view.Header = d(view, type);
        }

        public static void FillVerticalHeader(this SliceView view)
        {
            var type = view.VerticalHeaderType;
            var d = FillDelegate(type);
            if (d != null) view.VerticalHeader = d(view, type);
        }

        public static void Fill(this SliceCellViewModel cell, IEnumerable<Models.Schedule> scheduleItems, IEnumerable<Conflict> conflicts)
        {
            var hComparer = ItemsComparer(cell.HorizontalValue.GetType());
            var vComparer = ItemsComparer(cell.VerticalValue.GetType());

            if (hComparer != null && vComparer != null)
            {
                var items =
                    scheduleItems
                    .Where(item => hComparer(cell.HorizontalValue, item) && vComparer(cell.VerticalValue, item))
                    .Select(item => new ScheduleCardViewModel { Item = item, ScheduleView = cell.ScheduleView, HasConflict = conflicts.Where(c => c.Schedule.CompareTo(item) == 0).Count() > 0 });

                var header = new StringBuilder();
                int cnt = items.Where(m => m.HasConflict).Count();
                if (cnt > 0) header.Append(cnt + " conflict(s)");

                cell.Items = items.Take(SliceCell.VisibleItemsCount);
                cell.ExpanderItems = items.Skip(SliceCell.VisibleItemsCount);
                cell.ExpanderVisibility = cell.ExpanderItems.Count() > 0;
                cell.Header = header.ToString();
            }
        }
    }
}
