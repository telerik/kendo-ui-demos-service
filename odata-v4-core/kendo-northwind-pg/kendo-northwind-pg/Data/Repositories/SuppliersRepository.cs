using kendo_northwind_pg.Data.Models;
using kendo_northwind_pg.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Diagnostics.Metrics;
using System.Net;
using System.Numerics;

namespace kendo_northwind_pg.Data.Repositories
{
    public class SuppliersRepository
    {
        private readonly ISession _session;
        private readonly IServiceScopeFactory _scopeFactory;
        private IHttpContextAccessor _contextAccessor;
        private readonly IUserDataCache _userCache;
        private readonly ILogger<SuppliersRepository> _logger;

        private static readonly TimeSpan Ttl = TimeSpan.FromMinutes(15);
        private const string LogicalName = "Suppliers";


        public SuppliersRepository(IHttpContextAccessor httpContextAccessor, IServiceScopeFactory scopeFactory, IUserDataCache userCache,
            ILogger<SuppliersRepository> logger)
        {
            _session = httpContextAccessor.HttpContext.Session;
            _contextAccessor = httpContextAccessor;
            _scopeFactory = scopeFactory;
            _userCache = userCache;
            _logger = logger;
        }

        public IList<Supplier> All()
        {
            var userKey = SessionUtils.GetUserKey(_contextAccessor);

            return _userCache.GetOrCreateList<Supplier>(userKey,LogicalName, () =>
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<DemoDbContext>();
                    return context.Suppliers
                    .Include(p => p.Products).Select(p => new Supplier
                    {
                        SupplierID = p.SupplierID,
                        CompanyName = p.CompanyName,
                        ContactName = p.ContactName,
                        ContactTitle = p.ContactTitle,
                        Address = p.Address,
                        City = p.City,
                        Region = p.Region,
                        PostalCode = p.PostalCode,
                        Country = p.Country,
                        Phone = p.Phone,
                        Products = p.Products,
                        Fax = p.Fax,
                        HomePage = p.HomePage
                    }).ToList();
                }

            },Ttl, sliding: true);
        }

        public Supplier One(Func<Supplier, bool> predicate)
        {
            return All().FirstOrDefault(predicate);
        }

        public IEnumerable<Supplier> Where(Func<Supplier, bool> predicate)
        {
            return All().Where(predicate);
        }

        private void UpdateContent(List<Supplier> entries)
        {
            var userKey = SessionUtils.GetUserKey(_contextAccessor);
            _userCache.GetOrCreateList<Supplier>(
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

        public void Insert(Supplier Supplier)
        {
            var entries = All().ToList();
            var first = entries.OrderByDescending(p => p.SupplierID).FirstOrDefault();
            if (first != null)
            {
                Supplier.SupplierID = first.SupplierID + 1;
            }
            else
            {
                Supplier.SupplierID = 0;
            }

            entries.Insert(0, Supplier);
            UpdateContent(entries);
        }

        public void Insert(IEnumerable<Supplier> Suppliers)
        {
            foreach (var Supplier in Suppliers)
            {
                Insert(Supplier);
            }
        }

        public void Update(Supplier Supplier)
        {
            var target = One(p => p.SupplierID == Supplier.SupplierID);
            if (target != null)
            {
                target.CompanyName = Supplier.CompanyName;
                target.ContactName = Supplier.ContactName;
                target.ContactTitle = Supplier.ContactTitle;
                target.Address = Supplier.Address;
                target.City = Supplier.City;
                target.Region = Supplier.Region;
                target.PostalCode = Supplier.PostalCode;
                target.Country = Supplier.Country;
                target.Phone = Supplier.Phone;
                target.Fax = Supplier.Fax;
                target.HomePage = Supplier.HomePage;
            }
        }

        public void Update(IEnumerable<Supplier> Suppliers)
        {
            foreach (var Supplier in Suppliers)
            {
                Update(Supplier);
            }
        }

        public void Delete(Supplier Supplier)
        {
            var target = One(p => p.SupplierID == Supplier.SupplierID);
            if (target != null)
            {
                var entries = All().ToList();
                entries.Remove(target);
                UpdateContent(entries);
            }
        }

        public void Delete(IEnumerable<Supplier> Suppliers)
        {
            foreach (var Supplier in Suppliers)
            {
                Delete(Supplier);
            }
        }
    }
}
