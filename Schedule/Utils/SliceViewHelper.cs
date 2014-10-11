using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using Schedule.Controls.Slices;
using Schedule.Models;
using Schedule.Models.DataLayer;
using Schedule.Models.ViewModels;
using Schedule.Models.ViewModels.Slices;

namespace Schedule.Utils
{
    public delegate IEnumerable<object> SliceHeadersFillCallback(Type type);

    public delegate bool SliceViewItemsComparer(object header, Models.Schedule item);

    static class SliceViewHelper
    {
        private static readonly Dictionary<Type, SliceHeadersFillCallback> FillCallbacks =
            new Dictionary<Type, SliceHeadersFillCallback>
            {
                { typeof(DayOfWeek), FillDayHeader },
                { typeof(Entity), FillEntityHeader },
                { typeof(Enum), FillEnumHeader }
            };

        public static readonly Dictionary<Type, SliceViewItemsComparer> Comparers =
            new Dictionary<Type, SliceViewItemsComparer>
            {
                { typeof(DoubleClass), (header, item) => item.DoubleClass == (DoubleClass) header },
                { typeof(CourseType), (header, item) => item.Type == (CourseType) header },
                { typeof(DayOfWeek), (header, item) => item.DaysOfWeek().Contains((DayOfWeek)header) },
                { typeof(Teacher), (header, item) => item.Teacher.CompareTo(header as IComparable) == 0 },
                { typeof(Course), (header, item) => item.Course.CompareTo(header as IComparable) == 0 },
                { typeof(Group), (header, item) => item.Group.CompareTo(header as IComparable) == 0 },
                { typeof(Classroom), (header, item) => item.Class.CompareTo(header as IComparable) == 0 },
            };

        private static IEnumerable<object> FillEnumHeader(Type type)
        {
            var enums = new List<Enum>(type.GetEnumValues().Cast<Enum>());
            return enums;
        }

        private static IEnumerable<object> FillEntityHeader(Type type)
        {
            using (ScheduleDbContext ctx = new ScheduleDbContext())
            {
                ctx.Set(type).Load();
                return ctx.Set(type).Local as IEnumerable<Entity>;
            }
        }

        private static IEnumerable<object> FillDayHeader(Type type)
        {
            List<object> daysOfWeek = new List<object>(7)
            {
                CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek
            };

            for (int i = (int)daysOfWeek[0] + 1; i < (int)daysOfWeek[0] + 7; i++)
                daysOfWeek.Add((DayOfWeek)(i % 7));
            return daysOfWeek;
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
            if (d != null) view.Header = d(type);
        }

        public static void FillVerticalHeader(this SliceView view)
        {
            var type = view.VerticalHeaderType;
            var d = FillDelegate(type);
            if (d != null) view.VerticalHeader = d(type);
        }

        public static void Fill(this SliceCellViewModel cell, IEnumerable<Models.Schedule> scheduleItems)
        {
            var hComparer = ItemsComparer(cell.HorizontalValue.GetType());
            var vComparer = ItemsComparer(cell.VerticalValue.GetType());

            if (hComparer != null && vComparer != null)
            {
                cell.Items =
                    scheduleItems
                        .Where(item => hComparer(cell.HorizontalValue, item) && vComparer(cell.VerticalValue, item))
                        .Select(item => new ScheduleCardViewModel { Item = item, ScheduleView = cell.ScheduleView });
            }
        }
    }
}
