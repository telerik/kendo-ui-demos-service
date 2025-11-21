using KendoCRUDService.Data.Models;
using KendoCRUDService.Extensions;
using KendoCRUDService.Models;
using Microsoft.EntityFrameworkCore;

namespace KendoCRUDService.Data.Repositories
{
    public class ProductRepository
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IUserDataCache _userCache;
        private readonly ILogger<ProductRepository> _logger;

        private static readonly TimeSpan Ttl = TimeSpan.FromMinutes(15);
        private const string LogicalName = "products";

        public ProductRepository(
            IHttpContextAccessor httpContextAccessor,
            IServiceScopeFactory scopeFactory,
            IUserDataCache userCache,
            ILogger<ProductRepository> logger)
        {
            _scopeFactory = scopeFactory;
            _contextAccessor = httpContextAccessor;
            _userCache = userCache;
            _logger = logger;
        }

        public IList<Product> All()
        {
            var userKey = SessionUtils.GetUserKey(_contextAccessor);
            return _userCache.GetOrCreateList<Product>(
                userKey,
                LogicalName,
                () =>
                {
                    using var scope = _scopeFactory.CreateScope();
                    var context = scope.ServiceProvider.GetRequiredService<DemoDbContext>();
                    return context.Products.Select(p => new Product
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
                },
                Ttl,
                sliding: true
            );
        }

        public Product One(Func<Product, bool> predicate)
        {
            return All().FirstOrDefault(predicate);
        }

        private void UpdateContent(List<Product> entries)
        {
            var userKey = SessionUtils.GetUserKey(_contextAccessor);
            _userCache.GetOrCreateList<Product>(
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

        public void Insert(Product product)
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
                var entries = All().ToList();
                entries.Remove(target);
                UpdateContent(entries);
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
