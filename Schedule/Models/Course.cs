using Schedule.Utils.Attributes;
    using System.Collections.Generic;

namespace Schedule.Models
{
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
        [Hidden]
        public virtual ICollection<Teacher> Teachers { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
