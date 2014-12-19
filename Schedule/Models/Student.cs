using System.ComponentModel.DataAnnotations;
using Schedule.Annotations;
using Schedule.Utils.Attributes;

namespace Schedule.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class Student : Entity
    {
        [NotNull, MaxLength(300)]
        public string Name { get; set; }
        public string Address { get; set; }
        

        [Hidden]
        public int GroupId { get; set; }

        [NotNull, ForeignKey("GroupId")]
        public virtual Group Group { get; set; }


        public override string ToString()
        {
            return Name;
        }
    }
}
