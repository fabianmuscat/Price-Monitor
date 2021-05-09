using System.Collections.Generic;
using System.IO;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using PriceMonitor.ProductManager;

namespace PriceMonitor.WebScrapers
{
    public class GearBestScraper : ScrapingManager
    {
        public GearBestScraper()
        {
            _driver.Dispose();
            ChromeDriverService service = ChromeDriverService.CreateDefaultService(Directory.GetCurrentDirectory());
            ChromeOptions options = new ChromeOptions();
            service.SuppressInitialDiagnosticInformation = true;
            service.HideCommandPromptWindow = true;
            _driver = new ChromeDriver(service, options);
            _url = "https://www.gearbest.com/";
        }

        public override List<Item> Search(string toSearch, string category = null)
        {
            _driver.Navigate().GoToUrl(_url);
            try
            {
                string categoryID = SetCategory(category);
                if (categoryID != null)
                {
                    _url = categoryID.Equals("0") ? $"{_url}sale/{toSearch}" : $"{_url}c_{categoryID}/{toSearch}";
                    _driver.Navigate().GoToUrl(_url);
                }
                else
                {
                    _driver.FindElementById("js-iptKeyword").SendKeys(toSearch);
                    _driver.FindElementById("js-iptKeyword").SendKeys(Keys.Enter);
                }

                IWebElement mainDiv = _driver.FindElementByXPath(".//div[contains(@class,'cateMain_specialBox')]//div[contains(@class,'cateMain_listModel')]");
                _items = mainDiv.FindElements(By.XPath(".//div[@class='gbGoodsItem_outBox']"));

                foreach (var item in _items)
                {
                    try
                    {
                        string title = item.FindElement(By.XPath(".//p[@class='gbGoodsItem_titleInfor']")).Text;
                        string price = item.FindElement(By.XPath(".//p[contains(@class,'gbGoodsItem_price')]")).Text;
                        string href = item
                            .FindElement(By.XPath(".//p[@class='gbGoodsItem_titleInfor']//a[contains(@class,'gbGoodsItem_title')]"))
                            .GetAttribute("href");
                        _productsDetails.Add(new Item(title, price, null, href, Web.GearBest, category));
                    }
                    catch { }
                }
            } catch { }

            _driver.Dispose();
            return _productsDetails;
        }

        private string SetCategory(string categoryToSelect)
        {
            IWebElement categorySelect = _driver.FindElementById("js-selSearchCateList");
            IReadOnlyCollection<IWebElement> categories = categorySelect.FindElements(By.TagName("option"));

            string categoryID = null;
            foreach (var category in categories)
            {
                if (category.GetAttribute("innerText").Equals(categoryToSelect))
                {
                    categoryID = category.GetAttribute("value");
                    break;
                }
            }

            return categoryID;
        }

        public override List<string> GetCategories()
        {
            _driver.Navigate().GoToUrl(_url);

            IWebElement categorySelect = _driver.FindElementById("js-selSearchCateList");
            IReadOnlyCollection<IWebElement> categories = categorySelect.FindElements(By.TagName("option"));

            foreach (var category in categories)
                _categories.Add(category.GetAttribute("innerText"));

            return _categories;
        }
    }
}