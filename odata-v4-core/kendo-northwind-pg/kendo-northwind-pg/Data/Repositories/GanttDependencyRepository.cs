using kendo_northwind_pg;
using kendo_northwind_pg.Data;
using kendo_northwind_pg.Data.Models;
using kendo_northwind_pg.Extensions;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.RateLimiting;

namespace kendo_northwind_pg.Data.Repositories
{
    public  class GanttDependencyRepository
    {
        private bool UpdateDatabase = false;
        private readonly ISession _session;
        private readonly IServiceScopeFactory _scopeFactory;
        private IHttpContextAccessor _contextAccessor;
        private readonly IUserDataCache _userCache;
        private readonly ILogger<GanttDependencyRepository> _logger;

        private static readonly TimeSpan Ttl = TimeSpan.FromMinutes(15);
        private const string LogicalName = "GanttDependencies";

        public GanttDependencyRepository(IHttpContextAccessor httpContextAccessor, IServiceScopeFactory scopeFactory, IUserDataCache userCache,
            ILogger<GanttDependencyRepository> logger)
        {
            _session = httpContextAccessor.HttpContext.Session;
            _contextAccessor = httpContextAccessor;
            _scopeFactory = scopeFactory;
            _userCache = userCache;
            _logger = logger;
        }


        public IList<GanttDependency> All()
        {
            var userKey = SessionUtils.GetUserKey(_contextAccessor);

            return _userCache.GetOrCreateList<GanttDependency>(userKey, LogicalName, () =>
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<DemoDbContext>();
                    return context.GanttDependencies.ToList();
                }
            },Ttl, sliding: true);
        }

        private void UpdateContent(List<GanttDependency> entries)
        {
            var userKey = SessionUtils.GetUserKey(_contextAccessor);
            _userCache.GetOrCreateList<GanttDependency>(
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

        public GanttDependency One(Func<GanttDependency, bool> predicate)
        {
            return All().FirstOrDefault(predicate);
        }

        public IList<GanttDependency> GetDependencies()
        {
            IList<GanttDependency> result;

            result = All().ToList()
                    .Select(p => new GanttDependency
                    {
                        Id = p.Id,
                        PredecessorId = p.PredecessorId,
                        SuccessorId = p.SuccessorId,
                        Type = p.Type
                    })
                    .ToList();

            return result;
        }

        public void Insert(IEnumerable<GanttDependency> dependencies)
        {
            foreach (var dependency in dependencies)
            {
                Insert(dependency);
            }
        }

        public void Insert(GanttDependency dependency)
        {
            var entries = All().ToList();
            var first = entries.OrderByDescending(e => e.Id).FirstOrDefault();

            var id = 0;

            if (first != null)
            {
                id = first.Id;
            }

            dependency.Id = id + 1;

            entries.Insert(0, dependency);
            UpdateContent(entries);
        }

        public void Update(IEnumerable<GanttDependency> dependencies)
        {
            foreach (var dependency in dependencies)
            {
                Update(dependency);
            }
        }

        public void Update(GanttDependency dependency)
        {
            var target = One(e => e.Id == dependency.Id);

            if (target != null)
            {
                target.Type = dependency.Type;
                target.PredecessorId = dependency.PredecessorId;
                target.SuccessorId = dependency.SuccessorId;
            }
        }

        public void Delete(IEnumerable<GanttDependency> dependencies)
        {
            foreach (var dependency in dependencies)
            {
                Delete(dependency);
            }
        }

        public void Delete(GanttDependency dependency)
        {
            var target = One(p => p.Id == dependency.Id);
            if (target != null)
            {
                var entries = All().ToList();
                entries.Remove(target);
                UpdateContent(entries);
            }
        }

        public IEnumerable<GanttDependency> Where(Func<GanttDependency, bool> predicate)
        {
            return All().Where(predicate);
        }
    }
}
