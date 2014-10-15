using System.Linq;
using System.Reflection;
using System.ComponentModel;
using System.Collections.Generic;
using Schedule.Models.ViewModels;

namespace Schedule.Utils.Conflicts.Checkers
{
    [Description("Too small classroom")]
    class SmallClassroom : IConflictsChecker
    {
        public IEnumerable<Conflict> Check(IEnumerable<Models.Schedule> items)
        {
            return items.Where(i => i.Group.Students.Count() > i.Class.Capacity).Select(s => new Conflict { Schedule = s });
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
                    conflict.Schedule.Class + " (" +
                    conflict.Schedule.Class.Capacity + " seats) is too small for " +
                    conflict.Schedule.Group + " (" +
                    conflict.Schedule.Group.Students.Count() + " men)";
                child.Item = conflict;

                model.Children.Add(child);
            }

            return model;
        }
    }
}
