using System.Collections.Generic;
using System.Linq;
using Schedule.Controls.Editors;
using Schedule.Models;

namespace Schedule.Utils.Filters
{
    static class FiltersExtender
    {
        public static IEnumerable<Entity> ApplyFilters(this IEnumerable<Entity> entities, IEnumerable<Filter> filters)
        {
            if (filters != null)
                return filters.Aggregate(entities, (e, f) => f.FilterEntities(e));
            return entities;
        }
    }
}
