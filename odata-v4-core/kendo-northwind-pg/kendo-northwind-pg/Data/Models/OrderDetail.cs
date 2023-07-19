namespace kendo_northwind_pg.Data.Models
{
    public partial class OrderDetail
    {
        public int OrderID { get; set; }
        public int ProductID { get; set; }
        public double Discount { get; set; }
        public int Quantity { get; set; }
        public double UnitPrice { get; set; }

        public virtual Order Order { get; set; }
        public virtual Product Product { get; set; }
    }
}
