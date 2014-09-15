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
    
    public partial class Classroom
    {
        public int Id { get; set; }
        public int Number { get; set; }
        public string Sign { get; set; }
        public ClassroomType Type { get; set; }
        public uint Capacity { get; set; }
    
        public virtual Building Building { get; set; }
        public virtual ICollection<Schedule> Schedules { get; set; } 
    }
}
