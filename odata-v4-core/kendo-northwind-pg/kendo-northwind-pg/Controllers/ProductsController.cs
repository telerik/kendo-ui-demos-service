using kendo_northwind_pg.Data;
using kendo_northwind_pg.Data.Models;
using kendo_northwind_pg.Data.Repositories;
using kendo_northwind_pg.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System.Net;

namespace kendo_northwind_pg.Controllers
{
    public class ProductsController : ODataController
    {
        private readonly ProductRepository _productRepository;

        public ProductsController(ProductRepository productRepository)
        {
            _productRepository = productRepository;
        }


        // GET: odata/Products
        [HttpGet]
        [EnableQuery]
        [Route("Products")]
        public IEnumerable<Product> GetProducts()
        {
            return _productRepository.All();
        }

        // GET: odata/Products(5)
        [HttpGet]
        [EnableQuery]
        [Route("Products({key})")]
        public IEnumerable<Product> GetProduct([FromODataUri] int key)
        {
            return _productRepository.Where(product => product.ProductID == key);
        }

        // PUT: odata/Products(5)
        [HttpPut]
        [Route("Products({key})")]
        public IActionResult Put([FromODataUri] int key, [FromBody] Product product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (key != product.ProductID)
            {
                return BadRequest();
            }

            _productRepository.Update(product);

            return Updated(product);
        }

        // POST: odata/Products
        [HttpPost]
        [Route("Products")]
        public IActionResult Post([FromBody] Product product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _productRepository.Insert(product);

            return Created(product);
        }

        // PATCH: odata/Products(5)
        [AcceptVerbs("PATCH", "MERGE")]
        [Route("Products({key})")]
        public IActionResult Patch([FromODataUri] int key, Delta<Product> patch)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Product product = _productRepository.Where(x=>x.ProductID == key).First();
            if (product == null)
            {
                return NotFound();
            }

            patch.Patch(product);

            return Updated(product);
        }

        // DELETE: odata/Products(5)

        [HttpDelete]
        [Route("Products({key})")]
        public IActionResult Delete([FromODataUri] int key)
        {
            Product product = _productRepository.Where(x => x.ProductID == key).First();
            if (product == null)
            {
                return NotFound();
            }

            _productRepository.Delete(product);

            return StatusCode(204);
        }

        // GET: odata/Products(5)/Category
        [HttpGet]
        [EnableQuery]
        [Route("Products({key})/Category")]
        public IQueryable<Category> GetCategory([FromODataUri] int key)
        {
            return _productRepository.Where(m => m.ProductID == key).Select(m => m.Category).AsQueryable();
        }

        // GET: odata/Products(5)/Order_Details
        [HttpGet]
        [EnableQuery]
        [Route("Products({key})/Order_Details")]
        public IQueryable<OrderDetail> GetOrder_Details([FromODataUri] int key)
        {
            return _productRepository.Where(m => m.ProductID == key).SelectMany(m => m.OrderDetails).AsQueryable();
        }

        // GET: odata/Products(5)/Supplier
        [HttpGet]
        [EnableQuery]
        [Route("Products({key})/Supplier")]
        public IQueryable<Supplier> GetSupplier([FromODataUri] int key)
        {
            return _productRepository.Where(m => m.ProductID == key).Select(m => m.Supplier).AsQueryable();
        }
    }
}
