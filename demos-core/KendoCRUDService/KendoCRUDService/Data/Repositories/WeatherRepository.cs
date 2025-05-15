using KendoCRUDService.Data.Models;
using KendoCRUDService.Extensions;
using Microsoft.EntityFrameworkCore;

namespace KendoCRUDService.Data.Repositories
{
    public class WeatherRepository
    {
        private readonly ISession _session;
        private readonly IDbContextFactory<DemoDbContext> _contextFactory;

        public WeatherRepository(IHttpContextAccessor httpContextAccessor, IDbContextFactory<DemoDbContext> contextFactory)
        {
            _session = httpContextAccessor.HttpContext.Session;
            _contextFactory = contextFactory;
        }

        public IList<WeatherModel> ByStation(string station)
        {
            using (var db = _contextFactory.CreateDbContext())
            {
                return _contextFactory.CreateDbContext().Weather.Select(w => new WeatherModel
                     {
                         Date = w.Date,
                         TMax = w.TMax,
                         TMin = w.TMin,
                         Rain = w.Rain,
                         Wind = w.Wind
                     }).ToList();
            }
        }

        public IList<WeatherModel> ByMonth(string station, int year, int month)
        {
            using (var db = _contextFactory.CreateDbContext())
            {
                return
                    ByStation(station)
                        .Where(w => w.Date.Year == year && w.Date.Month == month)
                        .ToList();
            }
        }
    }
}
