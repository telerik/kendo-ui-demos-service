using kendo_northwind_pg;
using kendo_northwind_pg.Data;
using kendo_northwind_pg.Data.Models;
using kendo_northwind_pg.Extensions;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;
using System.Collections.Concurrent;
using System.Linq;

namespace kendo_northwind_pg.Data.Repositories
{
    public  class GanttDependencyRepository
    {
        private bool UpdateDatabase = false;
        private readonly ISession _session;
        private readonly IServiceScopeFactory _scopeFactory;
        private ConcurrentDictionary<string, IList<GanttDependency>> _dependencies;
        private IHttpContextAccessor _contextAccessor;

        public GanttDependencyRepository(IHttpContextAccessor httpContextAccessor, IServiceScopeFactory scopeFactory)
        {
            _session = httpContextAccessor.HttpContext.Session;
            _contextAccessor = httpContextAccessor;
            _scopeFactory = scopeFactory;
            _dependencies = new ConcurrentDictionary<string, IList<GanttDependency>>();
        }


        public IList<GanttDependency> All()
        {
            var userKey = SessionUtils.GetUserKey(_contextAccessor);

            return _dependencies.GetOrAdd(userKey, key =>
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<DemoDbContext>();
                    return context.GanttDependencies.ToList();
                }
            });
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
            var first = All().OrderByDescending(e => e.Id).FirstOrDefault();

            var id = 0;

            if (first != null)
            {
                id = first.Id;
            }

            dependency.Id = id + 1;

            All().Insert(0, dependency);
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
                All().Remove(target);
            }
        }

        public IEnumerable<GanttDependency> Where(Func<GanttDependency, bool> predicate)
        {
            return All().Where(predicate);
        }
    }
}
