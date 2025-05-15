using KendoCRUDService.Data.Models;
using KendoCRUDService.Data.Repositories;
using KendoCRUDService.Models;
using KendoCRUDService.Models.Request;
using KendoCRUDService.Models.Response;
using KendoCRUDService.Extensions;
using System.Collections;
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

        public IActionResult DataSourceRead([FromBody]Request request)
        {
            var data = this._productRepository.All().AsQueryable();
            int total = data.Count();
            IList resultData;
            bool isGrouped = false;

            var aggregates = new Dictionary<string, Dictionary<string, string>>();
            
            if (request.Sorts != null)
            {
                data = data.Sort(request.Sorts);
            }

            if (request.Filter != null)
            {
                data = data.Filter(request.Filter);
                total = data.Count();
            }

            if (request.Aggregates != null)
            {
                aggregates = data.CalculateAggregates(request.Aggregates);
            }

            if (request.Take > 0)
            {
                data = data.Page(request.Skip, request.Take);
            }

            if (request.Groups != null && request.Groups.Count > 0 && !request.GroupPaging)
            {
                resultData = data.Group(request.Groups).Cast<Group>().ToList();
                isGrouped = true;
            }
            else
            {
                resultData = data.ToList();
            }

            var result = new Response(resultData, aggregates, total, isGrouped).ToResult();
            return Ok(result);
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
