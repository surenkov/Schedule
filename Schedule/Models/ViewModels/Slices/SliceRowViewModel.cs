﻿using System.Collections.Generic;

namespace Schedule.Models.ViewModels.Slices
{
    class SliceRowViewModel : BaseViewModel
    {
        private string _header;
        private ICollection<SliceCellViewModel> _items;

        public string Header
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
