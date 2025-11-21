using KendoCRUDService.Data.Models;
using KendoCRUDService.Extensions;
using KendoCRUDService.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace KendoCRUDService.Data.Repositories
{
    public class DetailProductRepository
    {
        private readonly ISession _session;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IUserDataCache _userCache;
        private readonly ILogger<DetailProductRepository> _logger;

        private static readonly TimeSpan Ttl = TimeSpan.FromMinutes(15);
        private const string LogicalName = "DetailProducts";
        private IHttpContextAccessor _contextAccessor;

        public DetailProductRepository(IHttpContextAccessor httpContextAccessor, IServiceScopeFactory scopeFactory, IUserDataCache userCache,
            ILogger<DetailProductRepository> logger)
        {
            _session = httpContextAccessor.HttpContext.Session;
            _contextAccessor = httpContextAccessor;
            _scopeFactory = scopeFactory;
            _userCache = userCache;
            _logger = logger;
        }

        public IList<DetailProduct> All()
        {
            var userKey = SessionUtils.GetUserKey(_contextAccessor);

            return _userCache.GetOrCreateList<DetailProduct>(userKey, LogicalName, () =>
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
            },Ttl, sliding: true);
        }

        private void UpdateContent(List<DetailProduct> entries)
        {
            var userKey = SessionUtils.GetUserKey(_contextAccessor);
            _userCache.GetOrCreateList<DetailProduct>(
                userKey,
                LogicalName,
                () =>
                {
                    return entries;
                },
                Ttl,
                sliding: true
            );
        }

        public DetailProduct One(Func<DetailProduct, bool> predicate)
        {
            return All().FirstOrDefault(predicate);
        }

        public void Insert(DetailProduct product)
        {
            var entries = All().ToList();
            var first = entries.OrderByDescending(p => p.ProductID).FirstOrDefault();
            if (first != null)
            {
                product.ProductID = first.ProductID + 1;
            }
            else
            {
                product.ProductID = 0;
            }

            entries.Insert(0, product);
            UpdateContent(entries);
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
                var entries = All().ToList();
                entries.Remove(target);
                UpdateContent(entries);
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
