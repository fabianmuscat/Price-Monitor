using System;
using System.Collections.Generic;
using System.Threading;
using OpenQA.Selenium;
using PriceMonitor.ProductManager;

namespace PriceMonitor.WebScrapers
{
    public class ScanScraper : ScrapingManager
    {
        public ScanScraper()
        {
            _url = "https://www.scanmalta.com/shop/";
        }

        public override List<Item> Search(string toSearch, string category = null)
        {
            _driver.Navigate().GoToUrl(_url);

            try
            {
                _driver.Navigate().GoToUrl(_url);

                _driver.FindElementByXPath(".//input[@id='search']").SendKeys(toSearch);
                _driver.FindElementByXPath(".//input[@id='search']").SendKeys(Keys.Enter);

                Thread.Sleep(2000);
                IWebElement mainDiv = _driver.FindElementByXPath(".//div[@class='search results']");
                IReadOnlyCollection<IWebElement> items = mainDiv.FindElements(By.XPath(".//div[contains(@class, 'product-item-details')]"));

                foreach (var item in items)
                {
                    string title = item.FindElement(By.XPath(".//a[@class='product-item-link']")).Text;
                    string price = item.FindElement(By.XPath(".//span[@class='price']")).Text;
                    string href = item.FindElement(By.TagName("a")).GetAttribute("href");
                    _productsDetails.Add(new Item(title, price, "0", "Mobiles", "Scan", href));
                }
            } catch { }

            _driver.Dispose();
            return _productsDetails;
        }
    }
}