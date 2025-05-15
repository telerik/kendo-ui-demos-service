using KendoCRUDService.Data.Models;
using KendoCRUDService.Models;
using KendoCRUDService.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;

namespace KendoCRUDService.Data.Repositories
{
    public class DiagramShapesRepository
    {
        private readonly ISession _session;
        private readonly IServiceScopeFactory _scopeFactory;
        private ConcurrentDictionary<string, IList<OrgChartShape>> _shapes;
        private IHttpContextAccessor _contextAccessor;

        public DiagramShapesRepository(IHttpContextAccessor httpContextAccessor, IServiceScopeFactory scopeFactory)
        {
            _session = httpContextAccessor.HttpContext.Session;
            _contextAccessor = httpContextAccessor;
            _scopeFactory = scopeFactory;
            _shapes = new ConcurrentDictionary<string, IList<OrgChartShape>>();
        }

        public IList<OrgChartShape> All()
        {
            var userKey = SessionUtils.GetUserKey(_contextAccessor);

            return _shapes.GetOrAdd(userKey, key =>
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<DemoDbContext>();
                    return context.OrgChartShapes.ToList();
                }
            });
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
