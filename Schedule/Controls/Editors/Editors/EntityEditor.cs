using System.Linq;
using System.Windows.Controls;
using Schedule.Utils.Editors;
using Schedule.Models;
using System.Reflection;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Schedule.Controls.Editors.Editors
{
    public class EntityEditor : ComboBox, IEditorControl
    {
        private EditorsMapper mapper;

        public EntityEditor()
        {
            mapper = new EditorsMapper();
        }

        public object Value
        {
            get { return SelectedValue; }

            set
            {
                if (value == null) return;

                SelectedItem = ItemsSource.
                                    OfType<Entity>().
                                    FirstOrDefault(e => e.Id == ((Entity)value).Id);
            }
        }

        public void Initialize(Type t)
        {
            mapper.FillData(this, t);
        }

        public void SetObjectValue(PropertyInfo info, object obj)
        {
            info.SetValue(obj, Value);
            if (Value == null)
                throw new Exception("\"" + info.Name + "\" field cannot be empty");
            obj.GetType()
                .GetProperty(info.GetCustomAttribute<ForeignKeyAttribute>().Name).SetValue(obj, ((Entity)Value).Id);
        }
    }
}
