﻿using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Schedule.Models.ViewModels;
using Schedule.Utils;

namespace Schedule.Controls.Editors.Filters
{
    class EnumFilter : ComboBox, IFilterControl
    {
        public IEnumerable<FilterComparerViewModel> Comparers()
        {
            return new List<FilterComparerViewModel>
            {
                new FilterComparerViewModel { Sign = "=", Comparer = (i1, i2) => (int)i1 == (int)i2 },
                new FilterComparerViewModel { Sign = "<>", Comparer = (i1, i2) => (int)i1 != (int)i2 }
            };
        }

        public void SetSourceType(Type t)
        {
            var mapper = new EditorsMapper();
            mapper.FillData(this, t);
        }

        public object Value()
        {
            return SelectedItem;
        }

        public DependencyProperty ValueProperty()
        {
            return SelectedItemProperty;
        }
    }
}