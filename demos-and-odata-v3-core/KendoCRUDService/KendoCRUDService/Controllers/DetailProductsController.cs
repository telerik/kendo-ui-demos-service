using KendoCRUDService.Data.Models;
using KendoCRUDService.Data.Repositories;
using KendoCRUDService.Models;
using Microsoft.AspNetCore.Mvc;

namespace KendoCRUDService.Controllers
{
    public class DetailProductsController : Controller
    {
        private readonly DetailProductRepository _detailProductRepository;

        public DetailProductsController(DetailProductRepository detailProductRepository)
        {
            _detailProductRepository = detailProductRepository;
        }
        public ActionResult Index()
        {
            return Json(_detailProductRepository.All());
        }

        public JsonResult Update([FromBody] IEnumerable<DetailProduct> models)
        {
            if (models != null)
            {
                _detailProductRepository.Update(models);
            }
            return Json(models);
        }

        public ActionResult Destroy([FromBody] IEnumerable<DetailProduct> models)
        {
            if (models != null)
            {
                _detailProductRepository.Delete(models);
            }
            return Json(models);
        }

        public ActionResult Create([FromBody] IEnumerable<DetailProduct> models)
        {
            if (models != null)
            {
                _detailProductRepository.Insert(models);
            }
            return Json(models);
        }

        public JsonResult Read(int skip, int take)
        {
            var result = _detailProductRepository.All()
                .OrderByDescending(p => p.ProductID)
                .Skip(skip)
                .Take(take);

            return Json(result);
        }

        public JsonResult Submit(DetailProductSubmitViewModel model)
        {
            if (model != null && model.Created != null)
            {
                _detailProductRepository.Insert(model.Created);
            }

            if (model != null && model.Updated != null)
            {
                _detailProductRepository.Update(model.Updated);
            }

            if (model != null && model.Destroyed != null)
            {
                _detailProductRepository.Delete(model.Destroyed);
            }

            return Json(model);
        }
    }
}
