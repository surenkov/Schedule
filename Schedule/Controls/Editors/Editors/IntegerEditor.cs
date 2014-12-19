using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace Schedule.Controls.Editors.Editors
{
    public class IntegerEditor : TextBox, IEditorControl
    {
        public IntegerEditor()
        {
            VerticalContentAlignment = VerticalAlignment.Center;
        }

        public object Value
        {
            get
            {
                int i = 0;
                int.TryParse(Text, out i);
                return i;
            }

            set
            {
                if (value == null) return;

                Text = ((int)value).ToString();
            }
        }

        public void Initialize(Type t) { }

        public void SetObjectValue(PropertyInfo info, object obj)
        {
            info.SetValue(obj, Value);
        }
    }
}
