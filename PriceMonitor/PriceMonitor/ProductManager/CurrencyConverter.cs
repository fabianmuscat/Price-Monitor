using System;
using PriceMonitor.WebScrapers;

namespace PriceMonitor.ProductManager
{
    public class CurrencyConverter
    {
        private static readonly CurrencyRateScraper RateScraper = new CurrencyRateScraper();

        public decimal ConvertToEuros(decimal price, Currency currency)
        {
            double dPrice = Convert.ToDouble(price);

            double euroPrice =
                (currency == Currency.Gbp) ? FromGBP(dPrice) :
                (currency == Currency.Usd) ? FromUSD(dPrice) : 0;

            return Math.Round(Convert.ToDecimal(euroPrice), 2);
        }

        private double FromGBP(double price) => price * RateScraper.GbpRate;

        private double FromUSD(double price) => price * RateScraper.UsdRate;
    }
}