using KendoCRUDService.Data.Models;
using KendoCRUDService.SessionExtensions;
using Microsoft.EntityFrameworkCore;

namespace KendoCRUDService.Data.Repositories
{

    public class DiagramConnectionsRepository
    {
        private bool UpdateDatabase = false;
        private readonly ISession _session;
        private readonly IDbContextFactory<DemoDbContext> _contextFactory;

        public DiagramConnectionsRepository(IHttpContextAccessor httpContextAccessor, IDbContextFactory<DemoDbContext> contextFactory)
        {
            _session = httpContextAccessor.HttpContext.Session;
            _contextFactory = contextFactory;
        }

        public IList<OrgChartConnection> All()
        {
            var result = _session.GetObjectFromJson<IList<OrgChartConnection>>("OrgChartConnections");

            if (result == null || UpdateDatabase)
            {
                using (var context = _contextFactory.CreateDbContext())
                {
                    result = context.OrgChartConnections.ToList();
                }

                _session.SetObjectAsJson("OrgChartConnections", result);
            }

            return result;
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
            if (!UpdateDatabase)
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
            else
            {
                using (var db = _contextFactory.CreateDbContext())
                {
                    db.OrgChartConnections.Add(connection);
                    db.SaveChanges();
                }
            }
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
            if (!UpdateDatabase)
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
            else
            {
                using (var db = _contextFactory.CreateDbContext())
                {
                    db.OrgChartConnections.Entry(connection).State = EntityState.Modified;
                }
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
            if (!UpdateDatabase)
            {
                var target = One(p => p.Id == connection.Id);
                if (target != null)
                {
                    All().Remove(target);
                }
            }
            else
            {
                using (var db  = _contextFactory.CreateDbContext())
                {
                    db.OrgChartConnections.Remove(connection);

                    db.SaveChanges();
                }
            }
        }
    }
}
