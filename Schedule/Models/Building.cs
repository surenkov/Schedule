using Schedule.Annotations;

namespace Schedule.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Building
    {
        public Building()
        {
            this.Schools = new HashSet<School>();
            this.Classrooms = new HashSet<Classroom>();
        }
    
        public int Id { get; set; }

        [NotNull]
        public string Name { get; set; }
    
        public virtual ICollection<School> Schools { get; set; }
        public virtual ICollection<Classroom> Classrooms { get; set; }
    }
}
