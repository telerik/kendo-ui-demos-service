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
    builder.EntitySet<Shipper>("Shippers");
    builder.EntitySet<Order>("Orders"); 
    config.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ShippersController : ODataController
    {
        private NorthwindEntities db = new NorthwindEntities();

        // GET: odata/Shippers
        [EnableQuery]
        public IQueryable<Shipper> GetShippers()
        {
            return db.Shippers;
        }

        // GET: odata/Shippers(5)
        [EnableQuery]
        public SingleResult<Shipper> GetShipper([FromODataUri] int key)
        {
            return SingleResult.Create(db.Shippers.Where(shipper => shipper.ShipperID == key));
        }

        // PUT: odata/Shippers(5)
        public IHttpActionResult Put([FromODataUri] int key, Shipper shipper)
        {        
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (key != shipper.ShipperID)
            {
                return BadRequest();
            }

            db.Entry(shipper).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ShipperExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(shipper);
        }

        // POST: odata/Shippers
        public IHttpActionResult Post(Shipper shipper)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Shippers.Add(shipper);
            db.SaveChanges();

            return Created(shipper);
        }

        // PATCH: odata/Shippers(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public IHttpActionResult Patch([FromODataUri] int key, Delta<Shipper> patch)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Shipper shipper = db.Shippers.Find(key);
            if (shipper == null)
            {
                return NotFound();
            }

            patch.Patch(shipper);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ShipperExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(shipper);
        }

        // DELETE: odata/Shippers(5)
        public IHttpActionResult Delete([FromODataUri] int key)
        {
            Shipper shipper = db.Shippers.Find(key);
            if (shipper == null)
            {
                return NotFound();
            }

            db.Shippers.Remove(shipper);
            db.SaveChanges();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // GET: odata/Shippers(5)/Orders
        [EnableQuery]
        public IQueryable<Order> GetOrders([FromODataUri] int key)
        {
            return db.Shippers.Where(m => m.ShipperID == key).SelectMany(m => m.Orders);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ShipperExists(int key)
        {
            return db.Shippers.Count(e => e.ShipperID == key) > 0;
        }
    }
}
