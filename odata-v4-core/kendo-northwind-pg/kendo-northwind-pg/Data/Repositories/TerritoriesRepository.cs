using kendo_northwind_pg.Data.Models;
using kendo_northwind_pg.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;
using System.Net;

namespace kendo_northwind_pg.Data.Repositories
{
    public class TerritoriesRepository
    {
        private readonly ISession _session;
        private readonly IServiceScopeFactory _scopeFactory;
        private IHttpContextAccessor _contextAccessor;
        private ConcurrentDictionary<string, IList<Territory>> _Territorys;


        public TerritoriesRepository(IHttpContextAccessor httpContextAccessor, IServiceScopeFactory scopeFactory)
        {
            _session = httpContextAccessor.HttpContext.Session;
            _contextAccessor = httpContextAccessor;
            _scopeFactory = scopeFactory;
            _Territorys = new ConcurrentDictionary<string, IList<Territory>>();
        }

        public IList<Territory> All()
        {
            var userKey = SessionUtils.GetUserKey(_contextAccessor);

            return _Territorys.GetOrAdd(userKey, key =>
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

            });
        }

        public Territory One(Func<Territory, bool> predicate)
        {
            return All().FirstOrDefault(predicate);
        }

        public IEnumerable<Territory> Where(Func<Territory, bool> predicate)
        {
            return All().Where(predicate);
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
            var first = All().OrderByDescending(p => p.TerritoryID).FirstOrDefault();
            Territory.TerritoryID = GenerateRandomID();

            All().Insert(0, Territory);
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
                All().Remove(target);
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
