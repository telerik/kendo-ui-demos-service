using KendoCRUDService.Data.Models;
using KendoCRUDService.Models;
using KendoCRUDService.SessionExtensions;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;

namespace KendoCRUDService.Data.Repositories
{
    public class GanttResourceAssignmentsRepository
    {
        private bool UpdateDatabase = false;
        private readonly ISession _session;
        private readonly IServiceScopeFactory _scopeFactory;
        private ConcurrentDictionary<string, IList<GanttResourceAssignment>> _resourceAssignments;
        private IHttpContextAccessor _contextAccessor;

        public GanttResourceAssignmentsRepository(IHttpContextAccessor httpContextAccessor, IServiceScopeFactory scopeFactory)
        {
            _session = httpContextAccessor.HttpContext.Session;
            _contextAccessor = httpContextAccessor;
            _scopeFactory = scopeFactory;
            _resourceAssignments = new ConcurrentDictionary<string, IList<GanttResourceAssignment>>();
        }

        public IList<GanttResourceAssignment> All()
        {
            var userKey = SessionUtils.GetUserKey(_contextAccessor);

            return _resourceAssignments.GetOrAdd(userKey, key =>
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<DemoDbContext>();
                    return context.GanttResourceAssignments.ToList();
                }
            });
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
            var first = All().OrderByDescending(a => a.ID).FirstOrDefault();
            var id = 0;

            if (first != null)
            {
                id = first.ID;
            }

            assignment.ID = id + 1;

            All().Add(assignment);
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
            var target = One(a => a.ID == assigment.ID);

            if (target != null)
            {
                target.Units = assigment.Units;
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
            var target = One(a => a.ID == assigment.ID);
            if (target != null)
            {
                All().Remove(target);
            }
        }
    }
}
