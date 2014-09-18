using Schedule.Attributes;

namespace Schedule.Models
{
    using System;
    using System.Collections.Generic;

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
    
        public virtual Building Building { get; set; }
        public virtual ICollection<Schedule> Schedules { get; set; }

        public override string ToString()
        {
            return Building.Name + Number;
        }
    }
}
