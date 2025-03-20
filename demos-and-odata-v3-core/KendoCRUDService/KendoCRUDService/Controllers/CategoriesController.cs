using KendoCRUDService.Data.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace KendoCRUDService.Controllers
{
    public class Categories : Controller
    {
        private readonly CategoriesRepository _categoriesRepository;

        public Categories(CategoriesRepository orderRepository)
        {
            _categoriesRepository = orderRepository;
        }

        public ActionResult Index()
        {
            return Json(_categoriesRepository.All());
        }
    }
}
