using System;

namespace graphql_aspnet_core.Data
{
    public partial class Stock
    {
        public int ID { get; set; }
        public decimal Close { get; set; }
        public DateTime Date { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal Open { get; set; }
        public string Symbol { get; set; }
        public long Volume { get; set; }
    }
}
