using KendoCRUDService.Data.Models;
using KendoCRUDService.SessionExtensions;
using Microsoft.EntityFrameworkCore;

namespace KendoCRUDService.Data.Repositories
{
    public class GanttResourceAssignmentsRepository
    {
        private bool UpdateDatabase = false;
        private readonly ISession _session;
        private readonly IDbContextFactory<DemoDbContext> _contextFactory;

        public GanttResourceAssignmentsRepository(IHttpContextAccessor httpContextAccessor, IDbContextFactory<DemoDbContext> contextFactory)
        {
            _session = httpContextAccessor.HttpContext.Session;
            _contextFactory = contextFactory;
        }

        public IList<GanttResourceAssignment> All()
        {
            var result = _session.GetObjectFromJson<IList<GanttResourceAssignment>>("GanttAssignments");

            if (result == null || UpdateDatabase)
            {
                using (var context = _contextFactory.CreateDbContext())
                {
                    result = context.GanttResourceAssignments.ToList();
                }

                _session.SetObjectAsJson("GanttAssignments", result);
            }

            return result;
        }

        public GanttResourceAssignment One(Func<GanttResourceAssignment, bool> predicate)
        {
            return All().FirstOrDefault(predicate);
        }

        public void Insert(IEnumerable<GanttResourceAssignment> assignments)
        {
            foreach (var task in assignments)
            {
                Insert(task);
            }
        }

        public void Insert(GanttResourceAssignment assignment)
        {
            if (!UpdateDatabase)
            {
                var first = All().OrderByDescending(a => a.ID).FirstOrDefault();
                var id = 0;

                if (first != null)
                {
                    id = first.ID;
                }

                assignment.ID = id + 1;

                All().Add(assignment);
            }
            else
            {
                using (var db = _contextFactory.CreateDbContext())
                {
                    db.GanttResourceAssignments.Add(assignment);

                    db.SaveChanges();
                }
            }
        }

        public void Update(IEnumerable<GanttResourceAssignment> assigments)
        {
            foreach (var assigment in assigments)
            {
                Update(assigment);
            }
        }

        public void Update(GanttResourceAssignment assigment)
        {
            if (!UpdateDatabase)
            {
                var target = One(a => a.ID == assigment.ID);

                if (target != null)
                {
                    target.Units = assigment.Units;
                }
            }
            else
            {
                using (var db = _contextFactory.CreateDbContext())
                {
                    db.GanttResourceAssignments.Attach(assigment);

                    db.SaveChanges();
                }
            }
        }

        public void Delete(IEnumerable<GanttResourceAssignment> assigments)
        {
            foreach (var assigment in assigments)
            {
                Delete(assigment);
            }
        }

        public void Delete(GanttResourceAssignment assigment)
        {
            if (!UpdateDatabase)
            {
                var target = One(a => a.ID == assigment.ID);
                if (target != null)
                {
                    All().Remove(target);
                }
            }
            else
            {
                using (var db = _contextFactory.CreateDbContext())
                {
                    db.GanttResourceAssignments.Attach(assigment);

                    db.GanttResourceAssignments.Remove(assigment);

                    db.SaveChanges();
                }
            }
        }
    }
}
