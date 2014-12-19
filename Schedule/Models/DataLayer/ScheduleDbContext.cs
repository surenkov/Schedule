using System.Data.Entity;

namespace Schedule.Models.DataLayer
{
    class ScheduleDbContext : DbContext
    {
        public ScheduleDbContext() : 
            base("name=ScheduleDBConnectionString") { }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Group>().
                HasRequired(g => g.Faculty).
                WithMany().
                WillCascadeOnDelete(false);

            modelBuilder.Entity<Teacher>().
                HasRequired(t => t.Faculty).
                WithMany().
                WillCascadeOnDelete(false);
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
