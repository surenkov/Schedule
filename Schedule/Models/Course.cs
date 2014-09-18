using Schedule.Attributes;

namespace Schedule.Models
{
    using System;
    using System.Collections.Generic;


    public enum CourseType
    {
        Lection,
        Practice,
        Lab
    }
    
    public partial class Course : Entity
    {
        public Course()
        {
            this.Teachers = new HashSet<Teacher>();
        }
    
        public string Name { get; set; }
        public virtual ICollection<Teacher> Teachers { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
