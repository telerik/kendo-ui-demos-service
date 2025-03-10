using KendoCRUDService.Data.Models;
using KendoCRUDService.SessionExtensions;
using Microsoft.EntityFrameworkCore;

namespace KendoCRUDService.Data.Repositories
{

    public class DiagramConnectionsRepository
    {
        private readonly ISession _session;
        private readonly IServiceScopeFactory _scopeFactory;
        private IList<OrgChartConnection> _connetctions;

        public DiagramConnectionsRepository(IHttpContextAccessor httpContextAccessor, IServiceScopeFactory scopeFactory)
        {
            _session = httpContextAccessor.HttpContext.Session;
            _scopeFactory = scopeFactory;
        }

        public IList<OrgChartConnection> All()
        {
            if (_connetctions == null)
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<DemoDbContext>();
                    _connetctions = context.OrgChartConnections.ToList();
                }
            }

            return _connetctions;
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
            var first = All().OrderByDescending(e => e.Id).FirstOrDefault();

            long id = 0;

            if (first != null)
            {
                id = first.Id;
            }

            connection.Id = id + 1;

            All().Insert(0, connection);
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
                All().Remove(target);
            }
        }
    }
}
