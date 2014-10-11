using Schedule.Models.DataLayer;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Schedule.Utils.Conflicts.Checkers
{
    [Description("One group in different classrooms at a time")]
    class GroupInDifferentClassroomsChecker : IConflictsChecker
    {
        public IEnumerable<Conflict> Check()
        {
            throw new NotImplementedException();
        }
    }
}
