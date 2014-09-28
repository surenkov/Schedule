using System;
using System.Windows.Controls;
using Schedule.Controls.Editors;

namespace Schedule.Utils.Filters
{
    class FiltersFactory
    {
        private readonly Panel _panel;
        private readonly Type _type;
        private readonly int _height = 25;

        public FiltersFactory(Panel panel, Type type, int height = 25)
        {
            _panel = panel;
            _type = type;
            _height = height;
        }

        public Filter CreateFilter()
        {
            var filter = new Filter(_type) { Height = _height };
            filter.RemoveButton.Click += 
                (sender, args) => _panel.Children.Remove(filter);
            return filter;
        }
    }
}
