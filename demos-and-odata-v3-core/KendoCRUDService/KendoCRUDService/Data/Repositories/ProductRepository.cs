using KendoCRUDService.Data.Models;
using KendoCRUDService.SessionExtensions;
using Microsoft.EntityFrameworkCore;

namespace KendoCRUDService.Data.Repositories
{
    public class ProductRepository
    {
        private readonly ISession _session;
        private readonly IDbContextFactory<DemoDbContext> _contextFactory;

        public ProductRepository(IHttpContextAccessor httpContextAccessor, IDbContextFactory<DemoDbContext> contextFactory)
        {
            _session = httpContextAccessor.HttpContext.Session;
            _contextFactory = contextFactory;
        }

        public IList<Product> All()
        {
            var result = _session.GetObjectFromJson<IList<Product>>("Products"); //HttpContext.Current.Session["Products"] as IList<ProductModel>;

            if (result == null)
            {
                result =
                    _contextFactory.CreateDbContext().Products.Select(p => new Product
                    {
                        ProductID = p.ProductID,
                        ProductName = p.ProductName,
                        UnitPrice = p.UnitPrice.GetValueOrDefault(),
                        UnitsInStock = p.UnitsInStock.GetValueOrDefault(),
                        UnitsOnOrder = p.UnitsOnOrder.GetValueOrDefault(),
                        CategoryID = p.CategoryID.GetValueOrDefault(),
                        ReorderLevel = p.ReorderLevel.GetValueOrDefault(),
                        SupplierID = p.SupplierID.GetValueOrDefault(),
                        QuantityPerUnit = p.QuantityPerUnit,
                        Discontinued = p.Discontinued
                    }).ToList();

                _session.SetObjectAsJson("Products", result);
            }

            return result;
        }

        public Product One(Func<Product, bool> predicate)
        {
            return All().FirstOrDefault(predicate);
        }

        public void Insert(Product product)
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

        public void Insert(IEnumerable<Product> products)
        {
            foreach (var product in products)
            {
                Insert(product);
            }
        }

        public void Update(Product product)
        {
            var target = One(p => p.ProductID == product.ProductID);
            if (target != null)
            {
                target.ProductName = product.ProductName;
                target.UnitPrice = product.UnitPrice;
                target.UnitsInStock = product.UnitsInStock;
                target.Discontinued = product.Discontinued;
                target.QuantityPerUnit = product.QuantityPerUnit;
                target.ReorderLevel = product.ReorderLevel;
                target.SupplierID = product.SupplierID;
                target.CategoryID = product.CategoryID;
                target.UnitsOnOrder = product.UnitsOnOrder;
            }
        }

        public void Update(IEnumerable<Product> products)
        {
            foreach (var product in products)
            {
                Update(product);
            }
        }

        public void Delete(Product product)
        {
            var target = One(p => p.ProductID == product.ProductID);
            if (target != null)
            {
                All().Remove(target);
            }
        }

        public void Delete(IEnumerable<Product> products)
        {
            foreach (var product in products)
            {
                Delete(product);
            }
        }
    }
}
