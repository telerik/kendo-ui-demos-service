using System;
using System.Collections.Generic;

namespace graphql_aspnet_core.Data
{
    public partial class Order
    {
        public Order()
        {
            OrderDetails = new HashSet<OrderDetail>();
        }

        public int OrderID { get; set; }
        public string CustomerID { get; set; }
        public int? EmployeeID { get; set; }
        public decimal? Freight { get; set; }
        public DateTime? OrderDate { get; set; }
        public string RequiredDate { get; set; }
        public string ShipAddress { get; set; }
        public string ShipCity { get; set; }
        public string ShipCountry { get; set; }
        public string ShipName { get; set; }
        public DateTime? ShippedDate { get; set; }
        public string ShipPostalCode { get; set; }
        public string ShipRegion { get; set; }
        public int? ShipVia { get; set; }

        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        public virtual Customer Customer { get; set; }
        public virtual Employee Employee { get; set; }
        public virtual Shipper ShipViaNavigation { get; set; }
    }
}
