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
    
    public partial class Product
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Product()
        {
            this.MonitorProducts = new HashSet<MonitorProduct>();
        }
    
        public int ProductID { get; set; }
        public string ProductTitle { get; set; }
        public decimal Price { get; set; }
        public Nullable<decimal> Shipping { get; set; }
        public string URL { get; set; }
        public Nullable<int> CategoryID { get; set; }
        public Nullable<int> SearchID { get; set; }
        public Nullable<int> WebsiteID { get; set; }
    
        public virtual Category Category { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MonitorProduct> MonitorProducts { get; set; }
        public virtual Website Website { get; set; }
        public virtual Search Search { get; set; }
    }
}