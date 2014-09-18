using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Forms;
using Schedule.Controls.Editors;
using Schedule.Models;
using Button = System.Windows.Controls.Button;
using ComboBox = System.Windows.Controls.ComboBox;
using Control = System.Windows.Controls.Control;
using ListBox = System.Windows.Controls.ListBox;
using TextBox = System.Windows.Controls.TextBox;


namespace Schedule.Utils
{
    public sealed class EditorsFactory
    {
        private static readonly Dictionary<Type, Type> ControlsTypes = new Dictionary<Type, Type>
        {
            { typeof(string), typeof(TextBox) },
            { typeof(short), typeof(TextBox) },
            { typeof(int), typeof(TextBox) },
            { typeof(float), typeof(TextBox) },
            { typeof(double), typeof(TextBox) },
            { typeof(IEnumerable), typeof(ListBox) },
            { typeof(Enum), typeof(ComboBox) },
            { typeof(DateTime), typeof(DatePicker) },
            { typeof(Entity), typeof(EntitySelector) }
        };

        private static readonly Dictionary<Type, DependencyProperty> BindingProperties =
            new Dictionary<Type, DependencyProperty>
        {
            { typeof(TextBox), TextBox.TextProperty },
            { typeof(ComboBox), Selector.SelectedItemProperty },
            { typeof(EntitySelector), Selector.SelectedItemProperty },
            { typeof(DatePicker), DatePicker.SelectedDateProperty },
            { typeof(Button), ContentControl.ContentProperty }
        };

        public Control CreateControl(Type type)
        {
            var t = ControlType(type);
            if (t == null) return new Control();
            return Activator.CreateInstance(t) as Control;
        }

        public Type ControlType(Type type)
        {
            while (type != null)
            {
                if (ControlsTypes.ContainsKey(type)) break;
                type = type.BaseType;
            }
            return type != null ? ControlsTypes[type] : null;
        }

        public DependencyProperty BindingProperty(Type controlType)
        {
            if (controlType == null || !BindingProperties.ContainsKey(controlType))
                return null;
            return BindingProperties[controlType];
        }
    }

}
