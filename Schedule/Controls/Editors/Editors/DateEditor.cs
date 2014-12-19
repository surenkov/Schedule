using System;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Media;

namespace Schedule.Controls.Editors.Editors
{
    public class DateEditor : DatePicker, IEditorControl
    {
        public DateEditor()
        {
            Background = Brushes.White;
        }

        public object Value
        {
            get { return SelectedDate; }
            set
            {
                if (value == null) return;

                SelectedDate = (DateTime)value;
            }
        }

        public void Initialize(Type t) { }

        public void SetObjectValue(PropertyInfo info, object obj)
        {
            info.SetValue(obj, Value);
        }
    }
}
