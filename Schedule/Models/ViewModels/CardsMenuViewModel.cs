using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedule.Models.ViewModels
{
    class CardsMenuViewModel : BaseViewModel
    {
        private string _header;
        private IEnumerable<Entity> _items;

        public string Header
        {
            get { return _header; }
            set
            {
                _header = value;
                OnPropertyChanged();
            }
        }

        public IEnumerable<Entity> Items
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
