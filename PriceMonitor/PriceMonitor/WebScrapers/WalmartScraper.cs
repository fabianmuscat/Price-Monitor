using System;
using System.Collections.Generic;
using System.Threading;
using OpenQA.Selenium;
using PriceMonitor.ProductManager;

namespace PriceMonitor.WebScrapers
{
    public class WalmartScraper : ScrapingManager
    {
        public WalmartScraper()
        {
            _url = "https://www.walmart.com/";
        }

        public override List<Item> Search(string toSearch, string category = null)
        {
            _driver.Navigate().GoToUrl(_url);

            try
            {
                SetCategory(category);
                _driver.FindElementById("global-search-input").SendKeys(toSearch);
                _driver.FindElementById("global-search-input").SendKeys(Keys.Enter);

                IWebElement mainDiv = _driver.FindElementById("searchProductResult");
                _items = mainDiv.FindElements(By.XPath(".//li[contains(@data-tl-id, 'ProductTileGridView')]"));

                string linkXPath = ".//a[contains(@class, 'product-title-link')]";
                foreach (var item in _items)
                {
                    try
                    {
                        string href = item.FindElement(By.XPath(linkXPath)).GetAttribute("href");
                        string title = item.FindElement(By.XPath(linkXPath)).Text;
                        string price = item
                            .FindElement(By.XPath(".//span[@class='price-main-block']//span[@class='price-group']")).Text;
                        _productsDetails.Add(new Item(title, price, null, href, Web.Walmart, category));
                    }
                    catch {}
                }
            }
            catch { }

            _driver.Dispose();
            return _productsDetails;
        }

        private void SetCategory(string categoryToSelect)
        {
            try
            {
                _driver.FindElementById("global-search-dropdown-toggle").Click();
                IWebElement form = _driver.FindElementById("global-search-form");
                IWebElement dropdown = form.FindElement(By.Id("searchDropdown-list"));

                IList<IWebElement> cols = dropdown.FindElements(By.XPath(".//div[@role='menu']"));
                foreach (var col in cols)
                {
                    string[] categories = col.Text.Split('\r', '\n');
                    foreach (string category in categories)
                    {
                        if (!string.IsNullOrEmpty(category) && category.Equals(categoryToSelect))
                            col.Click();
                    }
                }
            }
            catch { }
        }

        public override List<string> GetCategories()
        {
            _driver.Navigate().GoToUrl(_url);

            try
            {
                _driver.FindElementById("global-search-dropdown-toggle").Click();
                Thread.Sleep(1500);

                IWebElement form = _driver.FindElementById("global-search-form");
                IWebElement dropdown = form.FindElement(By.Id("searchDropdown-list"));

                IList<IWebElement> cols = dropdown.FindElements(By.XPath(".//div[@role='menu']"));
                foreach (var col in cols)
                {
                    string[] categories = col.Text.Split('\r', '\n');
                    foreach (string category in categories)
                    {
                        if (!string.IsNullOrEmpty(category))
                            _categories.Add(category);
                    }
                }
            }
            catch { }
            
            return _categories;
        }
    }
}