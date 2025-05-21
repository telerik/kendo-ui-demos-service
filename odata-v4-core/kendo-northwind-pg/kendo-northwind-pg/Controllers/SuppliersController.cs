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
    public class SuppliersController : ODataController
    {
        private readonly DemoDbContext db;

        public SuppliersController(DemoDbContext demoDbContext)
        {
            db = demoDbContext;
        }

        // GET: odata/Suppliers
        [HttpGet]
        [EnableQuery]
        [Route("Suppliers")]
        public IQueryable<Supplier> GetSuppliers()
        {
            return db.Suppliers;
        }

        // GET: odata/Suppliers(5)
        [HttpGet]
        [EnableQuery]
        [Route("Suppliers({key})")]
        public IQueryable<Supplier> GetSupplier([FromODataUri] int key)
        {
            return db.Suppliers.Where(supplier => supplier.SupplierID == key);
        }

        // PUT: odata/Suppliers(5)
        [HttpPut]
        [Route("Suppliers({key})")]
        public IActionResult Put([FromODataUri] int key, Supplier supplier)
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
        [HttpPost]
        [Route("Suppliers")]
        public IActionResult Post(Supplier supplier)
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
        [Route("Suppliers({key})")]
        public IActionResult Patch([FromODataUri] int key, Delta<Supplier> patch)
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
        [HttpDelete]
        [Route("Suppliers({key})")]
        public IActionResult Delete([FromODataUri] int key)
        {
            Supplier supplier = db.Suppliers.Find(key);
            if (supplier == null)
            {
                return NotFound();
            }

            db.Suppliers.Remove(supplier);
            db.SaveChanges();

            return StatusCode(204);
        }

        // GET: odata/Suppliers(5)/Products
        [HttpGet]
        [EnableQuery]
        [Route("Suppliers({key})/Products")]
        public IQueryable<Product> GetProducts([FromODataUri] int key)
        {
            return db.Suppliers.Where(m => m.SupplierID == key).SelectMany(m => m.Products);
        }

        private bool SupplierExists(int key)
        {
            return db.Suppliers.Count(e => e.SupplierID == key) > 0;
        }
    }
}
