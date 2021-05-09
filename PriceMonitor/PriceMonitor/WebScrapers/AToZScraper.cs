using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using PriceMonitor.ProductManager;

namespace PriceMonitor.WebScrapers
{
    public class AToZScraper : ScrapingManager
    {
        public AToZScraper()
        {
            _url = "https://www.atoz.com.mt/";
        }

        public override List<Item> Search(string toSearch, string category = null)
        {
            _driver.Navigate().GoToUrl(_url);

            try
            {
                _driver.FindElementById("search").SendKeys(toSearch);
                _driver.FindElementByClassName("search__submit").Click();

                IWebElement mainDiv = _driver.FindElementById("content_area");
                _items = mainDiv.FindElements(By.XPath(".//a[contains(@class,'productnamecolor colors_productname')]"));

                List<string> links = GetHyperLinks(_items);
                foreach (var link in links)
                {
                    _driver.Navigate().GoToUrl(link);
                    Thread.Sleep(1500);

                    string title = _driver.FindElement(By.XPath(".//span[@itemprop='name']")).Text;
                    string price = _driver.FindElement(By.ClassName("product_listprice")).Text;
                    _productsDetails.Add(new Item(title, price, null, link, Web.AtoZ, category));
                }
            } catch { }

            _driver.Dispose();
            return _productsDetails;
        }
    }
}