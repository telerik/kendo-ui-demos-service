namespace KendoCRUDService.Data.Models
{
    public class ProductSignalR
    {
        public Guid? ID { get; set; }
        public string ProductName { get; set; }
        public double UnitPrice { get; set; }
        public int UnitsInStock { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
