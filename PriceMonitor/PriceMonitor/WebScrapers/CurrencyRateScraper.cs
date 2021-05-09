using System.IO;
using System.Threading;
using OpenQA.Selenium.Chrome;

namespace PriceMonitor.WebScrapers
{
    public class CurrencyRateScraper
    {
        private ChromeDriver _driver;
        private string _url;

        public double UsdRate { get; }
        public double GbpRate { get; }

        public CurrencyRateScraper()
        {
            InitialiseChromeDriver();
            UsdRate = Scrape("USD");
            GbpRate = Scrape("GBP");
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

        private double Scrape(string format)
        {
            double rate;
            _url = $"https://www.xe.com/currencyconverter/convert/?Amount=1&From={format}&To=EUR";
            _driver.Navigate().GoToUrl(_url);

            Thread.Sleep(1000);
            string rateDiv = _driver.FindElementByXPath(".//span[@class='converterresult-toAmount']").Text;
            double.TryParse(rateDiv, out rate);

            return rate;
        }
    }
}