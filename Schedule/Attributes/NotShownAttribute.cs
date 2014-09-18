using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedule.Attributes
{
    class NotShownAttribute : Attribute
    {
        private bool _shown;

        public NotShownAttribute(bool shown = false)
        {
            _shown = shown;
        }

        public bool Shown
        {
            get { return _shown; }
        }
    }
}
