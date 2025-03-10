using KendoCRUDService.Data.Models;
using KendoCRUDService.SessionExtensions;
using Microsoft.EntityFrameworkCore;

namespace KendoCRUDService.Data.Repositories
{
    public class GanttTaskRepository
    {
        private readonly ISession _session;
        private readonly IServiceScopeFactory _scopeFactory;
        private IList<GanttTask> _tasks;

        public GanttTaskRepository(IHttpContextAccessor httpContextAccessor, IServiceScopeFactory scopeFactory)
        {
            _session = httpContextAccessor.HttpContext.Session;
            _scopeFactory = scopeFactory;
        }

        public IList<GanttTask> All()
        {
            if (_tasks == null)
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<DemoDbContext>();
                    _tasks = context.GanttTasks.ToList();
                }
            }

            return _tasks;
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
             var first = _tasks.OrderByDescending(e => e.ID).FirstOrDefault();

             var id = 0;

             if (first != null)
             {
                 id = first.ID;
             }

             task.ID = id + 1;

             if (task.ID == task.ParentID)
             {
                 throw new Exception("Parent task was not created!");
             }

            _tasks.Insert(0, task);
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
            var target = _tasks.FirstOrDefault(e => e.ID == task.ID);

            if (target != null)
            {
                target.Title = task.Title;
                target.Start = task.Start;
                target.End = task.End;
                target.PercentComplete = task.PercentComplete;
                target.OrderID = task.OrderID;
                target.ParentID = task.ParentID;
                target.Summary = task.Summary;
                target.Expanded = task.Expanded;
            }
        }

        public void Delete(IEnumerable<GanttTask> tasks)
        {
            IList<GanttTask> allTasks = All();
            foreach (var task in tasks)
            {
                Delete(allTasks);
            }
        }

        public void Delete(GanttTask task)
        {
            var target = _tasks.FirstOrDefault(p => p.ID == task.ID);
            if (target != null)
            {
                _tasks.Remove(target);
            }
        }
    }
}
