using System.Windows;
using Schedule.Models.DataLayer;

namespace Schedule
{
    public partial class App : Application
    {
        public App()
        {
            using (ScheduleDbContext ctx = new ScheduleDbContext())
            {
                ctx.Database.Connection.Open();
            }
        }
    }
}