using System;

namespace Schedule.Utils.Attributes
{
    class HiddenAttribute : Attribute
    {
        private bool _hide;

        public HiddenAttribute(bool hide = true)
        {
            _hide = hide;
        }

        public bool Hidden
        {
            get { return _hide; }
        }
    }
}
