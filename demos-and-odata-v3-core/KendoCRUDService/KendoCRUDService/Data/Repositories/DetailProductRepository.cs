using KendoCRUDService.Data.Models;
using KendoCRUDService.Models;
using KendoCRUDService.SessionExtensions;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;

namespace KendoCRUDService.Data.Repositories
{
    public class DetailProductRepository
    {
        private readonly ISession _session;
        private readonly IServiceScopeFactory _scopeFactory;
        private ConcurrentDictionary<string, IList<DetailProduct>> _detailProducts;
        private IHttpContextAccessor _contextAccessor;

        public DetailProductRepository(IHttpContextAccessor httpContextAccessor, IServiceScopeFactory scopeFactory)
        {
            _session = httpContextAccessor.HttpContext.Session;
            _contextAccessor = httpContextAccessor;
            _scopeFactory = scopeFactory;
        }

        public IList<DetailProduct> All()
        {
            var userKey = SessionUtils.GetUserKey(_contextAccessor);

            return _detailProducts.GetOrAdd(userKey, key =>
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<DemoDbContext>();
                    return context.DetailProducts.Select(p => new DetailProduct
                    {
                        ProductID = p.ProductID,
                        ProductName = p.ProductName,
                        UnitPrice = (decimal)p.UnitPrice.GetValueOrDefault(),
                        UnitsInStock = p.UnitsInStock.GetValueOrDefault(),
                        Discontinued = p.Discontinued,
                        Category = new Category() { CategoryID = p.Category.CategoryID, CategoryName = p.Category.CategoryName },
                        Country = new Country() { CountryID = p.Country.CountryID, CountryNameShort = p.Country.CountryNameShort, CountryNameLong = p.Country.CountryNameLong },
                        CategoryID = p.CategoryID,
                        CountryID = p.CountryID,
                        CustomerRating = p.CustomerRating,
                        LastSupply = p.LastSupply,
                        QuantityPerUnit = p.QuantityPerUnit,
                        TargetSales = p.TargetSales,
                        UnitsOnOrder = p.UnitsOnOrder
                    }).ToList();
                }
            });
        }

        public DetailProduct One(Func<DetailProduct, bool> predicate)
        {
            return All().FirstOrDefault(predicate);
        }

        public void Insert(DetailProduct product)
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

        public void Insert(IEnumerable<DetailProduct> products)
        {
            foreach (var product in products)
            {
                Insert(product);
            }
        }

        public void Update(DetailProduct product)
        {
            var target = One(p => p.ProductID == product.ProductID);
            if (target != null)
            {
                target.ProductID = product.ProductID;
                target.ProductName = product.ProductName;
                target.UnitPrice = (decimal)product.UnitPrice;
                target.UnitsInStock = product.UnitsInStock;
                target.Discontinued = product.Discontinued;
                target.Category = new Category() { CategoryID = product.Category.CategoryID, CategoryName = product.Category.CategoryName };
                target.Country = new Country() { CountryID = product.Country.CountryID, CountryNameShort = product.Country.CountryNameShort, CountryNameLong = product.Country.CountryNameLong };
                target.CategoryID = product.CategoryID;
                target.CountryID = product.CountryID;
                target.CustomerRating = product.CustomerRating;
                target.LastSupply = product.LastSupply;
                target.QuantityPerUnit = product.QuantityPerUnit;
                target.TargetSales = product.TargetSales;
                target.UnitsOnOrder = product.UnitsOnOrder;
            }
        }

        public void Update(IEnumerable<DetailProduct> products)
        {
            foreach (var product in products)
            {
                Update(product);
            }
        }

        public void Delete(DetailProduct product)
        {
            var target = One(p => p.ProductID == product.ProductID);
            if (target != null)
            {
                All().Remove(target);
            }
        }

        public void Delete(IEnumerable<DetailProduct> products)
        {
            foreach (var product in products)
            {
                Delete(product);
            }
        }
    }
}
