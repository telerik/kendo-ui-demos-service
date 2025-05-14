namespace KendoCRUDService.Data.Models
{
    public partial class Category
    {
        public Category()
        {
            DetailProducts = new HashSet<DetailProduct>();
            Products = new HashSet<Product>();
        }

        public int CategoryID { get; set; }
        public string CategoryName { get; set; }
        public string Description { get; set; }
        public byte[] Picture { get; set; }

        public virtual ICollection<DetailProduct> DetailProducts { get; set; }
        public virtual ICollection<Product> Products { get; set; }
    }
}
