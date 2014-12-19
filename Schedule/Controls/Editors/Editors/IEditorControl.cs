using System;
using System.Reflection;

namespace Schedule.Controls.Editors.Editors
{
    public interface IEditorControl
    {
        object Value { get; set; }
        void Initialize(Type t);
        void SetObjectValue(PropertyInfo info, object obj);
    }
}
