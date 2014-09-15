using Schedule.Annotations;

namespace Schedule.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Faculty
    {
        public Faculty()
        {
            this.Teachers = new HashSet<Teacher>();
            this.Groups = new HashSet<Group>();
        }
    
        public int Id { get; set; }

        [NotNull]
        public string Name { get; set; }
    
        [NotNull]
        public virtual School School { get; set; }
        public virtual ICollection<Teacher> Teachers { get; set; }
        public virtual ICollection<Group> Groups { get; set; }
    }
}
