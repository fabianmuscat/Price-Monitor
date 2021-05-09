using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using MaterialDesignThemes.Wpf;
using PriceMonitor.EntityFramework;
using PriceMonitor.ProductManager;
using PriceMonitor.WebScrapers;

namespace PriceMonitor.UI
{
    /// <summary>
    /// Interaction logic for ProductsSearchPage.xaml
    /// </summary>
    public partial class ProductsSearchPage : Window
    {

        public ProductsPage ProductsPage = new ProductsPage();
        private readonly DatabaseManager _database = new DatabaseManager();
        private readonly List<string> _filteredWebsites = new List<string>();
        private List<Item> _products = new List<Item>();
        private readonly List<Item> _toShow = new List<Item>();
        private int _checkboxesSelected;

        public ProductsSearchPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Event Handling
        /// </summary>
        private void ProductsSearchPage_OnLoaded(object sender, RoutedEventArgs e)
        {
            CategoryComboBox.IsEnabled = false;
            SearchButton.IsEnabled = (!string.IsNullOrEmpty(SearchBox.Text));
            _database.AddCategory("N/A");
        }

        private void MenuItem_OnClick(object sender, RoutedEventArgs e) => CategoryComboBox.SelectedIndex = -1;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string category = GetCategory(); // Getting the selected category
            string toSearch = SearchBox.Text;

            /* if more than one website was selected, the list will be used instead.
             * First the database will searched for any products that have a matching
             * website, search term and category.
             * if nothing was found, those websites will be scraped and the results will be returned in a list
             * and added to the list which will be used to display the products in the data grid
             */
            void Scrape(string website)
            {
                ScrapingManager sm = null;
                switch (website.ToLower())
                {
                    case "amazon":
                        sm = new AmazonScraper();
                        break;

                    case "ebay":
                        sm = new EbayScraper();
                        break;

                    case "walmart":
                        sm = new WalmartScraper();
                        break;

                    case "atoz":
                        sm = new AToZScraper();
                        break;

                    case "gearbest":
                        sm = new GearBestScraper();
                        break;

                    case "scan":
                        sm = new ScanScraper();
                        break;

                    case "microcenter":
                        sm = new MicrocenterScraper();
                        break;

                    case "newegg":
                        sm = new NewEggScraper();
                        break;
                }
                if (sm != null) Search(sm, website, toSearch, category);
            }

            List<string> nullWebsites = new List<string>();
            if (!CheckIfSearchExpired(toSearch))
            {
                _database.AddSearchTerm(toSearch);
                _filteredWebsites.ForEach(website =>
                {
                    _products = _database.Search(website, toSearch, category);
                    if (_products != null) _products.ForEach(item => _toShow.Add(item));
                    else nullWebsites.Add(website);
                });
                if (nullWebsites.Count != 0) nullWebsites.ForEach(Scrape);
            }
            else
            {
                _database.AddSearchTerm(toSearch);
                _filteredWebsites.ForEach(website =>
                {
                    _products = _database.Search(website, toSearch, category);
                    if (_products != null) _products.ForEach(item => _toShow.Add(item));
                    else nullWebsites.Add(website);
                });
                if (nullWebsites.Count != 0) nullWebsites.ForEach(Scrape);
            }

            if (_toShow.Count == 0) MessageBox.Show("No Items were found", "", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            else
            {
                ProductsPage.products.ItemsSource = _toShow; // setting the source to the list which contains all products from the websites selected
                ProductsPage.Show();
            }
            DatabaseManager.Log(ActivityType.Search, UserAccount.LoggedUser, toSearch);
        }

        private void SearchBox_OnTextChanged(object sender, TextChangedEventArgs e) => SearchButton.IsEnabled = (_checkboxesSelected > 0 && !string.IsNullOrEmpty(SearchBox.Text));

        /// <summary>
        /// Interaction logic for Category Combo Box 
        /// </summary>
        private void PopulateCategoryComboBox(List<string> items)
        {
            // Creating and adding a new ComboBoxItem to the combo box with each category in the list
            items.ForEach(category =>
            {
                ComboBoxItem cbi = new ComboBoxItem();
                cbi.Content = category;
                CategoryComboBox.Items.Add(cbi);
            });
        }

        /// <summary>
        /// Interaction logic for websites checkbox
        /// </summary>
        private void AllWebsites_OnChecked(object sender, RoutedEventArgs e)
        {
            bool newVal = (AllWebsitesCheckBox.IsChecked == true);
            Amazon.IsChecked = newVal;
            AtoZ.IsChecked = newVal;
            Walmart.IsChecked = newVal;
            GearBest.IsChecked = newVal;
            NewEgg.IsChecked = newVal;
            Scan.IsChecked = newVal;
            Ebay.IsChecked = newVal;
            Microcenter.IsChecked = newVal;
        }

        private void Website_OnChecked(object sender, RoutedEventArgs e) => CheckBoxChecked(((CheckBox)e.Source).Content.ToString(), ((CheckBox)e.Source).Tag.ToString());
        
        private void Website_OnUnchecked(object sender, RoutedEventArgs e) => CheckBoxUnchecked(((CheckBox)e.Source).Content.ToString());

        private void EmptyCategoryComboBox() => CategoryComboBox.Items.Clear();

