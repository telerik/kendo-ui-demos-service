using kendo_northwind_pg.Data.Models;
using kendo_northwind_pg.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;

namespace kendo_northwind_pg.Data.Repositories
{
    public class ProductRepository
    {
        private readonly ISession _session;
        private readonly IServiceScopeFactory _scopeFactory;
        private IHttpContextAccessor _contextAccessor;
        private readonly IUserDataCache _userCache;
        private readonly ILogger<ProductRepository> _logger;

        private static readonly TimeSpan Ttl = TimeSpan.FromMinutes(15);
        private const string LogicalName = "Products";


        public ProductRepository(IHttpContextAccessor httpContextAccessor, IServiceScopeFactory scopeFactory, IUserDataCache userCache,
            ILogger<ProductRepository> logger)
        {
            _session = httpContextAccessor.HttpContext.Session;
            _contextAccessor = httpContextAccessor;
            _scopeFactory = scopeFactory;
            _userCache = userCache;
            _logger = logger;
        }

        public IList<Product> All()
        {
            var userKey = SessionUtils.GetUserKey(_contextAccessor);

            return _userCache.GetOrCreateList<Product>(userKey, LogicalName, () =>
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<DemoDbContext>();
                    return context.Products
                    .Include(p => p.OrderDetails)
                    .ThenInclude(od => od.Order).Select(p => new Product
                    {
                        ProductID = p.ProductID,
                        ProductName = p.ProductName,
                        UnitPrice = p.UnitPrice,
                        UnitsInStock = p.UnitsInStock.GetValueOrDefault(),
                        Discontinued = p.Discontinued,
                        CategoryID = p.CategoryID.GetValueOrDefault(),
                        UnitsOnOrder = p.UnitsOnOrder.GetValueOrDefault(),
                        ReorderLevel = p.ReorderLevel.GetValueOrDefault(),
                        SupplierID = p.SupplierID.GetValueOrDefault(),
                        OrderDetails = p.OrderDetails
                    }).ToList();
                }

            }, Ttl, sliding: true);
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

        public Product One(Func<Product, bool> predicate)
        {
            return All().FirstOrDefault(predicate);
        }

        public IEnumerable<Product> Where(Func<Product, bool> predicate)
        {
            return All().Where(predicate);
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
                target.UnitsOnOrder = product.UnitsOnOrder;
                target.CategoryID = product.CategoryID;
                target.SupplierID = product.SupplierID;
                target.ReorderLevel = product.ReorderLevel;
                target.Discontinued = product.Discontinued;
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
