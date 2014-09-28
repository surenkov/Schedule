using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using Schedule.Controls.Editors;

namespace Schedule.Utils.ValueConverters
{
    class StringToComparerConverter : IValueConverter
    {
        readonly Dictionary<string, CheckPropertyValueDelegate> _comparers = new Dictionary<string, CheckPropertyValueDelegate>()
        {
            {"=", (value, compared) => value.CompareTo(compared) == 0},
            {"<>", (value, compared) => value.CompareTo(compared) != 0},
            {"<", (value, compared) => value.CompareTo(compared) < 0},
            {">", (value, compared) => value.CompareTo(compared) > 0},
            {"<=", (value, compared) => value.CompareTo(compared) <= 0},
            {">=", (value, compared) => value.CompareTo(compared) >= 0}
        };

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            object res = null;
            if (targetType == typeof(CheckPropertyValueDelegate))
                if (_comparers.ContainsKey((string) value))
                    res = _comparers[(string) value];
            return res;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
