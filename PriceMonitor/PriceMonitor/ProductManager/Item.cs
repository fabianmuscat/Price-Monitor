using System;
using System.Linq;
using System.Text.RegularExpressions;
using PriceMonitor.WebScrapers;

namespace PriceMonitor.ProductManager
{
    public class Item
    {
        private readonly Regex _regex;
        private readonly CurrencyConverter _currencyConverter;
        private readonly Type _type;

        public string Title { get; private set; }
        public decimal Price { get; private set; }
        public decimal Shipping { get; private set; }
        public decimal Total { get; private set; }
        public string Url { get; private set; }
        public string Website { get; private set; }
        public string Category { get; private set; }

        public Item(string title, string price, string shipping, string url, object website, string category)
        {
            char priceSymbol = GetCurrencySymbol(price);
            char shippingSymbol = char.MinValue;
            if (shipping != null)
                shippingSymbol = GetCurrencySymbol(shipping);

            if (((priceSymbol == '$' || priceSymbol == '£') && (shippingSymbol =='$' || shippingSymbol == '£')) || shipping == null)
            {
                _currencyConverter = new CurrencyConverter();
                _regex = new Regex("[^0-9]");
                _type = website.GetType();

                decimal p = ConvertPrice(price, priceSymbol);
                decimal s = (shipping == null) ? 0 : ConvertPrice(shipping, shippingSymbol);
                string web = _type.IsEnum ? GetWebsite((Web)website) : website as string;

                SetProperties(title, p, s, url, web, category);
            }
            else
                SetProperties(title, Convert.ToDecimal(price), Convert.ToDecimal(shipping), url, website as string, category);
        }

        private void SetProperties(string title, decimal price, decimal shipping, string url, string website, string category)
        {
            Title = title;
            Price = price;
            Shipping = shipping;
            Total = Price + Shipping;
            Url = url;
            Website = website;
            Category = category;
        }

        private string GetWebsite(Web website)
        {
            string name = string.Empty;
            switch (website)
            {
                case Web.Amazon: name = "Amazon"; break;
                case Web.Ebay: name = "Ebay"; break;
                case Web.AtoZ: name = "AtoZ"; break;
                case Web.Walmart: name = "Walmart"; break;
                case Web.NewEgg: name = "NewEgg"; break;
                case Web.MicroCenter: name = "MicroCenter"; break;
                case Web.GearBest: name = "GearBest"; break;
                case Web.Scan: name = "Scan"; break;
            }

            return name;
        }

        private char GetCurrencySymbol(string price) => price.FirstOrDefault(format => format.Equals('$') || format.Equals('£'));

        private decimal ConvertPrice(string price, char symbol)
        {
            decimal ConvertCurrency(decimal convertedPrice, char format)
            {
                return (format == '$') ? _currencyConverter.ConvertToEuros(convertedPrice, Currency.Usd) :
                (format == '£') ? _currencyConverter.ConvertToEuros(convertedPrice, Currency.Gbp) : convertedPrice;
            }

            string strConvPrice = _regex.Replace(price, "");
            decimal.TryParse(strConvPrice, out var convPrice);
            convPrice /= 100;

            return ConvertCurrency(convPrice, symbol);
        }
    }
}