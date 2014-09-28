using System;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Windows.Data;

namespace Schedule.Utils.ValueConverters
{
    class PropertyToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var property = value as PropertyInfo;
            string result = null;

            if (targetType == typeof(string))
            {
                var descr = property.GetCustomAttribute(typeof(DescriptionAttribute)) as DescriptionAttribute;
                if (property != null)
                    result = descr != null ? descr.Description : property.Name;
            }
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
