using KendoCoreService.Extensions;
using KendoCoreService.Interfaces;
using KendoCoreService.Models.Request;
using KendoCoreService.Models.Response;
using Microsoft.AspNetCore.Mvc;
using System.Collections;

namespace KendoCoreService.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository _products;

        public ProductsController(IProductRepository products)
        {
            _products = products;
        }

        [HttpPost]
        public IActionResult Read([FromBody]Request request)
        {
            var data = this._products.All().AsQueryable();
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
    }
}
