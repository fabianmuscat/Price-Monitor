using System.Collections.Generic;
using System.IO;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using PriceMonitor.ProductManager;

namespace PriceMonitor.WebScrapers
{
    public abstract class ScrapingManager
    {
        protected string _url;
        protected ChromeDriver _driver;
        protected List<Item> _productsDetails;
        protected List<string> _categories;
        protected IReadOnlyCollection<IWebElement> _items;

        protected ScrapingManager()
        {
            _url = string.Empty;
            _productsDetails = new List<Item>();
            _categories = new List<string>();
            _items = new List<IWebElement>();
            InitialiseChromeDriver();
        }

        private void InitialiseChromeDriver()
        {
            ChromeDriverService service = ChromeDriverService.CreateDefaultService(Directory.GetCurrentDirectory());
            ChromeOptions options = new ChromeOptions();

            service.SuppressInitialDiagnosticInformation = true;
            service.HideCommandPromptWindow = true;
            options.AddArgument("--headless");

            _driver = new ChromeDriver(service, options);
        }

        public abstract List<Item> Search(string toSearch, string category = null);

        public virtual List<string> GetCategories() => null;

        protected List<string> GetHyperLinks(IReadOnlyCollection<IWebElement> items, string className = null)
        {
            List<string> links = new List<string>();
            foreach (var item in items)
            {
                string link = string.Empty;
                if (className != null) link = item.FindElement(By.ClassName(className)).GetAttribute("href");
                else link = item.GetAttribute("href");

                links.Add(link);
            }

            return links;
        }
    }
}
