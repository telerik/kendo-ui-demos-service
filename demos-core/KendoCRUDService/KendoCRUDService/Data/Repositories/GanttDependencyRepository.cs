using KendoCRUDService.Data.Models;
using KendoCRUDService.SessionExtensions;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;
using System.Collections.Concurrent;

namespace KendoCRUDService.Data.Repositories
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
                        ID = p.ID,
                        PredecessorID = p.PredecessorID,
                        SuccessorID = p.SuccessorID,
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
            var first = All().OrderByDescending(e => e.ID).FirstOrDefault();

            var id = 0;

            if (first != null)
            {
                id = first.ID;
            }

            dependency.ID = id + 1;

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
                All().Remove(target);
            }
        }
    }
}
