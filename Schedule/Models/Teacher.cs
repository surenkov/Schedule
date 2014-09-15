using System.ComponentModel.DataAnnotations;
using Schedule.Annotations;

namespace Schedule.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Teacher
    {
        public Teacher()
        {
            this.Courses = new HashSet<Course>();
        }
    
        public int Id { get; set; }

        [NotNull, MaxLength(300)]
        public string Name { get; set; }
        
        [NotNull]
        public virtual Faculty Faculty { get; set; }
        public virtual ICollection<Course> Courses { get; set; }
    }
}
