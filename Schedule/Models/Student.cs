using System.ComponentModel.DataAnnotations;
using Schedule.Annotations;

namespace Schedule.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Student
    {
        public int Id { get; set; }

        [NotNull, MaxLength(300)]
        public string Name { get; set; }
        public string Address { get; set; }
        
        [NotNull]
        public virtual Group Group { get; set; }
    }
}
