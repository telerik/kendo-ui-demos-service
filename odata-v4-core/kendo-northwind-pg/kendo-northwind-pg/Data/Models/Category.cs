﻿namespace kendo_northwind_pg.Data.Models
{
    public partial class Category
    {
        public int CategoryID { get; set; }
        public string CategoryName { get; set; }
        public string Description { get; set; }
        public byte[] Picture { get; set; }

        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
        public virtual ICollection<DetailProduct> DetailProducts { get; set; } = new List<DetailProduct>();
    }
}
