using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Windows.Data;
using Schedule.Annotations;
using Schedule.Properties;
using Schedule.Utils.Attributes;
using System.ComponentModel.DataAnnotations.Schema;

namespace Schedule.Models
{

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

    [ValueConversion(typeof(DoubleClass), typeof(string))]
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

        [Required, Positive(ErrorMessage = "Value must be a positive number"), Description("Interval")]
        public int Interval { get; set; }

        [Description("Number of period")]
        public DoubleClass DoubleClass { get; set; }

        [Description("Type of course")]
        public virtual CourseType Type { get; set; }


        [Hidden]
        public int TeacherId { get; set; }

        [NotNull, Required, ForeignKey("TeacherId")]
        public virtual Teacher Teacher { get; set; }


        [Hidden]
        public int CourseId { get; set; }

        [NotNull, Required, ForeignKey("CourseId")]
        public virtual Course Course { get; set; }

        
        [Hidden]
        public int GroupId { get; set; }

        [NotNull, Required, ForeignKey("GroupId")]
        public virtual Group Group { get; set; }


        [Hidden]
        public int ClassId { get; set; }

        [NotNull, Required, ForeignKey("ClassId")]
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
