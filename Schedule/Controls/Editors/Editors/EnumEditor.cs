using System;
using System.Linq;
using System.Reflection;
using System.Windows.Controls;
using Schedule.Utils.Editors;

namespace Schedule.Controls.Editors.Editors
{
    public class EnumEditor : ComboBox, IEditorControl
    {
        private EditorsMapper mapper;

        public EnumEditor()
        {
            mapper = new EditorsMapper();
        }
        public object Value
        {
            get { return SelectedItem as Enum; }
            set
            {
                if (value == null) return;

                SelectedItem = ItemsSource.
                                    OfType<Enum>().
                                    FirstOrDefault(e => e.CompareTo(value) == 0);
            }
        }

        public void Initialize(Type t)
        {
            mapper.FillData(this, t);
        }

        public void SetObjectValue(PropertyInfo info, object obj)
        {
            info.SetValue(obj, Value);
        }
    }
}
