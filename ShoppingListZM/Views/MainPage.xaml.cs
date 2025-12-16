using Microsoft.Maui.Controls;
using ShoppingListZM.Models;
using ShoppingListZM.ViewModels;

namespace ShoppingListZM.Views
{
    public partial class MainPage : ContentPage
    {
        private MainPageViewModel ViewModel => BindingContext as MainPageViewModel;

        public MainPage()
        {
            InitializeComponent();
            BindingContext = new MainPageViewModel();
        }

        private void ToggleCategoryExpanded(object sender, EventArgs e)
        {
            if (sender is Button button && button.BindingContext is ProductGroup group)
            {
                group.IsExpanded = !group.IsExpanded;
                button.Text = group.IsExpanded ? "▼" : "▶";
            }
        }

        private void OnProductCheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (sender is CheckBox checkBox && checkBox.BindingContext is Product product)
            {
                product.IsPurchased = e.Value;
                ViewModel?.GroupSortFilter();
                ViewModel?.SaveAll();
            }
        }

        private void OnDeleteProductClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.BindingContext is Product product)
            {
                ViewModel?.DeleteProduct(product);
            }
        }

        private void OnDecreaseQuantityClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.BindingContext is Product product)
            {
                product.Quantity = Math.Max(0, product.Quantity - 1);
                ViewModel?.SaveAll();
            }
        }

        private void OnIncreaseQuantityClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.BindingContext is Product product)
            {
                product.Quantity += 1;
                ViewModel?.SaveAll();
            }
        }

        private void OnQuantityCompleted(object sender, EventArgs e)
        {
            if (sender is Entry entry && entry.BindingContext is Product product)
            {
                if (int.TryParse(entry.Text, out int quantity))
                {
                    product.Quantity = quantity;
                    ViewModel?.SaveAll();
                }
            }
        }

        private async void OnAddProductClicked(object sender, EventArgs e)
        {
            await ViewModel.ExecuteAddNewItem();
        }

        private async void OnAddCategoryClicked(object sender, EventArgs e)
        {
            await ViewModel.ExecuteAddNewCategory();
        }
    }
}
