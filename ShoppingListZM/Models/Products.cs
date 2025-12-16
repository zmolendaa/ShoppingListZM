using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;

namespace ShoppingListZM.Models
{
    public class Product : BindableObject
    {
        private string _name = string.Empty;
        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value ?? string.Empty;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(SortableName));
                }
            }
        }

        private int _quantity = 1;
        public int Quantity
        {
            get => _quantity;
            set
            {
                if (_quantity != value)
                {
                    _quantity = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _unit = "szt"; 
        public string Unit
        {
            get => _unit;
            set
            {
                if (_unit != value)
                {
                    _unit = value ?? "szt";
                    OnPropertyChanged();
                }
            }
        }

        private string _categoryName = "Inne";
        public string CategoryName
        {
            get => _categoryName;
            set
            {
                if (_categoryName != value)
                {
                    _categoryName = value ?? "Inne";
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(SortableCategory));
                }
            }
        }

        private string _store = "Nie określono";
        public string Store
        {
            get => _store;
            set
            {
                if (_store != value)
                {
                    _store = value ?? "Nie określono";
                    OnPropertyChanged();
                }
            }
        }

        private bool _isPurchased = false;
        public bool IsPurchased
        {
            get => _isPurchased;
            set
            {
                if (_isPurchased != value)
                {
                    _isPurchased = value;
                    OnPropertyChanged();
                }
            }
        }

        public string SortableName => Name?.ToLowerInvariant() ?? string.Empty;
        public string SortableCategory => CategoryName?.ToLowerInvariant() ?? string.Empty;
    }
}
