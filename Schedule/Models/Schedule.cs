using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Windows.Data;
using Schedule.Annotations;
using Schedule.Properties;

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

    public partial class Schedule : Entity
    {
        [Description("Start date"), Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Schedule_StartDate_ErrMsg")]
        public System.DateTime StartDate { get; set; }

        [Description("End date"), Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Schedule_EndDate_ErrMsg")]
        public System.DateTime EndDate { get; set; }

        [Required, Description("Interval")]
        public int Interval { get; set; }

        [Description("Number of period")]
        public DoubleClass DoubleClass { get; set; }

        [NotNull, Required]
        public virtual Teacher Teacher { get; set; }
        [NotNull, Required]
        public virtual Course Course { get; set; }

        [Description("Type of course")]
        public virtual CourseType Type { get; set; }
        [NotNull, Required]
        public virtual Group Group { get; set; }
        [NotNull, Required]
        public virtual Classroom Class { get; set; }

        public ISet<DayOfWeek> DaysOfWeek()
        {
            var days = new HashSet<DayOfWeek>();
            var date = StartDate;
            do
            {
                days.Add(date.DayOfWeek);
                date = date.AddDays(Interval);
            } while (!days.Contains(date.DayOfWeek) && date <= EndDate);
            return days;
        }
    }
}
