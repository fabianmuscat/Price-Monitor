using System.Collections.Generic;
using System.Threading;
using OpenQA.Selenium;
using PriceMonitor.ProductManager;

namespace PriceMonitor.WebScrapers
{
    public class NewEggScraper : ScrapingManager
    {
        public NewEggScraper()
        {
            _url = "https://www.newegg.com/";
        }

        public override List<Item> Search(string toSearch, string category = null)
        {
            _driver.Navigate().GoToUrl(_url);

            try
            {
                _driver.FindElementById("SearchBox2020").SendKeys(toSearch);
                _driver.FindElementByXPath(".//button[@class='fas fa-search']").Click();

                Thread.Sleep(2000);                           
                _driver.FindElementByXPath(".//div[@id='onetrust-close-btn-container']//button[contains(@class,'onetrust-close-btn-handler onetrust-close-btn-ui banner-close-button onetrust-lg close-icon')]").Click(); //This closes the pop up

                IWebElement mainDiv = _driver.FindElementByXPath(".//div[@class='row-body-border']");
                _items = mainDiv.FindElements(By.XPath(".//div[contains(@class,'items-view is-grid')]//div[@class='item-container      ']"));

                foreach (var item in _items)
                {
                    string title = item.FindElement(By.XPath(".//a[@class='item-title']")).Text;
                    string price = item.FindElement(By.XPath(".//li[@class='price-current']")).FindElement(By.TagName("strong")).Text;
                    string href = item.FindElement(By.XPath(".//a[@class='item-title']")).GetAttribute("href");
                    _productsDetails.Add(new Item(title, price, null, href, Web.NewEgg, category));
                }
            } catch { }

            _driver.Dispose();
            return _productsDetails;
        }
    }
}