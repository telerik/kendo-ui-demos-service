using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KendoCRUDService.Models
{
    public class DetailProductModel
    {
        private int? targetSales;
        public int ProductID
        {
            get;
            set;
        }

        public string ProductName
        {
            get;
            set;
        }

        public decimal UnitPrice
        {
            get;
            set;
        }

        public int UnitsInStock
        {
            get;
            set;
        }

        public bool Discontinued
        {
            get;
            set;
        }

        public DateTime? LastSupply
        {
            get;
            set;
        }

        public int? UnitsOnOrder
        {
            get;
            set;
        }
        public CategoryModel Category
        {
            get;
            set;
        }

        public int? CategoryID { get; set; }
        public int? CountryID { get; set; }

        public string QuantityPerUnit { get; set; }
        public CountryModel Country { get; set; }
        public byte? CustomerRating { get; set; }
        public int? TargetSales
        {
            get
            {
                return targetSales;
            }
            set
            {
                targetSales = value;
                TotalSales = value * 100;
            }
        }
        public int? TotalSales { get; private set; }
    }
}