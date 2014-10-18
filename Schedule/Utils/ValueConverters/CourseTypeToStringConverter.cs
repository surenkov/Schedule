using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using Schedule.Models;

namespace Schedule.Utils.ValueConverters
{
    class CourseTypeToStringConverter : IValueConverter
    {
        private Dictionary<CourseType, string> courses = new Dictionary<CourseType, string>
        {
            { CourseType.Lection, "Лекция" },
            { CourseType.Practice, "Практика" },
            { CourseType.Lab, "Лабораторная" }
        };

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(string) || value.GetType() != typeof(CourseType))
                return string.Empty;

            CourseType type = (CourseType)value;
            if (courses.ContainsKey(type))
                return courses[type];
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
