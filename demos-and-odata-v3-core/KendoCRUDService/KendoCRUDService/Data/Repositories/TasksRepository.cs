using KendoCRUDService.Models;
using KendoCRUDService.SessionExtensions;
using Microsoft.EntityFrameworkCore;

namespace KendoCRUDService.Data.Repositories
{
    public class TasksRepository
    {
        private static bool UpdateDatabase = false;

        private readonly ISession _session;
        private readonly IServiceScopeFactory _scopeFactory;
        private IList<TaskViewModel> _tasks;

        public TasksRepository(IHttpContextAccessor httpContextAccessor, IServiceScopeFactory scopeFactory)
        {
            _session = httpContextAccessor.HttpContext.Session;
            _scopeFactory = scopeFactory;
        }

        public IList<TaskViewModel> All()
        {

            if (_tasks == null)
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<DemoDbContext>();
                    _tasks = context.Tasks.ToList().Select(task => new TaskViewModel
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
            }

            return _tasks;
        }

        public TaskViewModel One(Func<TaskViewModel, bool> predicate)
        {
            return All().FirstOrDefault(predicate);
        }

        public void Insert(TaskViewModel task)
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

        public void Update(TaskViewModel task)
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

        public void Delete(TaskViewModel task)
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
    }
}
