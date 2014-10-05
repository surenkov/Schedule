using System.Data.Entity;

namespace Schedule.Models.DataLayer
{
    class ScheduleDbContext : DbContext
    {
        public ScheduleDbContext()
            : base("ScheduleDBConnection")
        {
            //Database.SetInitializer(new ScheduleInitializer());
        }

        public DbSet<Building> Buildings { get; set; }
        public DbSet<Classroom> Classrooms { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Faculty> Faculties { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Schedule> Schedule { get; set; }
        public DbSet<School> Schools { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
    }
}
