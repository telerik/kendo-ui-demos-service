using KendoCRUDService.Data.Models;
using KendoCRUDService.Models;
using KendoCRUDService.SessionExtensions;
using Microsoft.EntityFrameworkCore;

namespace KendoCRUDService.Data.Repositories
{
    public class GanttResourceAssignmentsRepository
    {
        private bool UpdateDatabase = false;
        private readonly ISession _session;
        private readonly IServiceScopeFactory _scopeFactory;
        private IList<GanttResourceAssignment> _resourceAssignments;

        public GanttResourceAssignmentsRepository(IHttpContextAccessor httpContextAccessor, IServiceScopeFactory scopeFactory)
        {
            _session = httpContextAccessor.HttpContext.Session;
            _scopeFactory = scopeFactory;
        }

        public IList<GanttResourceAssignment> All()
        {
            if (_resourceAssignments == null)
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<DemoDbContext>();
                    _resourceAssignments = context.GanttResourceAssignments.ToList();
                }
            }

            return _resourceAssignments;
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
