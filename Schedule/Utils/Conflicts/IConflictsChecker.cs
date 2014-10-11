using System.Collections.Generic;

namespace Schedule.Utils.Conflicts
{
    public interface IConflictsChecker
    {
        IEnumerable<Conflict> Check();
    }
}
