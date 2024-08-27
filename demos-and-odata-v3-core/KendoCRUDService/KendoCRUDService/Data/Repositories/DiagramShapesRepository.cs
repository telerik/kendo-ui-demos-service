using KendoCRUDService.Data.Models;
using KendoCRUDService.SessionExtensions;
using Microsoft.EntityFrameworkCore;

namespace KendoCRUDService.Data.Repositories
{
    public class DiagramShapesRepository
    {
        private static bool UpdateDatabase = false;
        private readonly ISession _session;
        private readonly IDbContextFactory<DemoDbContext> _contextFactory;

        public DiagramShapesRepository(IHttpContextAccessor httpContextAccessor, IDbContextFactory<DemoDbContext> contextFactory)
        {
            _session = httpContextAccessor.HttpContext.Session;
            _contextFactory = contextFactory;
        }

        public IList<OrgChartShape> All()
        {
            var result = _session.GetObjectFromJson<IList<OrgChartShape>>("OrgChartShapes");

            if (result == null || UpdateDatabase)
            {
                using (var context = _contextFactory.CreateDbContext())
                {
                    result = context.OrgChartShapes.ToList();
                }
                
                _session.SetObjectAsJson("OrgChartShapes", result);
            }

            return result;
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
            if (!UpdateDatabase)
            {
                var first = All().OrderByDescending(e => e.Id).FirstOrDefault();

                var id = 0;

                if (first != null)
                {
                    id = first.Id;
                }

                shape.Id = id + 1;

                All().Insert(0, shape);
            }
            else
            {
                using (var db = _contextFactory.CreateDbContext())
                {
                    db.OrgChartShapes.Add(shape);
                    db.SaveChanges();
                }
            }
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
            if (!UpdateDatabase)
            {
                var target = One(e => e.Id == shape.Id);

                if (target != null)
                {
                    target.JobTitle = shape.JobTitle;
                    target.Color = shape.Color;
                }
            }
            else
            {
                using (var db = _contextFactory.CreateDbContext())
                {
                    db.OrgChartShapes.Attach(shape);
                    db.SaveChanges();
                }
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
            if (!UpdateDatabase)
            {
                var target = One(p => p.Id == shape.Id);
                if (target != null)
                {
                    All().Remove(target);
                }
            }
            else
            {
                using (var db = _contextFactory.CreateDbContext())
                {
                    db.OrgChartShapes.Attach(shape);
                    db.SaveChanges();
                }
            }
        }
    }
}
