using System.ComponentModel.DataAnnotations;
using Schedule.Annotations;
using Schedule.Attributes;

namespace Schedule.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Teacher : Entity
    {
        public Teacher()
        {
            this.Courses = new HashSet<Course>();
        }
    
        [NotNull, MaxLength(300)]
        public string Name { get; set; }
        
        [NotNull]
        public virtual Faculty Faculty { get; set; }
        public virtual ICollection<Course> Courses { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
