using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Schedule.Models.ViewModels;

namespace Schedule.Controls.Editors.Filters
{
    class IntegerFilter : TextBox, IFilterControl
    {
        public IEnumerable<FilterComparerViewModel> Comparers()
        {
            return new List<FilterComparerViewModel>
            {
                new FilterComparerViewModel { Sign = "=", Comparer = (i1, i2) => (int)i1 == (int)i2 },
                new FilterComparerViewModel { Sign = "<>", Comparer = (i1, i2) => (int)i1 != (int)i2 },
                new FilterComparerViewModel { Sign = ">", Comparer = (i1, i2) => (int)i1 > (int)i2 },
                new FilterComparerViewModel { Sign = "<", Comparer = (i1, i2) => (int)i1 < (int)i2 },
                new FilterComparerViewModel { Sign = ">=", Comparer = (i1, i2) => (int)i1 >= (int)i2 },
                new FilterComparerViewModel { Sign = "<=", Comparer = (i1, i2) => (int)i1 <= (int)i2 }
            };
        }

        public object Value()
        {
            return int.Parse(Text);
        }

        public void SetSourceType(Type t) { }

        public DependencyProperty ValueProperty()
        {
            return TextProperty;
        }
    }
}
