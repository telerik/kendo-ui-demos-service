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
    public class OrdersController : ODataController
    {
        private readonly DemoDbContext db;

        public OrdersController(DemoDbContext demoDbContext)
        {
            db = demoDbContext;
        }

        // GET: odata/Orders
        [HttpGet]
        [EnableQuery]
        [Route("odata/[controller]")]
        public IQueryable<Order> GetOrders()
        {
            return db.Orders;
        }

        // GET: odata/Orders(5)
        [HttpGet]
        [EnableQuery]
        [Route("odata/[controller]({key})")]
        public SingleResult<Order> GetOrder([FromODataUri] int key)
        {
            return SingleResult.Create(db.Orders.Where(order => order.OrderID == key));
        }

        // PUT: odata/Orders(5)
        [HttpPut]
        [Route("odata/[controller]({key})")]
        public IActionResult Put([FromODataUri] int key, Order order)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (key != order.OrderID)
            {
                return BadRequest();
            }

            db.Entry(order).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(order);
        }

        // POST: odata/Orders
        [HttpPost]
        [Route("odata/[controller")]
        public IActionResult Post(Order order)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Orders.Add(order);
            db.SaveChanges();

            return Created(order);
        }

        // PATCH: odata/Orders(5)
        [AcceptVerbs("PATCH", "MERGE")]
        [Route("odata/[controller]({key})")]
        public IActionResult Patch([FromODataUri] int key, Delta<Order> patch)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Order order = db.Orders.Find(key);
            if (order == null)
            {
                return NotFound();
            }

            patch.Patch(order);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(order);
        }

        // DELETE: odata/Orders(5)
        [HttpDelete]
        [Route("odata/[controller]({key})")]
        public IActionResult Delete([FromODataUri] int key)
        {
            Order order = db.Orders.Find(key);
            if (order == null)
            {
                return NotFound();
            }

            db.Orders.Remove(order);
            db.SaveChanges();

            return StatusCode(204);
        }

        // GET: odata/Orders(5)/Customer
        [HttpGet]
        [EnableQuery]
        [Route("odata/[controller]({key})/Customer")]
        public SingleResult<Customer> GetCustomer([FromODataUri] int key)
        {
            return SingleResult.Create(db.Orders.Where(m => m.OrderID == key).Select(m => m.Customer));
        }

        // GET: odata/Orders(5)/Employee
        [HttpGet]
        [EnableQuery]
        [Route("odata/[controller]({key})/Employee")]
        public SingleResult<Employee> GetEmployee([FromODataUri] int key)
        {
            return SingleResult.Create(db.Orders.Where(m => m.OrderID == key).Select(m => m.Employee));
        }

        // GET: odata/Orders(5)/Order_Details
        [HttpGet]
        [EnableQuery]
        [Route("odata/[controller]({key})/Order_Details")]
        public IQueryable<OrderDetail> GetOrder_Details([FromODataUri] int key)
        {
            return db.Orders.Where(m => m.OrderID == key).SelectMany(m => m.OrderDetails);
        }

        // GET: odata/Orders(5)/Shipper
        [HttpGet]
        [EnableQuery]
        [Route("odata/[controller]({key})/Shipper")]
        public SingleResult<Shipper> GetShipper([FromODataUri] int key)
        {
            return SingleResult.Create(db.Orders.Where(m => m.OrderID == key).Select(m => m.ShipViaNavigation));
        }

        private bool OrderExists(int key)
        {
            return db.Orders.Count(e => e.OrderID == key) > 0;
        }
    }
}
