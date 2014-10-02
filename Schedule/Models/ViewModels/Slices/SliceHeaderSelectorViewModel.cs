using System;

namespace Schedule.Models.ViewModels.Slices
{
    class SliceHeaderSelectorViewModel : BaseViewModel
    {
        private Type _type;
        private string _title;

        public Type Type
        {
            get { return _type; }
            set
            {
                _type = value;
                OnPropertyChanged();
            }
        }

        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                OnPropertyChanged();
            }
        }
    }
}
