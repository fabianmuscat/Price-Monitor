//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PriceMonitor.EntityFramework
{
    using System;
    using System.Collections.Generic;
    
    public partial class MonitoredProductsView
    {
        public string Product { get; set; }
        public decimal Subtotal { get; set; }
        public Nullable<decimal> Shipping { get; set; }
        public string Obtained_From { get; set; }
        public string Link { get; set; }
        public string Account { get; set; }
        public string Date_Monitored { get; set; }
    }
}
