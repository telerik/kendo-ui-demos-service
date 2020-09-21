using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KendoCRUDService.Models
{
    public static class DetailProductRepository
    {
        public static IList<DetailProductModel> All()
        {
            var result = HttpContext.Current.Session["DetailProducts"] as IList<DetailProductModel>;

            if (result == null)
            {
                HttpContext.Current.Session["DetailProducts"] = result =
                    new SampleDataContext().DetailProducts.Select(p => new DetailProductModel
                    {
                        ProductID = p.ProductID,
                        ProductName = p.ProductName,
                        UnitPrice = (decimal)p.UnitPrice.GetValueOrDefault(),
                        UnitsInStock = p.UnitsInStock.GetValueOrDefault(),
                        Discontinued = p.Discontinued,
                        Category = new CategoryModel() { CategoryID = p.Category.CategoryID, CategoryName = p.Category.CategoryName },
                        Country = new CountryModel() { CountryID = p.Country.CountryID, CountryNameShort = p.Country.CountryNameShort, CountryNameLong = p.Country.CountryNameLong },
                        CategoryID = p.CategoryID,
                        CountryID = p.CountryID,
                        CustomerRating = p.CustomerRating,
                        LastSupply = p.LastSupply,
                        QuantityPerUnit = p.QuantityPerUnit,
                        TargetSales = p.TargetSales,
                        UnitsOnOrder = p.UnitsOnOrder
                    }).ToList();
            }

            return result;
        }

        public static DetailProductModel One(Func<DetailProductModel, bool> predicate)
        {
            return All().FirstOrDefault(predicate);
        }

        public static void Insert(DetailProductModel product)
        {
            var first = All().OrderByDescending(p => p.ProductID).FirstOrDefault();
            if (first != null)
            {
                product.ProductID = first.ProductID + 1;
            }
            else
            {
                product.ProductID = 0;
            }

            All().Insert(0, product);
        }

        public static void Insert(IEnumerable<DetailProductModel> products)
        {
            foreach (var product in products)
            {
                Insert(product);
            }
        }

        public static void Update(DetailProductModel product)
        {
            var target = One(p => p.ProductID == product.ProductID);
            if (target != null)
            {
                target.ProductID = product.ProductID;
                target.ProductName = product.ProductName;
                target.UnitPrice = (decimal)product.UnitPrice;
                target.UnitsInStock = product.UnitsInStock;
                target.Discontinued = product.Discontinued;
                target.Category = new CategoryModel() { CategoryID = product.Category.CategoryID, CategoryName = product.Category.CategoryName };
                target.Country = new CountryModel() { CountryID = product.Country.CountryID, CountryNameShort = product.Country.CountryNameShort, CountryNameLong = product.Country.CountryNameLong };
                target.CategoryID = product.CategoryID;
                target.CountryID = product.CountryID;
                target.CustomerRating = product.CustomerRating;
                target.LastSupply = product.LastSupply;
                target.QuantityPerUnit = product.QuantityPerUnit;
                target.TargetSales = product.TargetSales;
                target.UnitsOnOrder = product.UnitsOnOrder;
            }
        }

        public static void Update(IEnumerable<DetailProductModel> products)
        {
            foreach (var product in products)
            {
                Update(product);
            }
        }

        public static void Delete(DetailProductModel product)
        {
            var target = One(p => p.ProductID == product.ProductID);
            if (target != null)
            {
                All().Remove(target);
            }
        }

        public static void Delete(IEnumerable<DetailProductModel> products)
        {
            foreach (var product in products)
            {
                Delete(product);
            }
        }
    }
}