using KendoCRUDService.Data.Models;
using KendoCRUDService.Extensions;
using KendoCRUDService.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace KendoCRUDService.Data.Repositories
{
    public class DiagramShapesRepository
    {
        private readonly ISession _session;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IUserDataCache _userCache;
        private readonly ILogger<DiagramShapesRepository> _logger;

        private static readonly TimeSpan Ttl = TimeSpan.FromMinutes(15);
        private const string LogicalName = "OrgChartShapes";
        private IHttpContextAccessor _contextAccessor;

        public DiagramShapesRepository(IHttpContextAccessor httpContextAccessor, IServiceScopeFactory scopeFactory, IUserDataCache userCache,
            ILogger<DiagramShapesRepository> logger)
        {
            _session = httpContextAccessor.HttpContext.Session;
            _contextAccessor = httpContextAccessor;
            _scopeFactory = scopeFactory;
            _userCache = userCache;
            _logger = logger;
        }

        public IList<OrgChartShape> All()
        {
            var userKey = SessionUtils.GetUserKey(_contextAccessor);

            return _userCache.GetOrCreateList<OrgChartShape>(userKey, LogicalName, () =>
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<DemoDbContext>();
                    return context.OrgChartShapes.ToList();
                }
            }, Ttl, sliding: true);
        }

        private void UpdateContent(List<OrgChartShape> entries)
        {
            var userKey = SessionUtils.GetUserKey(_contextAccessor);
            _userCache.GetOrCreateList<OrgChartShape>(
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

        public OrgChartShape One(Func<OrgChartShape, bool> predicate)
        {
            return All().FirstOrDefault(predicate);
        }

        public void Insert(IEnumerable<OrgChartShape> shapes)
        {
            foreach (var shape in shapes)
            {
                Insert(shape);
            }
        }

        public void Insert(OrgChartShape shape)
        {
            var entries = All().ToList();
            var first = entries.OrderByDescending(e => e.Id).FirstOrDefault();

            var id = 0;

            if (first != null)
            {
                id = first.Id;
            }

            shape.Id = id + 1;

            entries.Insert(0, shape);
            UpdateContent(entries);
        }

        public void Update(IEnumerable<OrgChartShape> shapes)
        {
            foreach (var shape in shapes)
            {
                Update(shape);
            }
        }

        public void Update(OrgChartShape shape)
        {
            var target = One(e => e.Id == shape.Id);

            if (target != null)
            {
                target.JobTitle = shape.JobTitle;
                target.Color = shape.Color;
            }
        }

        public void Delete(IEnumerable<OrgChartShape> shapes)
        {
            foreach (var shape in shapes)
            {
                Delete(shape);
            }
        }

        public void Delete(OrgChartShape shape)
        {
            var target = One(p => p.Id == shape.Id);
            if (target != null)
            {
                var entries = All().ToList();
                entries.Remove(target);
                UpdateContent(entries);
            }
        }
    }
}
