using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace signalr_for_aspnet_core.Models
{
    public class ProductSignalR
    {
        public long? ID { get; set; }
        public string ProductName { get; set; }
        public double UnitPrice { get; set; }
        public int UnitsInStock { get; set; }
        public DateTime? CreatedAt { get; set; }
        public CategorySignalR Category { get; set; }
    }
}