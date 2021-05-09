using System.Collections.Generic;
using OpenQA.Selenium;
using PriceMonitor.ProductManager;

namespace PriceMonitor.WebScrapers
{
    public class MicrocenterScraper: ScrapingManager
    {
        public MicrocenterScraper()
        {
            _url = "https://www.microcenter.com/";
        }

        public override List<Item> Search(string toSearch, string category = null)
        {
            _driver.Navigate().GoToUrl(_url);

            try
            {
                _driver.FindElementById("search-query").SendKeys(toSearch);
                _driver.FindElementByXPath(".//form[@id='searchForm']//input[@name='searchButton']").Click();

                IWebElement mainDiv = _driver.FindElementByXPath(".//article[@id='productGrid']");
                _items = mainDiv.FindElements(By.XPath(".//li[contains(@class, 'product_wrapper')]"));

                foreach (var item in _items)
                {
                    string title = item.FindElement(By.TagName("h2")).Text;
                    string price = item.FindElement(By.XPath(".//span[@itemprop='price']")).Text;
                    string href = item.FindElement(By.XPath(".//a[contains(@id, 'hypProduct_')]")).GetAttribute("href");
                    _productsDetails.Add(new Item(title, price, null, href, Web.MicroCenter, category));
                }
            }
            catch { }

            _driver.Dispose();
            return _productsDetails;
        }
    }
}