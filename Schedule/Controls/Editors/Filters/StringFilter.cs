using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Schedule.Models.ViewModels;

namespace Schedule.Controls.Editors.Filters
{
    class StringFilter : TextBox, IFilterControl
    {
        public StringFilter()
        {
            VerticalContentAlignment = VerticalAlignment.Center;
        }

        public IEnumerable<FilterComparerViewModel> Comparers()
        {
            return new List<FilterComparerViewModel>
            {
                new FilterComparerViewModel { Sign = "=", Comparer = (i1, i2) => ((string)i1).CompareTo((string)i2) == 0 },
                new FilterComparerViewModel { Sign = "<>", Comparer = (i1, i2) => ((string)i1).CompareTo((string)i2) != 0 },
                new FilterComparerViewModel { Sign = ">", Comparer = (i1, i2) => ((string)i1).CompareTo((string)i2) > 0},
                new FilterComparerViewModel { Sign = "<", Comparer = (i1, i2) => ((string)i1).CompareTo((string)i2) < 0 },
                new FilterComparerViewModel { Sign = ">=", Comparer = (i1, i2) => ((string)i1).CompareTo((string)i2) >= 0 },
                new FilterComparerViewModel { Sign = "<=", Comparer = (i1, i2) => ((string)i1).CompareTo((string)i2) <= 0 }
            };
        }

        public object Value
        {
            get { return Text; }
        }

        public DependencyProperty ValueProperty
        {
            get { return TextProperty; }
        }

        public void SetSourceType(Type t) { }
    }
}
