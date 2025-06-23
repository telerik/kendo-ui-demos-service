using kendo_northwind_pg.Data;
using kendo_northwind_pg.Data.Models;
using kendo_northwind_pg.Data.Repositories;
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
        private readonly SuppliersRepository _suppliers;

        public SuppliersController(SuppliersRepository suppliers)
        {
            _suppliers = suppliers;
        }

        // GET: odata/Suppliers
        [HttpGet]
        [EnableQuery]
        [Route("Suppliers")]
        public IEnumerable<Supplier> GetSuppliers()
        {
            return _suppliers.All();
        }

        // GET: odata/Suppliers(5)
        [HttpGet]
        [EnableQuery]
        [Route("Suppliers({key})")]
        public IEnumerable<Supplier> GetSupplier([FromODataUri] int key)
        {
            return _suppliers.Where(supplier => supplier.SupplierID == key);
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

            _suppliers.Update(supplier);

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

            _suppliers.Insert(supplier);

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

            Supplier supplier = _suppliers.Where(supplier => supplier.SupplierID == key).First();
            if (supplier == null)
            {
                return NotFound();
            }

            patch.Patch(supplier);

            return Updated(supplier);
        }

        // DELETE: odata/Suppliers(5)
        [HttpDelete]
        [Route("Suppliers({key})")]
        public IActionResult Delete([FromODataUri] int key)
        {
            Supplier supplier = _suppliers.Where(supplier => supplier.SupplierID == key).First();
            if (supplier == null)
            {
                return NotFound();
            }

            _suppliers.Delete(supplier);

            return StatusCode(204);
        }

        // GET: odata/Suppliers(5)/Products
        [HttpGet]
        [EnableQuery]
        [Route("Suppliers({key})/Products")]
        public IQueryable<Product> GetProducts([FromODataUri] int key)
        {
            return _suppliers.Where(m => m.SupplierID == key).SelectMany(m => m.Products).AsQueryable();
        }
    }
}
