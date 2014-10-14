using System;
using System.Linq;
using System.Reflection;
using System.ComponentModel;
using System.Collections.Generic;
using Schedule.Models.ViewModels;

namespace Schedule.Utils.Conflicts.Checkers
{
    [Description("Two teachers in one classroom at the same time")]
    class TwoTeachersInClassroom : IConflictsChecker
    {
        public IEnumerable<Conflict> Check(IEnumerable<Models.Schedule> items)
        {
            var res = new HashSet<Models.Schedule>();
            var days = Enum.GetValues(typeof(DayOfWeek)).Cast<DayOfWeek>();

            if (items != null)
            {
                foreach (var day in days)
                {
                    var q = items.Where(s => s.DaysOfWeek().Contains(day));
                    var r = from s1 in q
                            from s2 in q
                            where s1.Class == s2.Class
                            where s1.Teacher != s2.Teacher
                            where s1.DoubleClass == s2.DoubleClass
                            select s1;
                    res.UnionWith(r);
                }
            }
            return res.Select(s => new Conflict { Schedule = s });
        }

        public ConflictsViewModel TreeModel(IEnumerable<Models.Schedule> items)
        {
            var model = new ConflictsViewModel();
            var descr = GetType().GetCustomAttribute(typeof(DescriptionAttribute)) as DescriptionAttribute;
            var conflicts = Check(items);

            if (descr != null)
                model.Description = descr.Description + ": " + conflicts.Count();

            foreach (var conflict in conflicts)
            {
                var child = new ConflictsViewModel();
                child.Description = 
                    conflict.Schedule.Teacher + ", " + 
                    conflict.Schedule.Class + ", " + 
                    conflict.Schedule.DoubleClass;
                child.Item = conflict;

                model.Children.Add(child);
            }

            return model;
        }
    }
}
