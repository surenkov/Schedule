using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using Schedule.Annotations;

namespace Schedule.Models
{
    using System;
    using System.Collections.Generic;

    public enum DoubleClass
    {
        First,
        Second,
        Third,
        Fourth,
        Fifth,
        Sixth,
        Seventh,
        Eighth
    }

    [ValueConversion(typeof(DoubleClass), typeof(String))]
    public class DoubleClassToStringConverter : IValueConverter
    {
        private static readonly Dictionary<DoubleClass, String> Dict = new Dictionary<DoubleClass, string>
        {
            {DoubleClass.First, "1st Period"},
            {DoubleClass.Second, "2nd Period"},
            {DoubleClass.Third, "3rd Period"},
            {DoubleClass.Fourth, "4th Period"},
            {DoubleClass.Fifth, "5th Period"},
            {DoubleClass.Sixth, "6th Period"},
            {DoubleClass.Seventh, "7th Period"},
            {DoubleClass.Eighth, "8th Period"}
        };

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Dict[(DoubleClass)value];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public partial class Schedule
    {
        [Key]
        public int Id { get; set; }

        public System.DateTime StartDate { get; set; }
        public System.DateTime EndDate { get; set; }
        public int Interval { get; set; }
        public DoubleClass DoubleClass { get; set; }

        public virtual Teacher Teacher { get; set; }
        public virtual Course Course { get; set; }
        public virtual CourseType Type { get; set; }
        public virtual Group Group { get; set; }
    }
}
