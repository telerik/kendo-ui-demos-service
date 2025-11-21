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
        private readonly IUserDataCache _userCache;
        private readonly ILogger<ShippersRepository> _logger;

        private static readonly TimeSpan Ttl = TimeSpan.FromMinutes(15);
        private const string LogicalName = "Shippers";


        public ShippersRepository(IHttpContextAccessor httpContextAccessor, IServiceScopeFactory scopeFactory, IUserDataCache userCache,
            ILogger<ShippersRepository> logger)
        {
            _session = httpContextAccessor.HttpContext.Session;
            _contextAccessor = httpContextAccessor;
            _scopeFactory = scopeFactory;
            _userCache = userCache;
            _logger = logger;
        }

        public IList<Shipper> All()
        {
            var userKey = SessionUtils.GetUserKey(_contextAccessor);

            return _userCache.GetOrCreateList<Shipper>(userKey, LogicalName, () =>
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

            }, Ttl, sliding:true);
        }

        private void UpdateContent(List<Shipper> entries)
        {
            var userKey = SessionUtils.GetUserKey(_contextAccessor);
            _userCache.GetOrCreateList<Shipper>(
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
            var entries = All().ToList();
            var first = entries.OrderByDescending(p => p.ShipperID).FirstOrDefault();
            if (first != null)
            {
                Shipper.ShipperID = first.ShipperID + 1;
            }
            else
            {
                Shipper.ShipperID = 0;
            }

            entries.Insert(0, Shipper);
            UpdateContent(entries);
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
                var entries = All().ToList();
                entries.Remove(target);
                UpdateContent(entries);
               
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
