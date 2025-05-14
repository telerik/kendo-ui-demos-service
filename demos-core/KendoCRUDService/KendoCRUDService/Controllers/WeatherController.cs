using KendoCRUDService.Data.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace KendoCRUDService.Controllers
{
    public class WeatherController : Controller
    {
        private readonly WeatherRepository _weatherRepository;

        public WeatherController(WeatherRepository weatherRepository)
        {
            _weatherRepository = weatherRepository;
        }

        //
        // GET: /Weather/SOFIA

        [Route("/[controller]/{station}")]
        public ActionResult Index(string station)
        {
            return Json(_weatherRepository.ByStation(station));
        }

        //
        // GET: /Weather/SOFIA/2012/1
        // (January)
        [Route("/[controller]/{station}/{year}/{month}")]
        public ActionResult ByMonth(string station, int year, int month)
        {
            return Json(_weatherRepository.ByMonth(station, year, month));
        }
    }
}
