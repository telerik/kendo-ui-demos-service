namespace KendoCRUDService.Data.Models
{
    public partial class Country
    {
        public Country()
        {
            DetailProducts = new HashSet<DetailProduct>();
        }

        public int CountryID { get; set; }
        public string CountryNameShort { get; set; }
        public string CountryNameLong { get; set; }

        public virtual ICollection<DetailProduct> DetailProducts { get; set; }
    }
}
