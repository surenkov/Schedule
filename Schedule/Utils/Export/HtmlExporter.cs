using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using Schedule.Models.DataLayer;

namespace Schedule.Utils.Export
{
    class HtmlExporter : IExporter
    {
        public void Save(string path)
        {
            StringBuilder htmlData = new StringBuilder();
            using (var ctx = new ScheduleDbContext())
            {
                ctx.Set<Models.Schedule>().Load();
                var items = ctx.Set<Models.Schedule>().Local;
            }

            using (var stream = new StreamWriter(path))
            {

            }
        }
    }
}
