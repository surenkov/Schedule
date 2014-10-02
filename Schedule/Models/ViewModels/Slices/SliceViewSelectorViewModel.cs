using System;

namespace Schedule.Models.ViewModels.Slices
{
    public class SliceViewSelectorViewModel : BaseViewModel
    {
        private Type _headerType;
        private string _name;

        public Type HeaderType
        {
            get { return _headerType; }
            set
            {
                _headerType = value;
                OnPropertyChanged();
            }
        }

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }
    }
}
