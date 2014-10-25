using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Schedule.Models.ViewModels;
using System.Windows.Media;

namespace Schedule.Controls.Editors.Filters
{
    class DateFilter : DatePicker, IFilterControl
    {
        public DateFilter()
        {
            Background = Brushes.White;
        }

        public IEnumerable<FilterComparerViewModel> Comparers()
        {
            return new List<FilterComparerViewModel>
            {
                new FilterComparerViewModel { Sign = "=", Comparer = (i1, i2) => (DateTime)i1 == (DateTime)i2 },
                new FilterComparerViewModel { Sign = "<>", Comparer = (i1, i2) => (DateTime)i1 != (DateTime)i2 },
                new FilterComparerViewModel { Sign = ">", Comparer = (i1, i2) => (DateTime)i1 > (DateTime)i2 },
                new FilterComparerViewModel { Sign = "<", Comparer = (i1, i2) => (DateTime)i1 < (DateTime)i2 },
                new FilterComparerViewModel { Sign = ">=", Comparer = (i1, i2) => (DateTime)i1 >= (DateTime)i2 },
                new FilterComparerViewModel { Sign = "<=", Comparer = (i1, i2) => (DateTime)i1 <= (DateTime)i2 }
            };
        }

        public object Value
        {
            get { return SelectedDate; }
        }

        public DependencyProperty ValueProperty
        {
            get { return SelectedDateProperty; }
        }

        public void SetSourceType(Type t) { }
    }
}
