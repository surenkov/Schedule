using Schedule.Models.DataLayer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Schedule.Utils.Conflicts.Checkers
{
    [Description("Two teachers in one classroom at the same time")]
    class TwoTeachersInClassroomChecker : IConflictsChecker
    {
        public IEnumerable<Conflict> Check()
        {
            using (ScheduleDbContext ctx = new ScheduleDbContext())
            {
                var res = new HashSet<Models.Schedule>();
                var days = Enum.GetValues(typeof(DayOfWeek)).Cast<DayOfWeek>();
                foreach (var day in days)
                {
                    var q = ctx.Schedule.Where(s => s.DaysOfWeek().Contains(day));
                    var r = from s1 in q
                            from s2 in q
                            where s1.DoubleClass == s2.DoubleClass
                            where s1.Teacher != s2.Teacher
                            select s1;
                    res.UnionWith(r);
                }
                return res.Select(s => new Conflict { Schedule = s });
            }
        }
    }
}
