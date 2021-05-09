using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using PriceMonitor.EntityFramework;
using PriceMonitor.ProductManager;

namespace PriceMonitor.UI
{
    /// <summary>
    /// Interaction logic for StartupPage.xaml
    /// </summary>
    public partial class StartupPage : Window
    {
        DatabaseManager database = new DatabaseManager();

        public StartupPage()
        {
            InitializeComponent();
        }

        private void StartupPage_OnLoaded(object sender, RoutedEventArgs e)
        {
            SetDisplayUsername();
            PopulateDataGrid();
        }

        public void PopulateDataGrid()
        {
            List<UserItem> items = database.GetMonitoredProducts(UserAccount.LoggedUser.Username);
            if (items.Count > 0)
            {
                NoItemsMessage.Visibility = Visibility.Collapsed;
                myProducts.ItemsSource = items;
                myProducts.Visibility = Visibility.Visible;
            }
            else
            {
                myProducts.Visibility = Visibility.Collapsed;
                NoItemsMessage.Visibility = Visibility.Visible;
            }
        }

        private void SetDisplayUsername()
        {
            string username = UserAccount.LoggedUser.Username;
            Username.Text = username;
        }

        private void StartupPage_OnClosed(object sender, EventArgs e)
        {
            DatabaseManager.Log(ActivityType.Logout, UserAccount.LoggedUser);
            UserAccount.Logout();
            CloseAllWindows();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to exit? All windows will be closed",
                "Exit", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);

            if (result == MessageBoxResult.Yes)
            {
                DatabaseManager.Log(ActivityType.Logout, UserAccount.LoggedUser);
                ShowLoginPage();
                CloseAllWindows();
            }
        }

        private void RemoveMonitor_Click(object sender, RoutedEventArgs e)
        {
            var itemToRemove = myProducts.SelectedItem as UserItem;
            database.RemoveMonitoredProduct(itemToRemove, UserAccount.LoggedUser);
            if (UserAccount.LoggedUser.Email != null)
            {
                MessageBoxResult result =
                    MessageBox.Show(
                        "Do you want to receive an email with the details of the product that you are about to remove?",
                        "", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    Email email = new Email(UserAccount.LoggedUser.Email);
                    email.Send("Removed Product", itemToRemove.Title, itemToRemove.Subtotal.ToString(),
                        itemToRemove.Shipping.ToString(),
                        itemToRemove.Total.ToString(), itemToRemove.Website, null, itemToRemove.Url, true);
                }
            }

            DatabaseManager.Log(ActivityType.RemovedMonitoredProduct, UserAccount.LoggedUser);

            PopulateDataGrid();
        }

        private void LogsShow_OnClick(object sender, RoutedEventArgs e)
        {
            Logs.Text = "";
            LogsPanel.Visibility = Visibility.Visible;
            ShowLogs.Visibility = Visibility.Collapsed;
            HideLogs.Visibility = Visibility.Visible;

            Date.Text = DateTime.Now.ToString("dddd, dd MMMM, yyyy");
            List<string> logs = DatabaseManager.GetLogs(UserAccount.LoggedUser);
            logs.ForEach(log => { Logs.Text += log + "\n"; });
        }

        private void LogsHide_OnClick(object sender, RoutedEventArgs e)
        {
            Logs.Text = "";
            LogsPanel.Visibility = Visibility.Collapsed;
            ShowLogs.Visibility = Visibility.Visible;
            HideLogs.Visibility = Visibility.Collapsed;
        }

        private void CloseAllWindows()
        {
            for (int i = App.Current.Windows.Count - 1; i >= 0; i--)
            {
                if (!App.Current.Windows[i].Title.Equals("Login Form"))
                    App.Current.Windows[i].Close();
            }
        }

        private void ShowLoginPage() => new LoginPage().Show();

        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string header = ((sender as TabControl).SelectedItem as TabItem).Header as string;

            bool active = false;
            if (header.Equals("Search"))
            {
                for (int i = 0; i < App.Current.Windows.Count - 1; i++)
                {
                    if (App.Current.Windows[i].Title.Equals("Search Form"))
                    {
                        active = true;
                        break;
                    }
                }

                if (!active)
                    new ProductsSearchPage().Show();
            }
        }
    }
}
