using KendoCRUDService.Data.Models;
using KendoCRUDService.SessionExtensions;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;

namespace KendoCRUDService.Data.Repositories
{
    public  class GanttDependencyRepository
    {
        private bool UpdateDatabase = false;
        private readonly ISession _session;
        private readonly IDbContextFactory<DemoDbContext> _contextFactory;

        public GanttDependencyRepository(IHttpContextAccessor httpContextAccessor, IDbContextFactory<DemoDbContext> contextFactory)
        {
            _session = httpContextAccessor.HttpContext.Session;
            _contextFactory = contextFactory;
        }


        public IList<GanttDependency> All()
        {
            var result = _session.GetObjectFromJson<List<GanttDependency>>("GanttDependencies");

            if (result == null || UpdateDatabase)
            {
                using (var context = _contextFactory.CreateDbContext())
                {
                    result = context.GanttDependencies.ToList();
                }

                _session.SetObjectAsJson("GanttDependencies", result);
            }

            return result;
        }

        public GanttDependency One(Func<GanttDependency, bool> predicate)
        {
            return All().FirstOrDefault(predicate);
        }

        public IList<GanttDependency> GetDependencies()
        {
            IList<GanttDependency> result;

            using (var context = _contextFactory.CreateDbContext())
            {
                result = context.GanttDependencies.ToList()
                    .Select(p => new GanttDependency
                    {
                        ID = p.ID,
                        PredecessorID = p.PredecessorID,
                        SuccessorID = p.SuccessorID,
                        Type = p.Type
                    })
                    .ToList();
            }

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
            if (!UpdateDatabase)
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
            else
            {
                using (var db = _contextFactory.CreateDbContext())
                {
                    db.GanttDependencies.Add(dependency);

                    db.SaveChanges();
                }
            }
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
            if (!UpdateDatabase)
            {
                var target = One(e => e.ID == dependency.ID);

                if (target != null)
                {
                    target.Type = dependency.Type;
                    target.PredecessorID = dependency.PredecessorID;
                    target.SuccessorID = dependency.SuccessorID;
                }
            }
            else
            {
                using (var db = _contextFactory.CreateDbContext())
                {
                    db.GanttDependencies.Attach(dependency);

                    db.SaveChanges();
                }
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
            if (!UpdateDatabase)
            {
                var target = One(p => p.ID == dependency.ID);
                if (target != null)
                {
                    All().Remove(target);
                }
            }
            else
            {
                using (var db = _contextFactory.CreateDbContext())
                {
                    db.GanttDependencies.Attach(dependency);

                    db.GanttDependencies.Remove(dependency);

                    db.SaveChanges();
                }
            }
        }
    }
}
