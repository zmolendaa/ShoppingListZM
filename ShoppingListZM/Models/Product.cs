using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ShoppingListZM.Models;

public class Product : INotifyPropertyChanged
{
    private string name;
    private string unit;
    private int quantity;
    private bool isPurchased;

    public string Name { get => name; set { name = value; OnPropertyChanged(); } }
    public string Unit { get => unit; set { unit = value; OnPropertyChanged(); } }

    public int Quantity
    {
        get => quantity;
        set { if (quantity != value) { quantity = value; OnPropertyChanged(); } }
    }

    public bool IsPurchased
    {
        get => isPurchased;
        set { if (isPurchased != value) { isPurchased = value; OnPropertyChanged(); } }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}