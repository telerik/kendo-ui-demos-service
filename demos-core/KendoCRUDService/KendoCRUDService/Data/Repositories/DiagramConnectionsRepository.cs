using KendoCRUDService.Data.Models;
using KendoCRUDService.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace KendoCRUDService.Data.Repositories
{

    public class DiagramConnectionsRepository
    {
        private readonly ISession _session;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IUserDataCache _userCache;
        private readonly ILogger<DiagramConnectionsRepository> _logger;

        private static readonly TimeSpan Ttl = TimeSpan.FromMinutes(15);
        private const string LogicalName = "OrgChartConnections";
        private IHttpContextAccessor _contextAccessor;

        public DiagramConnectionsRepository(IHttpContextAccessor httpContextAccessor, IServiceScopeFactory scopeFactory, IUserDataCache userCache,
            ILogger<DiagramConnectionsRepository> logger)
        {
            _session = httpContextAccessor.HttpContext.Session;
            _contextAccessor = httpContextAccessor;
            _scopeFactory = scopeFactory;
            _userCache = userCache;
            _logger = logger;
        }

        public IList<OrgChartConnection> All()
        {
            var userKey = SessionUtils.GetUserKey(_contextAccessor);

            return _userCache.GetOrCreateList<OrgChartConnection>(userKey, LogicalName, () =>
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<DemoDbContext>();
                    return context.OrgChartConnections.ToList();
                }
            }, Ttl, sliding: true);
        }

        private void UpdateContent(List<OrgChartConnection> entries)
        {
            var userKey = SessionUtils.GetUserKey(_contextAccessor);
            _userCache.GetOrCreateList<OrgChartConnection>(
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

        public OrgChartConnection One(Func<OrgChartConnection, bool> predicate)
        {
            return All().FirstOrDefault(predicate);
        }

        public void Insert(IEnumerable<OrgChartConnection> connections)
        {
            foreach (var connection in connections)
            {
                Insert(connection);
            }
        }

        public void Insert(OrgChartConnection connection)
        {
            var entries = All();
            var first = entries.OrderByDescending(e => e.Id).FirstOrDefault();

            long id = 0;

            if (first != null)
            {
                id = first.Id;
            }

            connection.Id = id + 1;

            entries.Insert(0, connection);
            UpdateContent(entries.ToList());
        }

        public void Update(IEnumerable<OrgChartConnection> connections)
        {
            foreach (var connection in connections)
            {
                Update(connection);
            }
        }

        public void Update(OrgChartConnection connection)
        {
            var target = One(e => e.Id == connection.Id);

            if (target != null)
            {
                target.FromShapeId = connection.FromShapeId;
                target.ToShapeId = connection.ToShapeId;
                target.Text = connection.Text;
                target.FromPointX = connection.FromPointX;
                target.FromPointY = connection.FromPointY;
                target.ToPointX = connection.ToPointX;
                target.ToPointY = connection.ToPointY;
            }
        }

        public void Delete(IEnumerable<OrgChartConnection> connections)
        {
            foreach (var connection in connections)
            {
                Delete(connection);
            }
        }

        public void Delete(OrgChartConnection connection)
        {
            var target = One(p => p.Id == connection.Id);
            if (target != null)
            {
                var entries = All().ToList();
                entries.Remove(target);
                UpdateContent(entries);
            }
        }
    }
}
