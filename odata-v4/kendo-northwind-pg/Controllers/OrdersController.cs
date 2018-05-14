using System.Data;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web.Http;
using Microsoft.AspNet.OData;
using kendo_northwind_pg.Models;
using System.Data.Entity;
using System.Web.Http.Cors;

namespace kendo_northwind_pg.Controllers
{
    /*
    The WebApiConfig class may require additional changes to add a route for this controller. Merge these statements into the Register method of the WebApiConfig class as applicable. Note that OData URLs are case sensitive.

    using System.Web.OData.Builder;
    using System.Web.OData.Extensions;
    using kendo_northwind_pg.Models;
    ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
    builder.EntitySet<Order>("Orders");
    builder.EntitySet<Customer>("Customers"); 
    builder.EntitySet<Employee>("Employees"); 
    builder.EntitySet<Order_Detail>("Order_Details"); 
    builder.EntitySet<Shipper>("Shippers"); 
    config.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    public class OrdersController : ODataController
    {
        private NorthwindEntities db = new NorthwindEntities();

        // GET: odata/Orders
        [EnableQuery]
        public IQueryable<Order> GetOrders()
        {
            return db.Orders;
        }

        // GET: odata/Orders(5)
        [EnableQuery]
        public SingleResult<Order> GetOrder([FromODataUri] int key)
        {
            return SingleResult.Create(db.Orders.Where(order => order.OrderID == key));
        }

        // PUT: odata/Orders(5)
        public IHttpActionResult Put([FromODataUri] int key, Order order)
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
        public IHttpActionResult Post(Order order)
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
        public IHttpActionResult Patch([FromODataUri] int key, Delta<Order> patch)
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
        public IHttpActionResult Delete([FromODataUri] int key)
        {
            Order order = db.Orders.Find(key);
            if (order == null)
            {
                return NotFound();
            }

            db.Orders.Remove(order);
            db.SaveChanges();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // GET: odata/Orders(5)/Customer
        [EnableQuery]
        public SingleResult<Customer> GetCustomer([FromODataUri] int key)
        {
            return SingleResult.Create(db.Orders.Where(m => m.OrderID == key).Select(m => m.Customer));
        }

        // GET: odata/Orders(5)/Employee
        [EnableQuery]
        public SingleResult<Employee> GetEmployee([FromODataUri] int key)
        {
            return SingleResult.Create(db.Orders.Where(m => m.OrderID == key).Select(m => m.Employee));
        }

        // GET: odata/Orders(5)/Order_Details
        [EnableQuery]
        public IQueryable<Order_Detail> GetOrder_Details([FromODataUri] int key)
        {
            return db.Orders.Where(m => m.OrderID == key).SelectMany(m => m.Order_Details);
        }

        // GET: odata/Orders(5)/Shipper
        [EnableQuery]
        public SingleResult<Shipper> GetShipper([FromODataUri] int key)
        {
            return SingleResult.Create(db.Orders.Where(m => m.OrderID == key).Select(m => m.Shipper));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool OrderExists(int key)
        {
            return db.Orders.Count(e => e.OrderID == key) > 0;
        }
    }
}