        private void CheckUncheckAllWebsitesBox()
        {
            AllWebsitesCheckBox.IsChecked = null;
            if ((bool)Amazon.IsChecked && (bool)AtoZ.IsChecked && (bool)Walmart.IsChecked && (bool)GearBest.IsChecked && (bool)NewEgg.IsChecked && (bool)Scan.IsChecked &&
                (bool)Ebay.IsChecked && (bool)Microcenter.IsChecked)
                AllWebsitesCheckBox.IsChecked = true;

            if ((bool)!Amazon.IsChecked && (bool)!AtoZ.IsChecked && (bool)!Walmart.IsChecked && (bool)!GearBest.IsChecked && (bool)!NewEgg.IsChecked && (bool)!Scan.IsChecked &&
                (bool)!Ebay.IsChecked && (bool)!Microcenter.IsChecked)
                AllWebsitesCheckBox.IsChecked = false;
        }

        private void CheckBoxChecked(string website, string url)
        {
            _filteredWebsites.Add(website); // adding the website checked to search from the database/browser
            _database.AddWebsite(website, url);

            _filteredWebsites.ForEach(web => Debug.Print(web));
            _checkboxesSelected++;
            SearchButton.IsEnabled = (_checkboxesSelected > 0 && !string.IsNullOrEmpty(SearchBox.Text));

            /* the category combo box is disabled if more than 2 websites are checked since there would be
             * a mixture of categories from different websites. It will only be enabled if 1 website is checked
             * and its categories will be shown in the combo box
             */

            if (_checkboxesSelected >= 2)
            {
                CategoryComboBox.IsEnabled = false;
                CategoryComboBox.SelectedIndex = -1;
            }
            if (_checkboxesSelected <= 1)
            {
                /* the category combo box is disabled for the following websites
                 * as the functionality of the categories in these websites do not work
                 * as I want them to for this program.
                 */
                if (website.ToLower().Equals("new egg") || website.ToLower().Equals("microcenter") ||
                    website.ToLower().Equals("scan") || website.ToLower().Equals("atoz"))
                    CategoryComboBox.IsEnabled = false;
                else
                {
                    CategoryComboBox.IsEnabled = true;
                    GetCategories(website);
                }
            }
            CheckUncheckAllWebsitesBox();
        }

        private void CheckBoxUnchecked(string website)
        {
            _filteredWebsites.Remove(website);
            EmptyCategoryComboBox(); // emptying the combo box before populating it again with new categories

            _checkboxesSelected--;
            SearchButton.IsEnabled = (_checkboxesSelected > 0 && !string.IsNullOrEmpty(SearchBox.Text));

            // Populating the combo box with the checked checkbox only if one checkbox is checked at that time
            CheckBox[] checkBoxes = {Amazon, Ebay, Walmart, GearBest};
            if (_checkboxesSelected <= 1)
            {
                // checking if each checkbox checked property is not null and that it is checked
                foreach (var checkBox in checkBoxes)
                    if (checkBox.IsChecked != null && (bool) checkBox.IsChecked) GetCategories(checkBox.Content.ToString());
                CategoryComboBox.IsEnabled = true;
            }

            if (_checkboxesSelected == 0) CategoryComboBox.IsEnabled = false;
            CheckUncheckAllWebsitesBox();
        }

        /// <summary>
        /// Search Methods
        /// </summary>
        private bool CheckIfSearchExpired(string searchTerm)
        {
            bool expired = false;
            Search search = _database.GetSearch(searchTerm);
            if (search != null)
            {
                DateTime timeSearched = search.Date;
                if (DateTime.Now.Subtract(timeSearched) > new TimeSpan(0, 0, 30, 0))
                {
                    _database.RemoveProductsWithSearch(searchTerm);
                    expired = true;
                }
            }

            return expired;
        }
        
        private void Search(ScrapingManager scraper, string website, string searchTerm, string category)
        {
            /* This method searches for products with the provided search term and category.
             * The products are than added to the database and no website is added if the category is "N/A"
             * Products found are added to the list that will be displayed in the data grid
             */

            _products = scraper.Search(searchTerm, category);
            if (category.Equals("N/A")) _database.AddProducts(_products, website, searchTerm, true);
            else _database.AddProducts(_products, website, searchTerm);
            _products.ForEach(product => _toShow.Add(product));
        }

        /// <summary>
        /// Category Methods
        /// </summary>
        private string GetCategory()
        {
            /* Setting the category to the item selected in the category combo box.
             * if no item was selected the category would be the first item in the combo box
             * and if the combo box is disabled the category would be set to "N/A"
             */

            string category;

            if (CategoryComboBox.SelectedIndex != -1) category = CategoryComboBox.Text;
            else if (!CategoryComboBox.IsEnabled) category = "N/A";
            else
            {
                CategoryComboBox.SelectedIndex = 0;
                category = CategoryComboBox.Text;
            }

            return category;
        }
        private void GetCategories(string website)
        {
            /* Getting the categories for a selected website either
             * from the database or from the website if the database does   
             * not contain the categories
             */

            List<string> categories = _database.GetCategories(website);
            if (categories != null) PopulateCategoryComboBox(categories);
            else
            {
                Dictionary<string, string> toAdd = new Dictionary<string, string>();
                ScrapingManager sm = null;
                switch (website.ToLower())
                {
                    case "amazon": sm = new AmazonScraper(); break;
                    case "ebay": sm = new EbayScraper(); break;
                    case "gearbest": sm = new GearBestScraper(); break;
                    case "walmart": sm = new WalmartScraper(); break;
                    case "atoz": sm = new AToZScraper(); break;
                }

                if (sm != null) categories = sm.GetCategories();
                if (categories != null)
                {
                    categories.ForEach(cat => toAdd.Add(cat, website));
                    _database.AddCategory(toAdd);
                }

                categories = _database.GetCategories(website);
                PopulateCategoryComboBox(categories);
            }
        }
    }
}