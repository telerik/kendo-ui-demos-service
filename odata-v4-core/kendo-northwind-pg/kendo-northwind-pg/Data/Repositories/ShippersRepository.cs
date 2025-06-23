using kendo_northwind_pg.Data.Models;
using kendo_northwind_pg.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;
using System.Net;

namespace kendo_northwind_pg.Data.Repositories
{
    public class ShippersRepository
    {
        private readonly ISession _session;
        private readonly IServiceScopeFactory _scopeFactory;
        private IHttpContextAccessor _contextAccessor;
        private ConcurrentDictionary<string, IList<Shipper>> _Shippers;


        public ShippersRepository(IHttpContextAccessor httpContextAccessor, IServiceScopeFactory scopeFactory)
        {
            _session = httpContextAccessor.HttpContext.Session;
            _contextAccessor = httpContextAccessor;
            _scopeFactory = scopeFactory;
            _Shippers = new ConcurrentDictionary<string, IList<Shipper>>();
        }

        public IList<Shipper> All()
        {
            var userKey = SessionUtils.GetUserKey(_contextAccessor);

            return _Shippers.GetOrAdd(userKey, key =>
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<DemoDbContext>();
                    return context.Shippers
                    .Include(p => p.Orders).Select(p => new Shipper
                    {
                        ShipperID = p.ShipperID,
                        CompanyName = p.CompanyName,
                        Phone = p.Phone,
                        Orders = p.Orders,
                    }).ToList();
                }

            });
        }

        public Shipper One(Func<Shipper, bool> predicate)
        {
            return All().FirstOrDefault(predicate);
        }

        public IEnumerable<Shipper> Where(Func<Shipper, bool> predicate)
        {
            return All().Where(predicate);
        }

        public void Insert(Shipper Shipper)
        {
            var first = All().OrderByDescending(p => p.ShipperID).FirstOrDefault();
            if (first != null)
            {
                Shipper.ShipperID = first.ShipperID + 1;
            }
            else
            {
                Shipper.ShipperID = 0;
            }

            All().Insert(0, Shipper);
        }

        public void Insert(IEnumerable<Shipper> Shippers)
        {
            foreach (var Shipper in Shippers)
            {
                Insert(Shipper);
            }
        }

        public void Update(Shipper Shipper)
        {
            var target = One(p => p.ShipperID == Shipper.ShipperID);
            if (target != null)
            {
                target.CompanyName = Shipper.CompanyName;
                target.Phone = Shipper.Phone;
            }
        }

        public void Update(IEnumerable<Shipper> Shippers)
        {
            foreach (var Shipper in Shippers)
            {
                Update(Shipper);
            }
        }

        public void Delete(Shipper Shipper)
        {
            var target = One(p => p.ShipperID == Shipper.ShipperID);
            if (target != null)
            {
                All().Remove(target);
            }
        }

        public void Delete(IEnumerable<Shipper> Shippers)
        {
            foreach (var Shipper in Shippers)
            {
                Delete(Shipper);
            }
        }
    }
}
