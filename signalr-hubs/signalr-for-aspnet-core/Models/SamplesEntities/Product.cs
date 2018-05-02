using System;
using System.Collections.Generic;

namespace signalr_for_aspnet_core.Models
{
    public partial class Product
    {
        public Product()
        {
            OrderDetails = new HashSet<OrderDetail>();
        }

        public int ProductID { get; set; }
        public int? CategoryID { get; set; }
        public bool Discontinued { get; set; }
        public string ProductName { get; set; }
        public string QuantityPerUnit { get; set; }
        public int? ReorderLevel { get; set; }
        public int? SupplierID { get; set; }
        public decimal? UnitPrice { get; set; }
        public int? UnitsInStock { get; set; }
        public int? UnitsOnOrder { get; set; }

        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        public virtual Category Category { get; set; }
        public virtual Supplier Supplier { get; set; }
    }
}
