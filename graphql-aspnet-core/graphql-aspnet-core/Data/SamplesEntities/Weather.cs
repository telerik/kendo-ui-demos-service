using System;

namespace graphql_aspnet_core.Data
{
    public partial class Weather
    {
        public int ID { get; set; }
        public string Station { get; set; }
        public DateTime Date { get; set; }
        public decimal TMax { get; set; }
        public decimal TMin { get; set; }
        public decimal Wind { get; set; }
        public decimal? Gust { get; set; }
        public decimal Rain { get; set; }
        public decimal? Snow { get; set; }
        public string Events { get; set; }
    }
}
