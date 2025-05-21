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
    public class Order_DetailController : ODataController
    {
        private readonly DemoDbContext db;

        public Order_DetailController(DemoDbContext demoDbContext)
        {
            db = demoDbContext;
        }

        // GET: odata/Order_Detail
        [HttpGet]
        [EnableQuery]
        [Route("Order_Detail")]
        public IQueryable<OrderDetail> Get()
        {
            return db.OrderDetails;
        }

        // GET: odata/Order_Detail(5)
        [HttpGet]
        [EnableQuery]
        [Route("Order_Detail({key})")]
        public IQueryable<OrderDetail> GetOrder_Detail([FromODataUri] int key)
        {
            return db.OrderDetails.Where(order_Detail => order_Detail.OrderID == key);
        }

        // PUT: odata/Order_Detail(5)
        [HttpPut]
        [Route("Order_Detail({key})")]
        public IActionResult Put([FromODataUri] int key, OrderDetail order_Detail)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (key != order_Detail.OrderID)
            {
                return BadRequest();
            }

            db.Entry(order_Detail).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!Order_DetailExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(order_Detail);
        }

        // POST: odata/Order_Detail
        [HttpPost]
        [Route("Order_Detail")]
        public IActionResult Post(OrderDetail order_Detail)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.OrderDetails.Add(order_Detail);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (Order_DetailExists(order_Detail.OrderID))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return Created(order_Detail);
        }

        // PATCH: odata/Order_Detail(5)
        [AcceptVerbs("PATCH", "MERGE")]        
        [Route("Order_Detail({key})")]
        public IActionResult Patch([FromODataUri] int key, Delta<OrderDetail> patch)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            OrderDetail order_Detail = db.OrderDetails.Find(key);
            if (order_Detail == null)
            {
                return NotFound();
            }

            patch.Patch(order_Detail);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!Order_DetailExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(order_Detail);
        }

        // DELETE: odata/Order_Detail(5)
        [HttpDelete]
        [Route("Order_Detail({key})")]
        public IActionResult Delete([FromODataUri] int key)
        {
            OrderDetail order_Detail = db.OrderDetails.Find(key);
            if (order_Detail == null)
            {
                return NotFound();
            }

            db.OrderDetails.Remove(order_Detail);
            db.SaveChanges();

            return StatusCode(204);
        }

        // GET: odata/Order_Detail(5)/Order
        [HttpGet]
        [EnableQuery]
        [Route("Order_Detail({key})/Order")]
        public IQueryable<Order> GetOrder([FromODataUri] int key)
        {
            return db.OrderDetails.Where(m => m.OrderID == key).Select(m => m.Order);
        }

        // GET: odata/Order_Detail(5)/Product
        [HttpGet]
        [EnableQuery]
        [Route("Order_Detail({key})/Product")]
        public IQueryable<Product> GetProduct([FromODataUri] int key)
        {
            return db.OrderDetails.Where(m => m.OrderID == key).Select(m => m.Product);
        }

        private bool Order_DetailExists(int key)
        {
            return db.OrderDetails.Count(e => e.OrderID == key) > 0;
        }
    }
}
