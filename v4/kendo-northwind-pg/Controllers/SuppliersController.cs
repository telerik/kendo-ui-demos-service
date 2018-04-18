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
    builder.EntitySet<Supplier>("Suppliers");
    builder.EntitySet<Product>("Products"); 
    config.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class SuppliersController : ODataController
    {
        private NorthwindEntities db = new NorthwindEntities();

        // GET: odata/Suppliers
        [EnableQuery]
        public IQueryable<Supplier> GetSuppliers()
        {
            return db.Suppliers;
        }

        // GET: odata/Suppliers(5)
        [EnableQuery]
        public SingleResult<Supplier> GetSupplier([FromODataUri] int key)
        {
            return SingleResult.Create(db.Suppliers.Where(supplier => supplier.SupplierID == key));
        }

        // PUT: odata/Suppliers(5)
        public IHttpActionResult Put([FromODataUri] int key, Supplier supplier)
        {        
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (key != supplier.SupplierID)
            {
                return BadRequest();
            }

            db.Entry(supplier).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SupplierExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(supplier);
        }

        // POST: odata/Suppliers
        public IHttpActionResult Post(Supplier supplier)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Suppliers.Add(supplier);
            db.SaveChanges();

            return Created(supplier);
        }

        // PATCH: odata/Suppliers(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public IHttpActionResult Patch([FromODataUri] int key, Delta<Supplier> patch)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Supplier supplier = db.Suppliers.Find(key);
            if (supplier == null)
            {
                return NotFound();
            }

            patch.Patch(supplier);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SupplierExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(supplier);
        }

        // DELETE: odata/Suppliers(5)
        public IHttpActionResult Delete([FromODataUri] int key)
        {
            Supplier supplier = db.Suppliers.Find(key);
            if (supplier == null)
            {
                return NotFound();
            }

            db.Suppliers.Remove(supplier);
            db.SaveChanges();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // GET: odata/Suppliers(5)/Products
        [EnableQuery]
        public IQueryable<Product> GetProducts([FromODataUri] int key)
        {
            return db.Suppliers.Where(m => m.SupplierID == key).SelectMany(m => m.Products);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool SupplierExists(int key)
        {
            return db.Suppliers.Count(e => e.SupplierID == key) > 0;
        }
    }
}
