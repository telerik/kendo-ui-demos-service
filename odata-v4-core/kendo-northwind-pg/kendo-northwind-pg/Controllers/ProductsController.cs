using kendo_northwind_pg.Data;
using kendo_northwind_pg.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace kendo_northwind_pg.Controllers
{
    public class ProductsController : ODataController
    {
        private readonly DemoDbContext db;

        public ProductsController(DemoDbContext demoDbContext)
        {
            db = demoDbContext;
        }

        // GET: odata/Products
        [HttpGet]
        [EnableQuery]
        [Route("[controller]")]
        public IQueryable<Product> GetProducts()
        {
            return db.Products;
        }

        // GET: odata/Products(5)
        [HttpGet]
        [EnableQuery]
        [Route("[controller]({key})")]
        public IQueryable<Product> GetProduct([FromODataUri] int key)
        {
            return db.Products.Where(product => product.ProductID == key);
        }

        // PUT: odata/Products(5)
        [HttpPut]
        [Route("odata/Products({key})")]
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

            db.Entry(product).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(product);
        }

        // POST: odata/Products
        [HttpPost]
        [Route("odata/Products")]
        public IActionResult Post([FromBody] Product product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Products.Add(product);
            db.SaveChanges();

            return Created(product);
        }

        // PATCH: odata/Products(5)
        [AcceptVerbs("PATCH", "MERGE")]
        [Route("odata/Products({key})")]
        public IActionResult Patch([FromODataUri] int key, Delta<Product> patch)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Product product = db.Products.Find(key);
            if (product == null)
            {
                return NotFound();
            }

            patch.Patch(product);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(product);
        }

        // DELETE: odata/Products(5)

        [HttpDelete]
        [Route("odata/Products({key})")]
        public IActionResult Delete([FromODataUri] int key)
        {
            Product product = db.Products.Find(key);
            if (product == null)
            {
                return NotFound();
            }

            db.Products.Remove(product);
            db.SaveChanges();

            return StatusCode(204);
        }

        // GET: odata/Products(5)/Category
        [HttpGet]
        [EnableQuery]
        [Route("odata/Products({key})/Category")]
        public SingleResult<Category> GetCategory([FromODataUri] int key)
        {
            return SingleResult.Create(db.Products.Where(m => m.ProductID == key).Select(m => m.Category));
        }

        // GET: odata/Products(5)/Order_Details
        [HttpGet]
        [EnableQuery]
        [Route("odata/Products({key})/Order_Details")]
        public IQueryable<OrderDetail> GetOrder_Details([FromODataUri] int key)
        {
            return db.Products.Where(m => m.ProductID == key).SelectMany(m => m.OrderDetails);
        }

        // GET: odata/Products(5)/Supplier
        [HttpGet]
        [EnableQuery]
        [Route("odata/Products({key})/Supplier")]
        public SingleResult<Supplier> GetSupplier([FromODataUri] int key)
        {
            return SingleResult.Create(db.Products.Where(m => m.ProductID == key).Select(m => m.Supplier));
        }

        private bool ProductExists(int key)
        {
            return db.Products.Count(e => e.ProductID == key) > 0;
        }
    }
}
