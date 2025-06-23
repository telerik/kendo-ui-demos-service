using kendo_northwind_pg.Data.Models;
using kendo_northwind_pg.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;
using System.Net;

namespace kendo_northwind_pg.Data.Repositories
{
    public class RegionsRepository
    {
        private readonly ISession _session;
        private readonly IServiceScopeFactory _scopeFactory;
        private IHttpContextAccessor _contextAccessor;
        private ConcurrentDictionary<string, IList<Region>> _Regions;


        public RegionsRepository(IHttpContextAccessor httpContextAccessor, IServiceScopeFactory scopeFactory)
        {
            _session = httpContextAccessor.HttpContext.Session;
            _contextAccessor = httpContextAccessor;
            _scopeFactory = scopeFactory;
            _Regions = new ConcurrentDictionary<string, IList<Region>>();
        }

        public IList<Region> All()
        {
            var userKey = SessionUtils.GetUserKey(_contextAccessor);

            return _Regions.GetOrAdd(userKey, key =>
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<DemoDbContext>();
                    return context.Region
                    .Include(p => p.Territories).Select(p => new Region
                    {
                        RegionID = p.RegionID,
                        RegionDescription = p.RegionDescription,
                        Territories = p.Territories
                    }).ToList();
                }

            });
        }

        public Region One(Func<Region, bool> predicate)
        {
            return All().FirstOrDefault(predicate);
        }

        public IEnumerable<Region> Where(Func<Region, bool> predicate)
        {
            return All().Where(predicate);
        }

        public void Insert(Region Region)
        {
            var first = All().OrderByDescending(p => p.RegionID).FirstOrDefault();
            if (first != null)
            {
                Region.RegionID = first.RegionID + 1;
            }
            else
            {
                Region.RegionID = 0;
            }

            All().Insert(0, Region);
        }

        public void Insert(IEnumerable<Region> Regions)
        {
            foreach (var Region in Regions)
            {
                Insert(Region);
            }
        }

        public void Update(Region Region)
        {
            var target = One(p => p.RegionID == Region.RegionID);
            if (target != null)
            {
                target.RegionDescription = Region.RegionDescription;
            }
        }

        public void Update(IEnumerable<Region> Regions)
        {
            foreach (var Region in Regions)
            {
                Update(Region);
            }
        }

        public void Delete(Region Region)
        {
            var target = One(p => p.RegionID == Region.RegionID);
            if (target != null)
            {
                All().Remove(target);
            }
        }

        public void Delete(IEnumerable<Region> Regions)
        {
            foreach (var Region in Regions)
            {
                Delete(Region);
            }
        }
    }
}
