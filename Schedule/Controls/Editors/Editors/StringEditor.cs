using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace Schedule.Controls.Editors.Editors
{
    public class StringEditor : TextBox, IEditorControl
    {
        public StringEditor()
        {
            VerticalContentAlignment = VerticalAlignment.Center;
        }

        public object Value
        {
            get { return Text; }
            set
            {
                if (value == null) return;

                Text = (string)value;
            }
        }

        public void Initialize(Type t) { }

        public void SetObjectValue(PropertyInfo info, object obj)
        {
            info.SetValue(obj, Value);
        }
    }
}
