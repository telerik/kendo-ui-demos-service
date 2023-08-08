using System.ComponentModel.DataAnnotations.Schema;

namespace KendoCRUDService.Data.Models
{
    public partial class DetailProduct
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public decimal? UnitPrice { get; set; }
        public int? UnitsInStock { get; set; }
        public string QuantityPerUnit { get; set; }
        public bool Discontinued { get; set; }
        public int? UnitsOnOrder { get; set; }
        public int? CategoryID { get; set; }
        public int? CountryID { get; set; }
        public int CustomerRating { get; set; }
        public int TargetSales { get; set; }
        public DateTime LastSupply { get; set; }

        [ForeignKey(nameof(CategoryID))]
        [InverseProperty("DetailProducts")]
        public virtual Category Category { get; set; }

        [ForeignKey(nameof(CountryID))]
        [InverseProperty("DetailProducts")]
        public virtual Country Country { get; set; }
    }
}
