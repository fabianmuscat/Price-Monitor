using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using PriceMonitor.EntityFramework;
using PriceMonitor.ProductManager;

namespace PriceMonitor.UI   
{
    /// <summary>
    /// Interaction logic for ProductsPage.xaml
    /// </summary>
    public partial class ProductsPage : Window
    {
        List<Item> unfilteredProducts = new List<Item>();
        List<Item> filteredProducts = new List<Item>();

        public ProductsPage()
        {
            InitializeComponent();
        }

        private void ProductsPage_OnLoaded(object sender, RoutedEventArgs e)
        {
            FilterChip.IsEnabled = false;
            PriceFilterChip.IsEnabled = false;

            foreach (var product in products.Items)
                unfilteredProducts.Add(product as Item);
        }

        private void Arrow_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Click_HideRowDetails(object sender, RoutedEventArgs routedEventArgs) => products.SelectedIndex = -1;

        private void Hyperlink_OnRequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        private void TextBoxBase_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            FilterChip.IsEnabled = !string.IsNullOrEmpty(((TextBox)e.Source).Text);
            if (string.IsNullOrEmpty(((TextBox)e.Source).Text))
                products.ItemsSource = unfilteredProducts;
        }

        private void MonitorProduct_OnClick(object sender, RoutedEventArgs e)
        {
            Item selectedItem = (products.SelectedItem as Item);

            for (int i = 0; i < App.Current.Windows.Count - 1; i++)
            {
                if (App.Current.Windows[i].Title.Equals("Price Monitor"))
                {
                    List<object> updatedProducts = new List<object>();
                    new DatabaseManager().AddMonitoredProduct(selectedItem, UserAccount.LoggedUser);

                    if (UserAccount.LoggedUser.Email != null)
                    {
                        MessageBoxResult result =
                            MessageBox.Show(
                                "Do you want to receive an email with the details of the product that you are about to add?",
                                "", MessageBoxButton.YesNo, MessageBoxImage.Question);

                        if (result == MessageBoxResult.Yes)
                        {
                            Email email = new Email(UserAccount.LoggedUser.Email);
                            email.Send("Monitored New Product", selectedItem.Title, selectedItem.Price.ToString(), selectedItem.Shipping.ToString(),
                                selectedItem.Total.ToString(), selectedItem.Website, selectedItem.Category, selectedItem.Url, false);
                        }
                    }

                    (App.Current.Windows[i] as StartupPage).PopulateDataGrid();

                    DatabaseManager.Log(ActivityType.MonitoredProduct, UserAccount.LoggedUser);
                    break;
                }
            }
        }

        // Filters
        private void PriceMin_Changed(object sender, TextChangedEventArgs e)
        {
            PriceFilterChip.IsEnabled = !string.IsNullOrEmpty(((TextBox)e.Source).Text) || !string.IsNullOrEmpty(MaxPriceTxtBox.Text);
            if (string.IsNullOrEmpty(((TextBox)e.Source).Text))
                products.ItemsSource = unfilteredProducts;
        }

        private void PriceMax_Changed(object sender, TextChangedEventArgs e)
        {
            PriceFilterChip.IsEnabled = !string.IsNullOrEmpty(((TextBox)e.Source).Text) || !string.IsNullOrEmpty(MinPriceTxtBox.Text);
            if (string.IsNullOrEmpty(((TextBox)e.Source).Text))
                products.ItemsSource = unfilteredProducts;
        }

        private void Filter_OnClick(object sender, RoutedEventArgs e)
        {
            if (LikeRadio.IsChecked == true)
                filteredProducts = GetProductsLike(unfilteredProducts, FilterTxtBox.Text);

            else if (ExactlyLikeRadio.IsChecked == true)
                filteredProducts = GetProductsExactlyLike(unfilteredProducts, FilterTxtBox.Text);

            else if (StartingWithRadio.IsChecked == true)
                filteredProducts = GetProductsStartingWith(unfilteredProducts, FilterTxtBox.Text);

            products.ItemsSource = filteredProducts;
        }

        private void PriceFilter_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                decimal min = 0, max = 0;
                min = !string.IsNullOrEmpty(MinPriceTxtBox.Text) ? Convert.ToDecimal(MinPriceTxtBox.Text) : 0;
                max = !string.IsNullOrEmpty(MaxPriceTxtBox.Text) ? Convert.ToDecimal(MaxPriceTxtBox.Text) : 0;

                filteredProducts = GetProductsFromPriceRange(unfilteredProducts, min, max);
                products.ItemsSource = filteredProducts;
            }
            catch { }
        }

        private void ResetPrice_OnClick(object sender, RoutedEventArgs e)
        {
            MinPriceTxtBox.Text = "";
            MaxPriceTxtBox.Text = "";
            products.ItemsSource = unfilteredProducts;
        }

        private void ResetTitle_OnClick(object sender, RoutedEventArgs e)
        {
            FilterTxtBox.Text = "";
            products.ItemsSource = unfilteredProducts;
        }
        //

        private List<Item> GetProductsLike(List<Item> products, string filter)
        {
            return products.Where(product => product.Title.ToLower().Contains(filter.ToLower())).ToList();
        }

        private List<Item> GetProductsExactlyLike(List<Item> products, string filter)
        {
            return products.Where(product => product.Title.ToLower().Equals(filter.ToLower())).ToList();
        }

        private List<Item> GetProductsStartingWith(List<Item> products, string filter)
        {
            return products.Where(product => product.Title.ToLower().StartsWith(filter.ToLower())).ToList();
        }

        private List<Item> GetProductsFromPriceRange(List<Item> products, decimal min = 0, decimal max = 0)
        {
            if (max == 0)
            {
                products.ForEach(product =>
                {
                    if (product.Total > max)
                        max = product.Total;
                });
            }

            return products.Where(product => product.Total <= max && product.Total >= min).ToList();
        }

        private void ProductsPage_OnClosed(object sender, EventArgs e)
        {
            for (int i = 0; i < App.Current.Windows.Count - 1; i++)
            {
                if (App.Current.Windows[i].Title.Equals("Search Form"))
                {
                    (App.Current.Windows[i] as ProductsSearchPage).Close();
                    new ProductsSearchPage().Show();
                    break;
                }
            }
        }
    }
}