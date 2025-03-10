using KendoCRUDService.Data.Models;
using KendoCRUDService.Models;
using KendoCRUDService.SessionExtensions;
using Microsoft.EntityFrameworkCore;

namespace KendoCRUDService.Data.Repositories
{
    public class DiagramShapesRepository
    {
        private readonly ISession _session;
        private readonly IServiceScopeFactory _scopeFactory;
        private IList<OrgChartShape> _shapes;

        public DiagramShapesRepository(IHttpContextAccessor httpContextAccessor, IServiceScopeFactory scopeFactory)
        {
            _session = httpContextAccessor.HttpContext.Session;
            _scopeFactory = scopeFactory;
        }

        public IList<OrgChartShape> All()
        {
            if (_shapes == null)
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<DemoDbContext>();
                    _shapes = context.OrgChartShapes.ToList();
                }
                
            }

            return _shapes;
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
            var first = All().OrderByDescending(e => e.Id).FirstOrDefault();

            var id = 0;

            if (first != null)
            {
                id = first.Id;
            }

            shape.Id = id + 1;

            All().Insert(0, shape);
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
                All().Remove(target);
            }
        }
    }
}
