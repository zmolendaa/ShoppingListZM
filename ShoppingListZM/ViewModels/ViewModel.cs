using System.Collections.ObjectModel;
using System.Linq;
using ShoppingListZM.Models;
using ShoppingListZM.Services;

namespace ShoppingListZM.ViewModels;

public class ViewModel
{
    private readonly DataService _dataService = new DataService();
    public ObservableCollection<Category> Categories { get; } = new ObservableCollection<Category>();

    public Command AddCategoryCommand { get; }
    public Command<Category> AddProductCommand { get; }
    public Command<Product> RemoveProductCommand { get; }
    public Command<Product> IncreaseQuantityCommand { get; }
    public Command<Product> DecreaseQuantityCommand { get; }
    public Command<Category> ToggleExpandCommand { get; }

    public ViewModel()
    {
        AddCategoryCommand = new Command(AddCategory);
        AddProductCommand = new Command<Category>(AddProduct);
        RemoveProductCommand = new Command<Product>(RemoveProduct);
        IncreaseQuantityCommand = new Command<Product>(p => { p.Quantity++; Save(); });
        DecreaseQuantityCommand = new Command<Product>(p => { if (p.Quantity > 1) p.Quantity--; Save(); });
        ToggleExpandCommand = new Command<Category>(c => c.IsExpanded = !c.IsExpanded);

        LoadData();
    }

    private void LoadData()
    {
        Categories.Clear();
        var data = _dataService.LoadData();
        foreach (var cat in data)
        {
            foreach (var prod in cat.Products)
            {
                prod.PropertyChanged += (s, e) => {
                    if (e.PropertyName == nameof(Product.IsPurchased)) SortProducts(cat);
                    Save();
                };
            }
            SortProducts(cat);
            Categories.Add(cat);
        }
    }

    private void SortProducts(Category cat)
    {
        var sorted = cat.Products.OrderBy(p => p.IsPurchased).ToList();
        for (int i = 0; i < sorted.Count; i++)
        {
            var oldIndex = cat.Products.IndexOf(sorted[i]);
            if (oldIndex != i) cat.Products.Move(oldIndex, i);
        }
    }

    private async void AddCategory()
    {
        string name = await App.Current.MainPage.DisplayPromptAsync("Nowa kategoria", "Wpisz nazwę:", "Dodaj", "Anuluj");
        if (!string.IsNullOrWhiteSpace(name))
        {
            Categories.Add(new Category { Name = name });
            Save();
        }
    }

    private async void AddProduct(Category cat)
    {
        string name = await App.Current.MainPage.DisplayPromptAsync("Nowy produkt", "Podaj nazwę produktu:", "Dalej", "Anuluj");
        if (string.IsNullOrWhiteSpace(name)) return;


        string unit = await App.Current.MainPage.DisplayActionSheet("Wybierz jednostkę:", "Anuluj", null, "szt.", "kg", "l", "g", "opak.");
        if (unit == "Anuluj" || string.IsNullOrEmpty(unit)) return;

     
        string quantityStr = await App.Current.MainPage.DisplayPromptAsync("Ilość", $"Ile {unit} produktu {name}?", "Dodaj", "Anuluj", initialValue: "1", keyboard: Keyboard.Numeric);
        if (quantityStr == null) return;

        int.TryParse(quantityStr, out int quantity);
        if (quantity <= 0) quantity = 1;


        var p = new Product
        {
            Name = name,
            Unit = unit,
            Quantity = quantity,
            IsPurchased = false
        };

        p.PropertyChanged += (s, e) => {
            if (e.PropertyName == nameof(Product.IsPurchased)) SortProducts(cat);
            Save(); 
        };

        cat.Products.Add(p);
        SortProducts(cat);
        Save();
    }

    private void RemoveProduct(Product p)
    {
        foreach (var c in Categories)
        {
            if (c.Products.Contains(p)) { c.Products.Remove(p); break; }
        }
        Save();
    }

    private void Save() => _dataService.SaveData(Categories);
}