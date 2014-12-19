using System.Collections.Generic;
using System.Globalization;
using System.ComponentModel.DataAnnotations.Schema;
using Schedule.Utils.Attributes;
using Schedule.Annotations;

namespace Schedule.Models
{
    public enum ClassroomType
    {
        LectionClass,
        PracticeClass,
        ComputerClass,
        Gym
    }
    
    public partial class Classroom : Entity
    {
        public int Number { get; set; }

        public string Sign { get; set; }

        public ClassroomType Type { get; set; }

        public int Capacity { get; set; }


        [Hidden]
        public int BuildingId { get; set; }
    
        [NotNull, ForeignKey("BuildingId")]
        public virtual Building Building { get; set; }


        [Hidden]
        public virtual ICollection<Schedule> Schedules { get; set; }

        public override string ToString()
        {
            return Building == null ? Number.ToString(CultureInfo.CurrentCulture) : Building.Name + Number;
        }
    }
}
