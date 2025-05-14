using KendoCRUDService.Data.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace KendoCRUDService.Controllers
{
    public class CountriesController : Controller
    {
        private readonly CountryRepository _countryRepository;

        public CountriesController(CountryRepository countryRepository)
        {
            _countryRepository = countryRepository;
        }

        public ActionResult Index()
        {
            return Json(_countryRepository.All());
        }
    }
}
