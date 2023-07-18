using KendoCRUDService.Data.Models;
using KendoCRUDService.SessionExtensions;
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
            var sessionKey = "w_byStation_" + station;
            var result = _session.GetObjectFromJson<IList<WeatherModel>>(sessionKey);

            if (result == null)
            {
                result =
                    _contextFactory.CreateDbContext().Weather.Select(w => new WeatherModel
                     {
                         Date = w.Date,
                         TMax = w.TMax,
                         TMin = w.TMin,
                         Rain = w.Rain,
                         Wind = w.Wind
                     }).ToList();

                _session.SetObjectAsJson(sessionKey, result);
            }

            return result;
        }

        public IList<WeatherModel> ByMonth(string station, int year, int month)
        {
            var sessionKey = "w_byMonth_" + station + year + month;
            var result = _session.GetObjectFromJson<IList<WeatherModel>>(sessionKey);

            if (result == null)
            {
                using (var db = _contextFactory.CreateDbContext()) {
                    result =
                        ByStation(station)
                            .Where(w => w.Date.Year == year && w.Date.Month == month)
                            .ToList();

                    _session.SetObjectAsJson(sessionKey, result);
                }
            }

            return result;
        }
    }
}
