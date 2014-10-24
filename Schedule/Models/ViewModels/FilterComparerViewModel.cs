using System;

namespace Schedule.Models.ViewModels
{
    public delegate bool CheckPropertyValueDelegate(object value, object compared);

    class FilterComparerViewModel : BaseViewModel
    {
        private string sign;

        public string Sign
        {
            get { return sign; }
            set
            {
                sign = value;
                OnPropertyChanged();
            }
        }

        private CheckPropertyValueDelegate comparer;

        public CheckPropertyValueDelegate Comparer
        {
            get { return comparer; }
            set
            {
                comparer = value;
                OnPropertyChanged();
            }
        }


    }
}
