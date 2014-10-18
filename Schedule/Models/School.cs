using System.ComponentModel.DataAnnotations;
using Schedule.Annotations;
using Schedule.Utils.Attributes;

namespace Schedule.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class School : Entity
    {
        public School()
        {
            this.Buildings = new HashSet<Building>();
            this.Faculties = new HashSet<Faculty>();
        }
    
        [NotNull]
        public string Name { get; set; }
    
        [Hidden]
        public virtual ICollection<Building> Buildings { get; set; }
        [Hidden]
        public virtual ICollection<Faculty> Faculties { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
