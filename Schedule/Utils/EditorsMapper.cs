﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Schedule.Controls.Editors;
using Schedule.Models;
using Schedule.Models.DataLayer;

namespace Schedule.Utils
{
    internal delegate void ControlFillDelegate(Control ctrl, Type dataType);

    class EditorsMapper
    {
        private static readonly Dictionary<Type, ControlFillDelegate> Delegates =
            new Dictionary<Type, ControlFillDelegate>
        {
            { typeof(Enum), FillComboBoxWithEnums },
            { typeof(Classroom), FillCalssroomSelector},
            { typeof(Entity), FillEntitiesSelector }
        };

        private static async void FillCalssroomSelector(Control c, Type t)
        {
            var selector = c as EntitySelector;
            if (selector == null) return;

            selector.ItemsType = t;
            using (ScheduleDbContext ctx = new ScheduleDbContext())
            {
                ctx.Set(t).Include("Building").Load();
                selector.ItemsSource = ctx.Set(t).Local;
            }
        }

        private static async void FillEntitiesSelector(Control c, Type t)
        {
            var selector = c as EntitySelector;
            if (selector == null) return;

            selector.ItemsType = t;
            using (ScheduleDbContext ctx = new ScheduleDbContext())
            {
                ctx.Set(t).Load();
                selector.ItemsSource = ctx.Set(t).Local;
            }
        }

        public void FillData(Control control, Type dataType)
        {
            var d = Delegate(dataType);
            if (d != null)
                d(control, dataType);
        }

        private ControlFillDelegate Delegate(Type type)
        {
            while (type != null)
            {
                if (Delegates.ContainsKey(type)) break;
                type = type.BaseType;
            }
            return type != null ? Delegates[type] : null;
        }

        private static void FillComboBoxWithEnums(Control c, Type t)
        {
            var check = c as ComboBox;
            if (check != null)
                check.ItemsSource = t.GetEnumValues();
        }
    }
}
