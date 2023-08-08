using KendoCRUDService.Data.Models;
using KendoCRUDService.SessionExtensions;
using Microsoft.EntityFrameworkCore;

namespace KendoCRUDService.Data.Repositories
{
    public class GanttTaskRepository
    {
        private bool UpdateDatabase = false;

        private readonly ISession _session;
        private readonly IDbContextFactory<DemoDbContext> _contextFactory;

        public GanttTaskRepository(IHttpContextAccessor httpContextAccessor, IDbContextFactory<DemoDbContext> contextFactory)
        {
            _session = httpContextAccessor.HttpContext.Session;
            _contextFactory = contextFactory;
        }

        public IList<GanttTask> All()
        {
            var result = _session.GetObjectFromJson<IList<GanttTask>>("GanttTasks");

            if (result == null || UpdateDatabase)
            {
                using (var context = _contextFactory.CreateDbContext())
                {
                    result = context.GanttTasks.ToList();
                }

                _session.SetObjectAsJson("GanttTasks", result);
            }

            return result;
        }

        public GanttTask One(Func<GanttTask, bool> predicate)
        {
            return All().FirstOrDefault(predicate);
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
            if (!UpdateDatabase)
            {
                var first = All().OrderByDescending(e => e.ID).FirstOrDefault();

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

                All().Insert(0, task);
            }
            else
            {
                using (var db = _contextFactory.CreateDbContext())
                {
                    db.GanttTasks.Add(task);

                    db.SaveChanges();
                }
            }
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
            if (!UpdateDatabase)
            {
                var target = One(e => e.ID == task.ID);

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
            else
            {
                using (var db = _contextFactory.CreateDbContext())
                {
                    db.GanttTasks.Attach(task);

                    db.SaveChanges();
                }
            }
        }

        public void Delete(IEnumerable<GanttTask> tasks)
        {
            foreach (var task in tasks)
            {
                Delete(task);
            }
        }

        public void Delete(GanttTask task)
        {
            if (!UpdateDatabase)
            {
                var target = One(p => p.ID == task.ID);
                if (target != null)
                {
                    All().Remove(target);
                }
            }
            else
            {
                using (var db = _contextFactory.CreateDbContext())
                {
                    db.GanttTasks.Attach(task);

                    db.GanttTasks.Remove(task);

                    db.SaveChanges();
                }
            }
        }
    }
}
