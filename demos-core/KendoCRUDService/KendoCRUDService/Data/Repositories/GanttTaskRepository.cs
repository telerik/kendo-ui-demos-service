using KendoCRUDService.Data.Models;
using KendoCRUDService.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;

namespace KendoCRUDService.Data.Repositories
{
    public class GanttTaskRepository
    {
        private readonly ISession _session;
        private readonly IServiceScopeFactory _scopeFactory;
        private ConcurrentDictionary<string, IList<GanttTask>> _tasks;
        private IHttpContextAccessor _contextAccessor;

        private readonly IUserDataCache _userCache;
        private readonly ILogger<GanttTaskRepository> _logger;

        private static readonly TimeSpan Ttl = TimeSpan.FromMinutes(15);
        private const string LogicalName = "GanttTasks";

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
                },Ttl, sliding: true);
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
             var entities = All().ToList();
             var first = entities.OrderByDescending(e => e.ID).FirstOrDefault();

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

             entities.Insert(0, task);
             UpdateContent(entities);
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
            var target = All().FirstOrDefault(e => e.ID == task.ID);

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
                Delete(task);
            }
        }

        public void Delete(GanttTask task)
        {
            var entities = All().ToList();
            var target = entities.FirstOrDefault(p => p.ID == task.ID);
            if (target != null)
            {
                entities.Remove(target);
            }
            UpdateContent(entities);
        }
    }
}
