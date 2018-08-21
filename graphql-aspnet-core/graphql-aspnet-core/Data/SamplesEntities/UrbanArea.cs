using System.ComponentModel.DataAnnotations.Schema;

namespace graphql_aspnet_core.Data
{
    public partial class UrbanArea
    {
        public int ID { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Country_ISO3 { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public long Pop1950 { get; set; }
        public long Pop1955 { get; set; }
        public long Pop1960 { get; set; }
        public long Pop1965 { get; set; }
        public long Pop1970 { get; set; }
        public long Pop1975 { get; set; }
        public long Pop1980 { get; set; }
        public long Pop1985 { get; set; }
        public long Pop1990 { get; set; }
        public long Pop1995 { get; set; }
        public long Pop2000 { get; set; }
        public long Pop2005 { get; set; }
        public long Pop2010 { get; set; }
        public long Pop2015 { get; set; }
        public long Pop2020 { get; set; }
        public long Pop2025 { get; set; }
        public long Pop2050 { get; set; }

        [NotMapped]
        public double[] Location
        {
            get
            {
                return new double[] { Latitude, Longitude };
            }

            private set { }
        }
    }
}
