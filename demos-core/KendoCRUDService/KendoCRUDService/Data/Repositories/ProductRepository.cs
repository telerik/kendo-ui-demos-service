using KendoCRUDService.Data.Models;
using KendoCRUDService.SessionExtensions;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace KendoCRUDService.Data.Repositories
{
    public class ProductRepository
    {
        private readonly ISession _session;
        private readonly IServiceScopeFactory _scopeFactory;
        private IHttpContextAccessor _contextAccessor;
        private ConcurrentDictionary<string, IList<Product>> _productsByUsers;

        public ProductRepository(IHttpContextAccessor httpContextAccessor, IServiceScopeFactory scopeFactory)
        {
            _session = httpContextAccessor.HttpContext.Session;
            _scopeFactory = scopeFactory;
            _contextAccessor = httpContextAccessor;
            _productsByUsers = new ConcurrentDictionary<string, IList<Product>>();
        }

        public IList<Product> All()
        {
            var userKey = SessionUtils.GetUserKey(_contextAccessor);

            return _productsByUsers.GetOrAdd(userKey, key =>
            {
                using (var scope = _scopeFactory.CreateScope())
                {
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
                }
            });
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
