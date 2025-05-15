using KendoCRUDService.Data;
using KendoCRUDService.Data.Models;
using KendoCRUDService.Extensions;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace KendoCRUDService.Hubs
{
    public class ProductHub : BaseHub
    {

        private readonly ISession _session;
        private readonly IDbContextFactory<DemoDbContext> _contextFactory;

        public ProductHub(IHttpContextAccessor httpContextAccessor, IDbContextFactory<DemoDbContext> contextFactory)
        {
            _session = httpContextAccessor.HttpContext.Session;
            _contextFactory = contextFactory;
        }

        public IEnumerable<ProductSignalR> Read()
        {
            var products = _session.GetObjectFromJson<IEnumerable<ProductSignalR>>("products");

            if (products == null)
            {
                using (var context = _contextFactory.CreateDbContext())
                {
                    var createdAt = DateTime.Now;

                    products = context.Products
                                      .OrderBy(p => p.ProductName)
                                      .ToList() // Execute the query because Linq to SQL doesn't get Guid.NewGuid()
                                      .Select(p => new ProductSignalR
                                      {
                                          ID = Guid.NewGuid(),
                                          ProductName = p.ProductName,
                                          UnitPrice = (double)p.UnitPrice.GetValueOrDefault(),
                                          UnitsInStock = p.UnitsInStock.GetValueOrDefault(),
                                          CreatedAt = createdAt = createdAt.AddMilliseconds(1)
                                      })
                                      .ToList();
                }

                _session.SetObjectAsJson("products", products);
            }

            return products;
        }

        public void Update(ProductSignalR product)
        {
            Clients.OthersInGroup(GetGroupName()).SendAsync("update", product);
        }

        public void Destroy(ProductSignalR product)
        {
            Clients.OthersInGroup(GetGroupName()).SendAsync("destroy", product);
        }

        public ProductSignalR Create(ProductSignalR product)
        {
            product.ID = Guid.NewGuid();
            product.CreatedAt = DateTime.Now;

            Clients.OthersInGroup(GetGroupName()).SendAsync("create", product);

            return product;
        }
    }
}
