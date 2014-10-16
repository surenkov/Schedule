using System.Collections.Generic;
using Schedule.Utils.Conflicts.Checkers;
using Schedule.Models.ViewModels;

namespace Schedule.Utils.Conflicts
{
    class ConflictsManager
    {
        private List<IConflictsChecker> checkers = new List<IConflictsChecker>
        {
                new TwoTeachersInClassroom(),
                new GroupInDifferentClassrooms(),
                new TeacherInDifferentClassrooms(),
                new DifferentCourseTypesInClassroom(),
                new SmallClassroom(),
                new DifferentCoursesInClassroom()
        };

        public IEnumerable<ConflictsViewModel> TreeModels(IEnumerable<Models.Schedule> items)
        {
            List<ConflictsViewModel> models = new List<ConflictsViewModel>();

            foreach (var checker in checkers)
                models.Add(checker.TreeModel(items));

            return models;
        }

        public IEnumerable<Conflict> CheckAll(IEnumerable<Models.Schedule> items)
        {
            List<Conflict> conflicts = new List<Conflict>();

            foreach (var checker in checkers)
                conflicts.AddRange(checker.Check(items));

            return conflicts;
        }
    }
}
