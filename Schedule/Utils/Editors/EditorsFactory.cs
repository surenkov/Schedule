using System;
using System.Collections.Generic;
using System.Windows.Controls;
using Schedule.Controls.Editors.Editors;
using Schedule.Models;

namespace Schedule.Utils.Editors
{
    public sealed class EditorsFactory
    {
        private static Dictionary<Type, Type> editors = new Dictionary<Type, Type>();

        static EditorsFactory()
        {
            RegisterEditor<int, IntegerEditor>();
            RegisterEditor<string, StringEditor>();
            RegisterEditor<DateTime, DateEditor>();
            RegisterEditor<Enum, EnumEditor>();
            RegisterEditor<Entity, EntityEditor>();
        }

        public static void RegisterEditor<TType, TControl>()
        {
            editors.Add(typeof(TType), typeof(TControl));
        }

        public Control CreateEditor(Type t)
        {
            while (t != null)
            {
                if (editors.ContainsKey(t))
                    return Activator.CreateInstance(editors[t]) as Control;
                t = t.BaseType;
            }
            return null;
        }
    }
}
