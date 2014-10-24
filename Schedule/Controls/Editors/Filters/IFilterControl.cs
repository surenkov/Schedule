using System;
using System.Collections.Generic;
using System.Windows;
using Schedule.Models.ViewModels;

namespace Schedule.Controls.Editors.Filters
{
    interface IFilterControl
    {
        IEnumerable<FilterComparerViewModel> Comparers();
        void SetSourceType(Type t);
        object Value();
        DependencyProperty ValueProperty();
    }
}
