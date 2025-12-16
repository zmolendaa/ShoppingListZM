using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using ShoppingListZM.Models;
using ShoppingListZM.Services;

namespace ShoppingListZM.ViewModels
{
    public class MainPageViewModel : BindableObject
    {
        public enum SortOption { Category, Name, Quantity }

        public ObservableCollection<Product> AllProducts { get; set; } = new ObservableCollection<Product>();
        public ObservableCollection<ProductGroup> GroupedProducts { get; set; } = new ObservableCollection<ProductGroup>();
        public ObservableCollection<string> AvailableStores { get; set; } = new ObservableCollection<string> { "All" };
        public List<string> AvailableCategories { get; set; } = new List<string> { "Dairy", "Vegetables", "Electronics", "Meat", "Bakery", "Inne" };

        private readonly DataStorageService _storageService = new DataStorageService();

        private string _storeFilter = "All";
        public string StoreFilter
        {
            get => _storeFilter;
            set
            {
                if (_storeFilter != value)
                {
                    _storeFilter = value;
                    OnPropertyChanged();
                    GroupSortFilter();
                }
            }
        }

        private SortOption _selectedSortOption = SortOption.Category;
        public SortOption SelectedSortOption
        {
            get => _selectedSortOption;
            set
            {
                if (_selectedSortOption != value)
                {
                    _selectedSortOption = value;
                    OnPropertyChanged();
                    GroupSortFilter();
                }
            }
        }

       

        public MainPageViewModel()
        {
            LoadAll();

            if (AllProducts.Count == 0)
            {
                AllProducts.Add(new Product
                {
                    Name = "Mleko 3.2%",
                    Quantity = 1,
                    Unit = "l",
                    CategoryName = "Dairy",
                    Store = "Biedronka",
                    IsPurchased = false
                });
                AllProducts.Add(new Product
                {
                    Name = "Banany",
                    Quantity = 5,
                    Unit = "szt",
                    CategoryName = "Vegetables",
                    Store = "Lidl",
                    IsPurchased = false
                });
                SaveAll();
            }

            UpdateAvailableStores();
            GroupSortFilter();
        }

        public async Task ExecuteAddNewItem()
        {
            string name = await App.Current.MainPage.DisplayPromptAsync(
                "Nowy produkt",
                "Podaj nazwę produktu:",
                "OK",
                "Anuluj",
                "Nazwa produktu"
            );
            if (string.IsNullOrWhiteSpace(name))
                return;

            string unit = await App.Current.MainPage.DisplayPromptAsync(
                "Nowy produkt",
                "Podaj jednostkę miary (np. szt, kg, l):",
                "OK",
                "Anuluj",
                "szt"
            );
            if (string.IsNullOrWhiteSpace(unit))
                unit = "szt";

            string quantityInput = await App.Current.MainPage.DisplayPromptAsync(
                "Nowy produkt",
                "Podaj ilość:",
                "OK",
                "Anuluj",
                "1"
            );
            if (!int.TryParse(quantityInput, out int quantity))
                quantity = 1;

            string category = await App.Current.MainPage.DisplayPromptAsync(
                "Nowy produkt",
                "Podaj kategorię:",
                "OK",
                "Anuluj",
                "Inne"
            );
            if (string.IsNullOrWhiteSpace(category))
                category = "Inne";

            string store = await App.Current.MainPage.DisplayPromptAsync(
                "Nowy produkt",
                "Podaj sklep:",
                "OK",
                "Anuluj",
                "Nie określono"
            );
            if (string.IsNullOrWhiteSpace(store))
                store = "Nie określono";

            AddNewProduct(new Product
            {
                Name = name,
                Quantity = quantity,
                Unit = unit,
                CategoryName = category,
                Store = store,
                IsPurchased = false
            });
        }

        public async Task ExecuteAddNewCategory()
        {
            string newCategory = await App.Current.MainPage.DisplayPromptAsync("Nowa kategoria", "Podaj nazwę kategorii:");
            if (!string.IsNullOrWhiteSpace(newCategory) && !AvailableCategories.Contains(newCategory))
            {
                AvailableCategories.Add(newCategory);
                GroupSortFilter();
                SaveAll();
            }
        }

        public void AddNewProduct(Product newProduct)
        {
            if (newProduct == null) return;
            if (string.IsNullOrWhiteSpace(newProduct.Name))
                newProduct.Name = "Bez nazwy";
            if (string.IsNullOrWhiteSpace(newProduct.CategoryName))
                newProduct.CategoryName = "Inne";
            if (string.IsNullOrWhiteSpace(newProduct.Store))
                newProduct.Store = "Nie określono";

            AllProducts.Add(newProduct);
            SaveAll();
            GroupSortFilter();
            UpdateAvailableStores();
        }

        public void DeleteProduct(Product product)
        {
            if (product != null && AllProducts.Remove(product))
            {
                SaveAll();
                GroupSortFilter();
                UpdateAvailableStores();
            }
        }

        public void GroupSortFilter()
        {
            try
            {
                var itemsToBuy = AllProducts.Where(p => !p.IsPurchased).ToList();
                var purchasedItems = AllProducts.Where(p => p.IsPurchased).ToList();

                switch (SelectedSortOption)
                {
                    case SortOption.Name:
                        itemsToBuy = itemsToBuy.OrderBy(p => p.SortableName).ToList();
                        break;
                    case SortOption.Quantity:
                        itemsToBuy = itemsToBuy.OrderByDescending(p => p.Quantity).ToList();
                        break;
                    case SortOption.Category:
                    default:
                        itemsToBuy = itemsToBuy.OrderBy(p => p.SortableCategory).ThenBy(p => p.SortableName).ToList();
                        break;
                }

                var allItems = itemsToBuy.Concat(purchasedItems).ToList();

                GroupedProducts.Clear();
                var groupedItems = allItems
                    .GroupBy(p => p.IsPurchased ? "Kupione" : p.CategoryName)
                    .OrderBy(g => g.Key == "Kupione" ? 1 : 0)
                    .ThenBy(g => g.Key);

                foreach (var group in groupedItems)
                {
                    GroupedProducts.Add(new ProductGroup(
                        group.Key,
                        group.Key == "Kupione",
                        group.ToList()
                    ));
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd w GroupSortFilter: {ex.Message}");
            }
        }

        public void SaveAll()
        {
            try
            {
                _storageService.SaveProducts(AllProducts);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd zapisu: {ex.Message}");
            }
        }

        private void LoadAll()
        {
            try
            {
                var loadedProducts = _storageService.LoadProducts();
                foreach (var product in loadedProducts)
                {
                    if (string.IsNullOrWhiteSpace(product.CategoryName))
                        product.CategoryName = "Inne";
                    if (string.IsNullOrWhiteSpace(product.Name))
                        product.Name = "Bez nazwy";
                    if (string.IsNullOrWhiteSpace(product.Store))
                        product.Store = "Nie określono";

                    AllProducts.Add(product);

                    if (!AvailableCategories.Contains(product.CategoryName))
                    {
                        AvailableCategories.Add(product.CategoryName);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd ładowania: {ex.Message}");
            }
        }

        private void UpdateAvailableStores()
        {
            try
            {
                AvailableStores.Clear();
                AvailableStores.Add("All");
                var uniqueStores = AllProducts
                    .Select(p => p.Store)
                    .Where(s => !string.IsNullOrWhiteSpace(s))
                    .Distinct()
                    .OrderBy(s => s);

                foreach (var store in uniqueStores)
                {
                    AvailableStores.Add(store);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd aktualizacji sklepów: {ex.Message}");
            }
        }
    }
}
