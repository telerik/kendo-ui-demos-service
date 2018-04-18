using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.ModelBinding;
using Microsoft.AspNet.OData;
using kendo_northwind_pg.Models;
using System.Web.Http.Cors;

namespace kendo_northwind_pg.Controllers
{
    /*
    The WebApiConfig class may require additional changes to add a route for this controller. Merge these statements into the Register method of the WebApiConfig class as applicable. Note that OData URLs are case sensitive.

    using System.Web.OData.Builder;
    using System.Web.OData.Extensions;
    using kendo_northwind_pg.Models;
    ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
    builder.EntitySet<Order_Detail>("Order_Detail");
    builder.EntitySet<Order>("Orders"); 
    builder.EntitySet<Product>("Products"); 
    config.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class Order_DetailController : ODataController
    {
        private NorthwindEntities db = new NorthwindEntities();

        // GET: odata/Order_Detail
        [EnableQuery]
        public IQueryable<Order_Detail> GetOrder_Detail()
        {
            return db.Order_Details;
        }

        // GET: odata/Order_Detail(5)
        [EnableQuery]
        public SingleResult<Order_Detail> GetOrder_Detail([FromODataUri] int key)
        {
            return SingleResult.Create(db.Order_Details.Where(order_Detail => order_Detail.OrderID == key));
        }

        // PUT: odata/Order_Detail(5)
        public IHttpActionResult Put([FromODataUri] int key, Order_Detail order_Detail)
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
        public IHttpActionResult Post(Order_Detail order_Detail)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Order_Details.Add(order_Detail);

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
        public IHttpActionResult Patch([FromODataUri] int key, Delta<Order_Detail> patch)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Order_Detail order_Detail = db.Order_Details.Find(key);
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
        public IHttpActionResult Delete([FromODataUri] int key)
        {
            Order_Detail order_Detail = db.Order_Details.Find(key);
            if (order_Detail == null)
            {
                return NotFound();
            }

            db.Order_Details.Remove(order_Detail);
            db.SaveChanges();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // GET: odata/Order_Detail(5)/Order
        [EnableQuery]
        public SingleResult<Order> GetOrder([FromODataUri] int key)
        {
            return SingleResult.Create(db.Order_Details.Where(m => m.OrderID == key).Select(m => m.Order));
        }

        // GET: odata/Order_Detail(5)/Product
        [EnableQuery]
        public SingleResult<Product> GetProduct([FromODataUri] int key)
        {
            return SingleResult.Create(db.Order_Details.Where(m => m.OrderID == key).Select(m => m.Product));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool Order_DetailExists(int key)
        {
            return db.Order_Details.Count(e => e.OrderID == key) > 0;
        }
    }
}
