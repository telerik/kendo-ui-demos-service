using KendoCRUDService.Models;
using KendoCRUDService.SessionExtensions;
using Microsoft.EntityFrameworkCore;

namespace KendoCRUDService.Data.Repositories
{
    public class TasksRepository
    {
        private static bool UpdateDatabase = false;

        private readonly ISession _session;
        private readonly IDbContextFactory<DemoDbContext> _contextFactory;

        public TasksRepository(IHttpContextAccessor httpContextAccessor, IDbContextFactory<DemoDbContext> contextFactory)
        {
            _session = httpContextAccessor.HttpContext.Session;
            _contextFactory = contextFactory;
        }

        public IList<TaskViewModel> All()
        {
            var result = _session.GetObjectFromJson<IList<TaskViewModel>>("Tasks");

            if (result == null || UpdateDatabase)
            {
                using (var db = _contextFactory.CreateDbContext())
                {
                    result = db.Tasks.ToList().Select(task => new TaskViewModel
                    {
                        TaskID = task.TaskID,
                        Title = task.Title,
                        Start = DateTime.SpecifyKind(task.Start, DateTimeKind.Utc),
                        End = DateTime.SpecifyKind(task.End, DateTimeKind.Utc),
                        StartTimezone = task.StartTimezone,
                        EndTimezone = task.EndTimezone,
                        Description = task.Description,
                        IsAllDay = task.IsAllDay,
                        RecurrenceRule = task.RecurrenceRule,
                        RecurrenceException = task.RecurrenceException,
                        RecurrenceID = task.RecurrenceID,
                        OwnerID = task.OwnerID
                    }).ToList();
                }

                _session.SetObjectAsJson("Tasks", result);
            }

            return result;
        }

        public TaskViewModel One(Func<TaskViewModel, bool> predicate)
        {
            return All().FirstOrDefault(predicate);
        }

        public void Insert(TaskViewModel task)
        {
            if (!UpdateDatabase)
            {
                var first = All().OrderByDescending(e => e.TaskID).FirstOrDefault();

                var id = 0;

                if (first != null)
                {
                    id = first.TaskID;
                }

                task.TaskID = id + 1;

                All().Insert(0, task);
            }
            else
            {
                using (var db = _contextFactory.CreateDbContext())
                {
                    var entity = task.ToEntity();

                    db.Tasks.Add(entity);
                    db.SaveChanges();

                    task.TaskID = entity.TaskID;
                }
            }
        }

        public void Update(TaskViewModel task)
        {
            if (!UpdateDatabase)
            {
                var target = One(e => e.TaskID == task.TaskID);

                if (target != null)
                {
                    target.Title = task.Title;
                    target.Description = task.Description;
                    target.IsAllDay = task.IsAllDay;
                    target.RecurrenceRule = task.RecurrenceRule;
                    target.RecurrenceException = task.RecurrenceException;
                    target.RecurrenceID = task.RecurrenceID;
                    target.OwnerID = task.OwnerID;
                    target.StartTimezone = task.StartTimezone;
                    target.EndTimezone = task.EndTimezone;
                    target.Start = task.Start;
                    target.End = task.End;
                }
            }
            else
            {
                using (var db = _contextFactory.CreateDbContext())
                {
                    var entity = task.ToEntity();
                    db.Tasks.Attach(entity);
                    db.SaveChanges();
                }
            }
        }

        public void Delete(TaskViewModel task)
        {
            if (!UpdateDatabase)
            {
                var target = One(p => p.TaskID == task.TaskID);
                if (target != null)
                {
                    All().Remove(target);

                    var recurrenceExceptions = All().Where(m => m.RecurrenceID == task.TaskID).ToList();

                    foreach (var recurrenceException in recurrenceExceptions)
                    {
                        All().Remove(recurrenceException);
                    }
                }
            }
            else
            {
                using (var db = _contextFactory.CreateDbContext())
                {
                    var entity = task.ToEntity();
                    db.Tasks.Attach(entity);

                    var recurrenceExceptions = db.Tasks.Where(t => t.RecurrenceID == task.TaskID);

                    foreach (var recurrenceException in recurrenceExceptions)
                    {
                        db.Tasks.Remove(recurrenceException);
                    }

                    db.Tasks.Remove(entity);
                    db.SaveChanges();
                }
            }
        }
    }
}
