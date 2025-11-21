using kendo_northwind_pg;
using kendo_northwind_pg.Data;
using kendo_northwind_pg.Data.Models;
using kendo_northwind_pg.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;
using System.Linq;

namespace kendo_northwind_pg.Data.Repositories
{
    public class GanttTaskRepository
    {
        private readonly ISession _session;
        private readonly IServiceScopeFactory _scopeFactory;
        private IHttpContextAccessor _contextAccessor;
        private readonly IUserDataCache _userCache;
        private readonly ILogger<GanttTaskRepository> _logger;

        private static readonly TimeSpan Ttl = TimeSpan.FromMinutes(15);
        private const string LogicalName = "GanttTaskRepositories";

        public GanttTaskRepository(IHttpContextAccessor httpContextAccessor, IServiceScopeFactory scopeFactory, IUserDataCache userCache,
            ILogger<GanttTaskRepository> logger)
        {
            _session = httpContextAccessor.HttpContext.Session;
            _contextAccessor = httpContextAccessor;
            _scopeFactory = scopeFactory;
            _userCache = userCache;
            _logger = logger;
        }

        public IList<GanttTask> All()
        {
            var userKey = SessionUtils.GetUserKey(_contextAccessor);

            return _userCache.GetOrCreateList<GanttTask>(
                userKey,
                LogicalName,
                () =>
                {
                    using (var scope = _scopeFactory.CreateScope())
                    {
                        var context = scope.ServiceProvider.GetRequiredService<DemoDbContext>();
                        return context.GanttTasks.ToList();
                    }
                }, Ttl, sliding: true);
        }
        private void UpdateContent(List<GanttTask> entries)
        {
            var userKey = SessionUtils.GetUserKey(_contextAccessor);
            _userCache.GetOrCreateList<GanttTask>(
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

        public void Insert(IEnumerable<GanttTask> tasks)
        {
            foreach (var task in tasks)
            {
                Insert(task);
            }
        }

        public void Insert(GanttTask task)
        {
            var entries = All().ToList();
            var first = entries.OrderByDescending(e => e.Id).FirstOrDefault();

             var id = 0;

             if (first != null)
             {
                id = first.Id;
             }

             task.Id = id + 1;

             if (task.Id == task.ParentId)
             {
                 throw new Exception("Parent task was not created!");
             }

            entries.Insert(0, task);
            UpdateContent(entries);
        }

        public void Update(IEnumerable<GanttTask> tasks)
        {
            foreach (var task in tasks)
            {
                Update(task);
            }
        }

        public void Update(GanttTask task)
        {
            var target = All().FirstOrDefault(e => e.Id == task.Id);

            if (target != null)
            {
                target.Title = task.Title;
                target.Start = task.Start;
                target.End = task.End;
                target.PercentComplete = task.PercentComplete;
                target.OrderId = task.OrderId;
                target.ParentId = task.ParentId;
                target.Summary = task.Summary;
                target.Expanded = task.Expanded;
            }
        }

        public void Delete(IEnumerable<GanttTask> tasks)
        {
            IList<GanttTask> allTasks = All();
            foreach (var task in tasks)
            {
                Delete(task);
            }
        }

        public IEnumerable<GanttTask> Where(Func<GanttTask, bool> predicate)
        {
            return All().Where(predicate);
        }

        public void Delete(GanttTask task)
        {
            var entries = All().ToList();
            var target = entries.FirstOrDefault(p => p.Id == task.Id);
            if (target != null)
            {
                entries.Remove(target);
                UpdateContent(entries);
            }
        }
    }
}
