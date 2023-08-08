using KendoCRUDService.Data.Models;
using KendoCRUDService.SessionExtensions;
using Microsoft.EntityFrameworkCore;

namespace KendoCRUDService.Data.Repositories
{
    public class GanttResourcesRepository
    {
        private readonly ISession _session;
        private readonly IDbContextFactory<DemoDbContext> _contextFactory;

        public GanttResourcesRepository(IHttpContextAccessor httpContextAccessor, IDbContextFactory<DemoDbContext> contextFactory)
        {
            _session = httpContextAccessor.HttpContext.Session;
            _contextFactory = contextFactory;
        }

        public IList<GanttResource> All()
        {
            var result = _session.GetObjectFromJson<IList<GanttResource>>("GanttResources");

            if (result == null)
            {
                using (var context = _contextFactory.CreateDbContext())
                {
                    result = context.GanttResources.ToList();
                }

                _session.SetObjectAsJson("GanttResources", result);
            }

            return result;
        }
    }
}
