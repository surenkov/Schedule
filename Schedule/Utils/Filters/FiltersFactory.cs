using System;
using System.Windows.Controls;
using Schedule.Controls.Editors;
using System.Collections.Generic;

namespace Schedule.Utils.Filters
{
    class FiltersFactory
    {
        private readonly Panel panel;
        private readonly Type type;
        private readonly int height = 25;

        public FiltersFactory(Panel p, Type t, int h = 25)
        {
            panel = p;
            type = t;
            height = h;
        }

        public Filter CreateFilter()
        {
            var filter = new Filter(type) { Height = height };
            filter.RemoveButton.Click += (sender, args) => panel.Children.Remove(filter);
            return filter;
        }
    }
}
