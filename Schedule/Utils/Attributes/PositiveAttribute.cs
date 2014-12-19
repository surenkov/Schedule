using System;
using System.ComponentModel.DataAnnotations;

namespace Schedule.Utils.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    class PositiveAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            return (int)value > 0;
        }
    }
}
