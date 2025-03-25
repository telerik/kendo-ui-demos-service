using KendoCRUDService.Data.Models;
using KendoCRUDService.SessionExtensions;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace KendoCRUDService.Data.Repositories
{
    public class GanttResourcesRepository
    {
        private readonly ISession _session;
        private readonly IServiceScopeFactory _scopeFactory;
        private IList<GanttResource> _resources;

        public GanttResourcesRepository(IHttpContextAccessor httpContextAccessor, IServiceScopeFactory scopeFactory)
        {
            _session = httpContextAccessor.HttpContext.Session;
            _scopeFactory = scopeFactory;
        }

        public IList<GanttResource> All()
        {
            var result = _session.GetObjectFromJson<IList<GanttResource>>("GanttResources");

            if (_resources == null)
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<DemoDbContext>();
                    _resources = context.GanttResources.ToList();
                }
            }

            return _resources;
        }
    }
}
