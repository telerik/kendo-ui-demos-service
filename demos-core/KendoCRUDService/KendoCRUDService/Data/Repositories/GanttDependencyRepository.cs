using KendoCRUDService.Data.Models;
using KendoCRUDService.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SQLitePCL;
using System.Collections.Concurrent;

namespace KendoCRUDService.Data.Repositories
{
    public  class GanttDependencyRepository
    {
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
            }, Ttl, sliding: true);
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
                        ID = p.ID,
                        PredecessorID = p.PredecessorID,
                        SuccessorID = p.SuccessorID,
                        Type = p.Type
                    })
                    .ToList();

            return result;
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

        public void Insert(IEnumerable<GanttDependency> dependencies)
        {
            foreach (var dependency in dependencies)
            {
                Insert(dependency);
            }
        }

        public void Insert(GanttDependency dependency)
        {
            var entries = All();
            var first = entries.OrderByDescending(e => e.ID).FirstOrDefault();

            var id = 0;

            if (first != null)
            {
                id = first.ID;
            }

            dependency.ID = id + 1;

            entries.Insert(0, dependency);
            UpdateContent(entries.ToList());
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
            var target = One(e => e.ID == dependency.ID);

            if (target != null)
            {
                target.Type = dependency.Type;
                target.PredecessorID = dependency.PredecessorID;
                target.SuccessorID = dependency.SuccessorID;
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
            var target = One(p => p.ID == dependency.ID);
            if (target != null)
            {
                var entries = All().ToList();
                entries.Remove(target);
                UpdateContent(entries);
            }
        }
    }
}
