namespace KendoCoreService.Models
{
    public class ProductModel
    {
        public int ProductID { get; set; }
        public string? ProductName { get; set; }
        public double UnitPrice { get; set; }
        public int UnitsInStock { get; set; }
        public bool Discontinued { get; set; }
    }
}
