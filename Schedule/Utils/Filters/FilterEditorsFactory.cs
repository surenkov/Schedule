using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Schedule.Models;

namespace Schedule.Utils.Filters
{
    class FilterEditorsFactory
    {
        private static readonly Dictionary<Type, Type> ControlTypes = new Dictionary<Type, Type>
        {
            { typeof(object), typeof(TextBox) },
            { typeof(Enum), typeof(ComboBox) },
            { typeof(Entity), typeof(ComboBox) },
            { typeof(DateTime), typeof(DatePicker) }
        };

        private static readonly Dictionary<Type, DependencyProperty> ValuesDictionary =
            new Dictionary<Type, DependencyProperty>
        {
            { typeof(TextBox), TextBox.TextProperty },
            { typeof(Selector), Selector.SelectedItemProperty },
            { typeof(DatePicker), DatePicker.SelectedDateProperty }
        };

        public DependencyProperty ValueProperty(Control control)
        {
            var t = control.GetType();
            while (t != null)
            {
                if (ValuesDictionary.ContainsKey(t)) break;
                t = t.BaseType;
            }
            return t != null ? ValuesDictionary[t] : null;
        }

        public Control CreateControl(Type type)
        {
            while (type != null)
            {
                if (ControlTypes.ContainsKey(type)) break;
                type = type.BaseType;
            }

            var t = type != null ? ControlTypes[type] : null;
            if (t == null) return new Control();
            return Activator.CreateInstance(t) as Control;
        }
    }
}
