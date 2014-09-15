using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Windows.Data;

namespace Schedule.Controls
{
    public class CalendarHeader : ItemsControl
    {
        static CalendarHeader()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CalendarHeader), new FrameworkPropertyMetadata(typeof(CalendarHeader)));
        }

        public CalendarHeader()
        {
            List<DayOfWeek> daysOfWeek = new List<DayOfWeek>(7)
            {
                CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek
            };

            for (int i = (int)daysOfWeek[0] + 1; i < (int)daysOfWeek[0] + 7; i++)
                daysOfWeek.Add((DayOfWeek)(i % 7));

            ItemsSource = daysOfWeek;
        }
    }

    [ValueConversion(typeof(object), typeof(String))]
    public class EnumToDayNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null ? value.ToString() : String.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
