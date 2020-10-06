using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KendoCRUDService.Models;
using KendoCRUDService.Common;
using System.Web.Script.Serialization;

namespace KendoCRUDService.Controllers
{
    public class DetailProductsController : Controller
    {
        public ActionResult Index()
        {
            return this.Jsonp(DetailProductRepository.All());
        }                       
        
        public JsonResult Update()
        {
            var models = this.DeserializeObject<IEnumerable<DetailProductModel>>("models");
            if (models != null)
            {
                DetailProductRepository.Update(models);
            }
            return this.Jsonp(models);
        }
        
        public ActionResult Destroy()
        {
            var products = this.DeserializeObject<IEnumerable<DetailProductModel>>("models");

            if (products != null)
            {
                DetailProductRepository.Delete(products);
            }
            return this.Jsonp(products);
        }
        
        public ActionResult Create()
        {
            var products = this.DeserializeObject<IEnumerable<DetailProductModel>>("models");
            if (products != null)
            {
                DetailProductRepository.Insert(products);
            }
            return this.Jsonp(products);
        }

        public JsonResult Read(int skip, int take)
        {
            IEnumerable<DetailProductModel> result = DetailProductRepository.All().OrderByDescending(p => p.ProductID);
            
            result = result.Skip(skip).Take(take);

            return this.Jsonp(result);
        }

        public JsonResult Submit()
        {
            var model = this.DeserializeObject<DetailProductSubmitViewModel>("models");

            if (model != null && model.Created != null)
            {
                DetailProductRepository.Insert(model.Created);
            }

            if (model != null && model.Updated != null)
            {
                DetailProductRepository.Update(model.Updated);
            }

            if (model != null && model.Destroyed != null)
            {
                DetailProductRepository.Delete(model.Destroyed);
            }

            return this.Jsonp(model);
        }
    }
}
