using System.Collections.ObjectModel;
using System.ComponentModel;

namespace ShoppingListZM.Models
{
    public class ProductGroup : ObservableCollection<Product>
    {
        private bool _isExpanded = true;
        public bool IsExpanded
        {
            get => _isExpanded;
            set
            {
                if (_isExpanded != value)
                {
                    _isExpanded = value;
                    OnPropertyChanged(new PropertyChangedEventArgs(nameof(IsExpanded)));
                }
            }
        }

        public string GroupName { get; set; }
        public bool IsPurchasedGroup { get; set; }

        public ProductGroup(string name, bool purchased, List<Product> items) : base(items)
        {
            GroupName = name ?? "Bez nazwy";
            IsPurchasedGroup = purchased;
        }
    }
}
