using System;

namespace Schedule.Models.ViewModels
{
    class CardsMenuViewModel : BaseViewModel
    {
        private string _header;
        private Type _itemsType;

        public string Header
        {
            get { return _header; }
            set
            {
                _header = value;
                OnPropertyChanged();
            }
        }

        public Type ItemsType
        {
            get { return _itemsType; }
            set
            {
                _itemsType = value;
                OnPropertyChanged();
            }
        }

    }
}
