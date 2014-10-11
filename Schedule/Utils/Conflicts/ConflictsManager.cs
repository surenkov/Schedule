using System.Collections.Generic;

namespace Schedule.Utils.Conflicts
{
    public class ConflictsManager
    {
        public IEnumerable<IConflictsChecker> Conflicts { get; set; }

        public ConflictsManager()
        {

        }
    }
}
