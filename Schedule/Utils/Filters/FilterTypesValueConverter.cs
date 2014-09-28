using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;

namespace Schedule.Utils.Filters
{
    static class FilterTypesValueConverter
    {
        private delegate object TypesCoversion(object obj);

        private static readonly Dictionary<Type, Dictionary<Type, TypesCoversion>> Conversions =
            new Dictionary<Type, Dictionary<Type, TypesCoversion>>
            {
                {
                    typeof(string),
                    new Dictionary<Type, TypesCoversion>
                    {
                        {typeof(int), delegate(object o)
                        {
                            int i;
                            int.TryParse((string) o, out i);
                            return i;
                        }}
                    }
                }
            };

        public static object FilterConvert(this object obj, Type targetType)
        {
            Dictionary<Type, TypesCoversion> convs;
            if (!Conversions.TryGetValue(obj.GetType(), out convs)) return null;
            return convs.ContainsKey(targetType) ? convs[targetType](obj) : null;
        }
    }
}
