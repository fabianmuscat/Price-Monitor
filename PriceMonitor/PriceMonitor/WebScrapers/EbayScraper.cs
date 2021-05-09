using System;
using System.Collections.Generic;
using System.Threading;
using OpenQA.Selenium;
using PriceMonitor.ProductManager;

namespace PriceMonitor.WebScrapers
{
    public class EbayScraper : ScrapingManager
    {
        public EbayScraper()
        {
            _url = "https://www.ebay.co.uk/";
        }

        public override List<Item> Search(string toSearch, string category = null)
        {
            _driver.Navigate().GoToUrl(_url);

            try
            {
                SetCategory(category);
                _driver.FindElementById("gh-ac").SendKeys(toSearch);
                _driver.FindElementById("gh-btn").Click();

                Thread.Sleep(2000);
                IWebElement mainDiv = _driver.FindElement(By.XPath(".//div[contains(@class,'srp-river srp-layout-inner')]"));
                _items = mainDiv.FindElements(By.ClassName("s-item__info"));

                foreach (var item in _items)
                {
                    try
                    {
                        string price = item.FindElement(By.ClassName("s-item__price")).Text;
                        if (!price.Contains("to"))
                        {
                            string href = item.FindElement(By.XPath(".//a[@class='s-item__link']")).GetAttribute("href");
                            string title = item.FindElement(By.TagName("h3")).Text;
                            string shipping = item.FindElement(By.ClassName("s-item__shipping")).Text;
                            _productsDetails.Add(new Item(title, price, shipping, href, Web.Ebay, category));
                        }
                    }
                    catch { }
                }
            }
            catch { }

            _driver.Dispose();
            return _productsDetails;
        }

        private void SetCategory(string category)
        {
            IWebElement dropdown = _driver.FindElementById("gh-cat-box");
            IReadOnlyCollection<IWebElement> categories = dropdown.FindElements(By.TagName("option"));

            foreach (var drpDownCategory in categories)
            {
                if (drpDownCategory.Text.Equals(category))
                    drpDownCategory.Click();
            }
        }

        public override List<string> GetCategories()
        {
            _driver.Navigate().GoToUrl(_url);

            IWebElement dropdown = _driver.FindElementById("gh-cat-box");
            IReadOnlyCollection<IWebElement> categories = dropdown.FindElements(By.TagName("option"));

            foreach (var category in categories)
            {
                _categories.Add(category.Text);
            }

            return _categories;
        }
    }
}