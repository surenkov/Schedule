using Schedule.Annotations;
using Schedule.Utils.Attributes;


namespace Schedule.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Building : Entity
    {
        public Building()
        {
            this.Schools = new HashSet<School>();
            this.Classrooms = new HashSet<Classroom>();
        }

        [NotNull]
        public string Name { get; set; }

        [Hidden]
        public virtual ICollection<School> Schools { get; set; }
        [Hidden]
        public virtual ICollection<Classroom> Classrooms { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
