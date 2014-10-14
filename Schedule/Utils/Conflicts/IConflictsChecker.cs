using System.Collections.Generic;
using Schedule.Models.ViewModels;

namespace Schedule.Utils.Conflicts
{
    public interface IConflictsChecker
    {
        IEnumerable<Conflict> Check(IEnumerable<Models.Schedule> items);
        ConflictsViewModel TreeModel(IEnumerable<Models.Schedule> items);
    }
}
