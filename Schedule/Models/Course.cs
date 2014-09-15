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
    
    public partial class Course
    {
        public Course()
        {
            this.Teachers = new HashSet<Teacher>();
        }
    
        public int Id { get; set; }
        public string Name { get; set; }
    
        public virtual ICollection<Teacher> Teachers { get; set; }
    }
}
