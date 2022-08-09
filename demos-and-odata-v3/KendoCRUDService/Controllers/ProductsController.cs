using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using KendoCRUDService.Models;
using KendoCRUDService.Common;
using KendoCRUDService.Models.Response;
using System.Collections;
using KendoCRUDService.Models.Request;
using Newtonsoft.Json;
using System.IO;

namespace KendoCRUDService.Controllers
{
    public class ProductsController : Controller
    {
        public ActionResult Index()
        {
            return this.Jsonp(ProductRepository.All());
        }                       
        
        public JsonResult Update()
        {
            var models = this.DeserializeObject<List<ProductModel>>("models");
            if (models != null)
            {
                ProductRepository.Update(models);
            }
            return this.Jsonp(models);
        }
        
        public ActionResult Destroy()
        {
            var products = this.DeserializeObject<List<ProductModel>>("models");

            if (products != null)
            {
                ProductRepository.Delete(products);
            }
            return this.Jsonp(products);
        }
        
        public ActionResult Create()
        {
            var products = this.DeserializeObject<List<ProductModel>>("models");
            if (products != null)
            {
                ProductRepository.Insert(products);
            }
            return this.Jsonp(products);
        }

        public JsonResult Read(int skip, int take)
        {
            IEnumerable<ProductModel> result = ProductRepository.All().OrderByDescending(p => p.ProductID);
            
            result = result.Skip(skip).Take(take);

            return this.Jsonp(result);
        }

        public JsonResult Submit()
        {
            var model = this.DeserializeObject<SpreadsheetSubmitViewModel>("models");

            if (model != null && model.Created != null)
            {
                ProductRepository.Insert(model.Created);
            }

            if (model != null && model.Updated != null)
            {
                ProductRepository.Update(model.Updated);
            }

            if (model != null && model.Destroyed != null)
            {
                ProductRepository.Delete(model.Destroyed);
            }

            return this.Jsonp(model);
        }

        [HttpPost]
        public ActionResult ServerRead()
        {
            Stream requestBody = Request.InputStream;
            requestBody.Seek(0, System.IO.SeekOrigin.Begin);
            string requestString = new StreamReader(requestBody).ReadToEnd();

            Request request = JsonConvert.DeserializeObject<Request>(requestString);
            IQueryable<ProductModel> data = ProductRepository.All().AsQueryable();

            int total;
            bool isGrouped = false;
            IList resultData;
            IList pagedData;
            var aggregates = new Dictionary<string, Dictionary<string, string>>();

            if (request.Sorts != null)
            {
                data = data.Sort(request.Sorts);
            }

            if (request.Filter != null)
            {
                data = data.Filter(request.Filter);
            }

            resultData = data.ToList();

            if (request.Aggregates != null)
            {
                aggregates = AggregateExtension.CalculateAggregates(request.Aggregates, resultData);
            }

            if (request.Groups != null && request.Groups.Count > 0)
            {
                resultData = GroupingExtension.ApplyGrouping<ProductModel>(resultData, request.Groups);
                pagedData = PagingExtension.ApplyPaging<Group>(request.Skip, request.Take, resultData);
                isGrouped = true;
            }
            else
            {
                pagedData = PagingExtension.ApplyPaging<ProductModel>(request.Skip, request.Take, resultData);
            }

            total = resultData.Count;
            var result = new Response(pagedData, aggregates, total, isGrouped).ToResult();

            return Content(JsonConvert.SerializeObject(result), "application/json");
        }
    }
}
