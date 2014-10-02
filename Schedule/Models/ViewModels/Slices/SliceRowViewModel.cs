using System.Collections.Generic;

namespace Schedule.Models.ViewModels.Slices
{
    public class SliceRowViewModel : BaseViewModel
    {
        private object _header;
        private ICollection<SliceCellViewModel> _items;

        public object Header
        {
            get { return _header; }
            set
            {
                _header = value;
                OnPropertyChanged();
            }
        }

        public ICollection<SliceCellViewModel> Items
        {
            get { return _items; }
            set
            {
                _items = value;
                OnPropertyChanged();
            }
        } 
    }
}
