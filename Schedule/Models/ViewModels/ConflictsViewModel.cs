using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Schedule.Models.ViewModels
{
    public class ConflictsViewModel : BaseViewModel
    {
        private object item;
        private string description;
        private ObservableCollection<ConflictsViewModel> children;

        public ConflictsViewModel()
        {
            children = new ObservableCollection<ConflictsViewModel>();
        }

        public object Item
        {
            get { return item; }
            set
            {
                item = value;
                OnPropertyChanged();
            }
        }

        public string Description
        {
            get { return description; }
            set
            {
                description = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<ConflictsViewModel> Children
        {
            get { return children; }
            set
            {
                children = value;
                OnPropertyChanged();
            }
        }
    }
}
