using KendoCRUDService.Data.Models;
using KendoCRUDService.Data.Repositories;
using KendoCRUDService.Models;
using Microsoft.AspNetCore.Mvc;
using SQLitePCL;

namespace KendoCRUDService.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ProductRepository _productRepository;

        public ProductsController(ProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public ActionResult Index()
        {
            return Json(_productRepository.All());
        }

        public JsonResult Update([FromBody]List<Product> models)
        {
            if (models != null)
            {
                _productRepository.Update(models);
            }
            return Json(models);
        }

        public ActionResult Destroy([FromBody]List<Product> models)
        {
            if (models != null)
            {
                _productRepository.Delete(models);
            }
            return Json(models);
        }

        public ActionResult Create([FromBody]List<Product> models)
        {
            if (models != null)
            {
                _productRepository.Insert(models);
            }
            return Json(models);
        }

        public JsonResult Read(int skip, int take)
        {
            var result = _productRepository.All().OrderByDescending(p => p.ProductID).Skip(skip).Take(take);

            return Json(result);
        }

        public JsonResult Submit(SpreadsheetSubmitViewModel models)
        {
            if (models != null && models.Created != null)
            {
                _productRepository.Insert(models.Created);
            }

            if (models != null && models.Updated != null)
            {
                _productRepository.Update(models.Updated);
            }

            if (models != null && models.Destroyed != null)
            {
                _productRepository.Delete(models.Destroyed);
            }

            return Json(models);
        }
    }
}
