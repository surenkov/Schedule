using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using Schedule.Annotations;
using Schedule.Utils.Attributes;

namespace Schedule.Models
{

    public partial class Group : Entity
    {
        public Group()
        {
            Students = new HashSet<Student>();
            Schedule = new HashSet<Schedule>();
        }

        [NotNull]
        public string Name { get; set; }


        [Hidden]
        public int FacultyId { get; set; }

        [NotNull, ForeignKey("FacultyId")]
        public virtual Faculty Faculty { get; set; }


        [Hidden]
        public virtual ICollection<Student> Students { get; set; }

        [Hidden]
        public virtual ICollection<Schedule> Schedule { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
