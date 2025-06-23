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
        private ConcurrentDictionary<string, IList<GanttTask>> _tasks;
        private IHttpContextAccessor _contextAccessor;

        public GanttTaskRepository(IHttpContextAccessor httpContextAccessor, IServiceScopeFactory scopeFactory)
        {
            _session = httpContextAccessor.HttpContext.Session;
            _contextAccessor = httpContextAccessor;
            _scopeFactory = scopeFactory;
            _tasks = new ConcurrentDictionary<string, IList<GanttTask>>();
        }

        public IList<GanttTask> All()
        {
            var userKey = SessionUtils.GetUserKey(_contextAccessor);

            return _tasks.GetOrAdd(userKey, key =>
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<DemoDbContext>();
                    return context.GanttTasks.ToList();
                }
            });
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
             var first = All().OrderByDescending(e => e.Id).FirstOrDefault();

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

            All().Insert(0, task);
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
            var target = All().FirstOrDefault(p => p.Id == task.Id);
            if (target != null)
            {
                All().Remove(target);
            }
        }
    }
}
