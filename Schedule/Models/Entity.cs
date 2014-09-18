using System.ComponentModel.DataAnnotations;
using Schedule.Attributes;

namespace Schedule.Models
{
    public abstract partial class Entity
    {
        [Key, NotShown]
        public int Id { get; set; }
    }
}
