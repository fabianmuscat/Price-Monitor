using System;

namespace PriceMonitor.ProductManager
{
    public class UserItem 
    {
        public string Title { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Shipping { get; set; }
        public decimal Total { get; set; }
        public string Website{ get; set; }
        public string Url { get; set; }
        public string DateAdded { get; set; }

        public UserItem(string title, decimal subtotal, decimal? shipping, string website, string url, string dateAdded)
        {
            Title = title;
            Subtotal = subtotal;
            Shipping = (shipping != null) ? (decimal) shipping : 0;
            Total = Subtotal + Shipping;
            Website = website;
            Url = url;
            DateAdded = dateAdded;
        }
    }
}