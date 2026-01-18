using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ShoppingListZM.Models;

public class Category : INotifyPropertyChanged
{
    private bool isExpanded = true;
    public string Name { get; set; }
    public bool IsExpanded { get => isExpanded; set { isExpanded = value; OnPropertyChanged(); } }
    public ObservableCollection<Product> Products { get; set; } = new ObservableCollection<Product>();

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}