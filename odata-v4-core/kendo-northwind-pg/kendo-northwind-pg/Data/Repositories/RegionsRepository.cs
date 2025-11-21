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

        private readonly IUserDataCache _userCache;
        private readonly ILogger<RegionsRepository> _logger;

        private static readonly TimeSpan Ttl = TimeSpan.FromMinutes(15);
        private const string LogicalName = "Regions";


        public RegionsRepository(IHttpContextAccessor httpContextAccessor, IServiceScopeFactory scopeFactory, IUserDataCache userCache,
            ILogger<RegionsRepository> logger)
        {
            _session = httpContextAccessor.HttpContext.Session;
            _contextAccessor = httpContextAccessor;
            _scopeFactory = scopeFactory;
            _userCache = userCache;
            _logger = logger;
        }

        public IList<Region> All()
        {
            var userKey = SessionUtils.GetUserKey(_contextAccessor);

            return _userCache.GetOrCreateList<Region>(userKey, LogicalName, () =>
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

            }, Ttl, sliding: true);
        }

        private void UpdateContent(List<Region> entries)
        {
            var userKey = SessionUtils.GetUserKey(_contextAccessor);
            _userCache.GetOrCreateList<Region>(
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
            var entries = All().ToList();
            var first = entries.OrderByDescending(p => p.RegionID).FirstOrDefault();
            if (first != null)
            {
                Region.RegionID = first.RegionID + 1;
            }
            else
            {
                Region.RegionID = 0;
            }

            entries.Insert(0, Region);
            UpdateContent(entries);
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
                var entries = All().ToList();
                entries.Remove(target);
                UpdateContent(entries);
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
