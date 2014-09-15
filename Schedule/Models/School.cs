using System.ComponentModel.DataAnnotations;
using Schedule.Annotations;

namespace Schedule.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class School
    {
        public School()
        {
            this.Buildings = new HashSet<Building>();
            this.Faculties = new HashSet<Faculty>();
        }
    
        public int Id { get; set; }

        [NotNull]
        public string Name { get; set; }
    
        public virtual ICollection<Building> Buildings { get; set; }
        public virtual ICollection<Faculty> Faculties { get; set; }
    }
}
