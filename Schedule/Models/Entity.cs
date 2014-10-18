using System;
using System.ComponentModel.DataAnnotations;
using System.Data;
using Schedule.Utils.Attributes;

namespace Schedule.Models
{
    public abstract partial class Entity : IComparable<Entity>
    {
        [Key, Hidden]
        public int Id { get; set; }

        public int CompareTo(Entity obj)
        {
            return Id.CompareTo(obj.Id);
        }
    }
}
