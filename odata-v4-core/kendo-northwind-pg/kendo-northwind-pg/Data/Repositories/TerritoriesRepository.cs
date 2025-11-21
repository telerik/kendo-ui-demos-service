using kendo_northwind_pg.Data.Models;
using kendo_northwind_pg.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Net;

namespace kendo_northwind_pg.Data.Repositories
{
    public class TerritoriesRepository
    {
        private readonly ISession _session;
        private readonly IServiceScopeFactory _scopeFactory;
        private IHttpContextAccessor _contextAccessor;
        private readonly IUserDataCache _userCache;
        private readonly ILogger<TerritoriesRepository> _logger;

        private static readonly TimeSpan Ttl = TimeSpan.FromMinutes(15);
        private const string LogicalName = "Territories";


        public TerritoriesRepository(IHttpContextAccessor httpContextAccessor, IServiceScopeFactory scopeFactory, IUserDataCache userCache,
            ILogger<TerritoriesRepository> logger)
        {
            _session = httpContextAccessor.HttpContext.Session;
            _contextAccessor = httpContextAccessor;
            _scopeFactory = scopeFactory;
            _userCache = userCache;
            _logger = logger;
        }

        public IList<Territory> All()
        {
            var userKey = SessionUtils.GetUserKey(_contextAccessor);

            return _userCache.GetOrCreateList<Territory>(userKey, LogicalName, () =>
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<DemoDbContext>();
                    return context.Territories
                    .Include(p => p.EmployeeTerritories).Select(p => new Territory
                    {
                        TerritoryID = p.TerritoryID,
                        TerritoryDescription = p.TerritoryDescription,
                        RegionID = p.RegionID,
                        Region = p.Region,
                        EmployeeTerritories = p.EmployeeTerritories
                    }).ToList();
                }

            }, Ttl, sliding: true);
        }

        public Territory One(Func<Territory, bool> predicate)
        {
            return All().FirstOrDefault(predicate);
        }

        public IEnumerable<Territory> Where(Func<Territory, bool> predicate)
        {
            return All().Where(predicate);
        }

        private void UpdateContent(List<Territory> entries)
        {
            var userKey = SessionUtils.GetUserKey(_contextAccessor);
            _userCache.GetOrCreateList<Territory>(
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

        public static string GenerateRandomID()
        {
            const string chars = "1234567890";
            var random = new Random();
            return new string(Enumerable.Range(0, 5)
                .Select(_ => chars[random.Next(chars.Length)]).ToArray());
        }

        public void Insert(Territory Territory)
        {
            var entries = All().ToList();
            var first = entries.OrderByDescending(p => p.TerritoryID).FirstOrDefault();
            Territory.TerritoryID = GenerateRandomID();

            entries.Insert(0, Territory);
            UpdateContent(entries);
        }

        public void Insert(IEnumerable<Territory> Territorys)
        {
            foreach (var Territory in Territorys)
            {
                Insert(Territory);
            }
        }

        public void Update(Territory Territory)
        {
            var target = One(p => p.TerritoryID == Territory.TerritoryID);
            if (target != null)
            {
                target.TerritoryDescription = Territory.TerritoryDescription;
            }
        }

        public void Update(IEnumerable<Territory> Territorys)
        {
            foreach (var Territory in Territorys)
            {
                Update(Territory);
            }
        }

        public void Delete(Territory Territory)
        {
            var target = One(p => p.TerritoryID == Territory.TerritoryID);
            if (target != null)
            {
                var entries = All().ToList();
                entries.Remove(target);
                UpdateContent(entries);
            }
        }

        public void Delete(IEnumerable<Territory> Territorys)
        {
            foreach (var Territory in Territorys)
            {
                Delete(Territory);
            }
        }
    }
}
