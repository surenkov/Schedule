using System.ComponentModel.DataAnnotations.Schema;
using Schedule.Annotations;
using Schedule.Attributes;

namespace Schedule.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Group : Entity
    {
        public Group()
        {
            this.Students = new HashSet<Student>();
            this.Schedule = new HashSet<Schedule>();
        }

        [NotNull]
        public string Name { get; set; }
    
        [NotNull]
        public virtual Faculty Faculty { get; set; }
        public virtual ICollection<Student> Students { get; set; }
        public virtual ICollection<Schedule> Schedule { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
