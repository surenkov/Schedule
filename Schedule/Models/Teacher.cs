using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Schedule.Annotations;
using Schedule.Utils.Attributes;

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


        [Hidden]
        public int FacultyId { get; set; }
        
        [NotNull, ForeignKey("FacultyId")]
        public virtual Faculty Faculty { get; set; }


        [Hidden]
        public virtual ICollection<Course> Courses { get; set; }


        public override string ToString()
        {
            return Name;
        }
    }
}
