using KendoCRUDService.Data.Models;
using KendoCRUDService.Extensions;
using KendoCRUDService.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace KendoCRUDService.Data.Repositories
{
    public class GanttResourceAssignmentsRepository
    {
        private bool UpdateDatabase = false;
        private readonly ISession _session;
        private readonly IServiceScopeFactory _scopeFactory;
        private IHttpContextAccessor _contextAccessor;

        private readonly IUserDataCache _userCache;
        private readonly ILogger<GanttResourceAssignmentsRepository> _logger;

        private static readonly TimeSpan Ttl = TimeSpan.FromMinutes(15);
        private const string LogicalName = "GanttResourceAssignments";

        public GanttResourceAssignmentsRepository(IHttpContextAccessor httpContextAccessor, IServiceScopeFactory scopeFactory, IUserDataCache userCache,
            ILogger<GanttResourceAssignmentsRepository> logger)
        {
            _session = httpContextAccessor.HttpContext.Session;
            _contextAccessor = httpContextAccessor;
            _scopeFactory = scopeFactory;
            _userCache = userCache;
            _logger = logger;
        }

        public IList<GanttResourceAssignment> All()
        {
            var userKey = SessionUtils.GetUserKey(_contextAccessor);

            return _userCache.GetOrCreateList<GanttResourceAssignment>(userKey, LogicalName, () =>
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<DemoDbContext>();
                    return context.GanttResourceAssignments.ToList();
                }
            },Ttl, sliding: true);
        }

        private void UpdateContent(List<GanttResourceAssignment> entries)
        {
            var userKey = SessionUtils.GetUserKey(_contextAccessor);
            _userCache.GetOrCreateList<GanttResourceAssignment>(
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
            var entries = All().ToList();
            var first = entries.OrderByDescending(a => a.ID).FirstOrDefault();
            var id = 0;

            if (first != null)
            {
                id = first.ID;
            }

            assignment.ID = id + 1;

            entries.Add(assignment);
            UpdateContent(entries);
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
                var entries = All().ToList();
                entries.Remove(target);
                UpdateContent(entries);
            }
        }
    }
}
