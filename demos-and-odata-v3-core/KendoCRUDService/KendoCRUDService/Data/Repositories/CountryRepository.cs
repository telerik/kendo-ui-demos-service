using KendoCRUDService.Data.Models;
using KendoCRUDService.SessionExtensions;
using Microsoft.EntityFrameworkCore;

namespace KendoCRUDService.Data.Repositories
{
    public class CountryRepository
    {
        private readonly ISession _session;
        private readonly IDbContextFactory<DemoDbContext> _contextFactory;

        public CountryRepository(IHttpContextAccessor httpContextAccessor, IDbContextFactory<DemoDbContext> contextFactory)
        {
            _session = httpContextAccessor.HttpContext.Session;
            _contextFactory = contextFactory;
        }

        public IList<Country> All()
        {
            IList<Country> result = _session.GetObjectFromJson<IList<Country>>("Countries");

            if (result == null)
            {
                result = _contextFactory.CreateDbContext().Countries.Select(c => new Country
                {
                    CountryID = c.CountryID,
                    CountryNameLong = c.CountryNameLong,
                    CountryNameShort = c.CountryNameShort,
                }).ToList();
            }

            return result;
        }
    }
}
