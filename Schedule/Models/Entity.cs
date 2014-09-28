using System;
using System.ComponentModel.DataAnnotations;
using System.Data;
using Schedule.Attributes;

namespace Schedule.Models
{
    public abstract partial class Entity : IComparable
    {
        [Key, NotShown]
        public int Id { get; set; }

        public int CompareTo(object obj)
        {
            var e = (Entity) obj;
            if (this.GetType() != obj.GetType()) throw new StrongTypingException();
            
            if (Id == e.Id) return 0;
            if (Id > e.Id) return 1;
            return -1;
        }
    }
}
