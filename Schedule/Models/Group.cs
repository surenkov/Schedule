using Schedule.Annotations;

namespace Schedule.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Group
    {
        public Group()
        {
            this.Students = new HashSet<Student>();
            this.Schedule = new HashSet<Schedule>();
        }
    
        public int Id { get; set; }

        [NotNull]
        public string Name { get; set; }
    
        [NotNull]
        public virtual Faculty Faculty { get; set; }
        public virtual ICollection<Student> Students { get; set; }
        public virtual ICollection<Schedule> Schedule { get; set; }
    }
}
