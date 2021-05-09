using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using OpenQA.Selenium;
using PriceMonitor.ProductManager;

namespace PriceMonitor.WebScrapers
{
    public class AmazonScraper : ScrapingManager
    {
        public AmazonScraper()
        {
            _url = "https://www.amazon.co.uk/";
            _driver.Navigate().GoToUrl(_url);
        }

        public override List<Item> Search(string toSearch, string category = null)
        {
            try
            {
                SetCategory(category);
                _driver.FindElementById("twotabsearchtextbox").SendKeys(toSearch);
                _driver.FindElementById("twotabsearchtextbox").SendKeys(Keys.Enter);

                IWebElement mainDiv = _driver.FindElementById("search");
                _items = mainDiv.FindElements(By.XPath(".//div[contains(@class, 'a-section a-spacing-medium')]"));

                foreach (var item in _items)
                {
                    try
                    {
                        string title = item
                            .FindElement(
                                By.XPath(".//h2[@class='a-size-mini a-spacing-none a-color-base s-line-clamp-2']"))
                            .Text;
                        string price = item.FindElement(By.XPath(".//span[@class='a-price']")).Text;
                        string href = item.FindElement(By.XPath(".//a[@class='a-link-normal a-text-normal']"))
                            .GetAttribute("href");
                        _productsDetails.Add(new Item(title, price, null, href, Web.Amazon, category));
                    }
                    catch (Exception e)
                    {
                    }
                }
            }
            catch (Exception e)
            { }

            _driver.Dispose();
            return _productsDetails;
        }

        private void SetCategory(string category)
        {
            IList<IWebElement> categories = _driver.FindElementsByXPath(".//select[@class='nav-search-dropdown searchSelect']//option");
            foreach (var drpDownCategory in categories)
            {
                if (drpDownCategory.Text.Equals(category))
                    drpDownCategory.Click();
            }
        }

        public override List<string> GetCategories()
        {
            _driver.Navigate().GoToUrl(_url);

            IList<IWebElement> categories = _driver.FindElementsByXPath(".//select[@class='nav-search-dropdown searchSelect']//option");
            foreach (var category in categories)
                _categories.Add(category.Text);

            return _categories;
        }
    }
}