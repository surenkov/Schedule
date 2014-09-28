using Schedule.Annotations;
using Schedule.Attributes;

namespace Schedule.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Faculty : Entity
    {
        public Faculty()
        {
            this.Teachers = new HashSet<Teacher>();
            this.Groups = new HashSet<Group>();
        }
    
        [NotNull]
        public string Name { get; set; }
        [NotShown]
        public int SchoolId { get; set; }
    
        [NotNull]
        public virtual School School { get; set; }
        public virtual ICollection<Teacher> Teachers { get; set; }
        public virtual ICollection<Group> Groups { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
