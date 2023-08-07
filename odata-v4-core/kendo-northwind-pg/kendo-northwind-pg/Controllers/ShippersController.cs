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
    public class ShippersController : ODataController
    {
        private readonly DemoDbContext db;

        public ShippersController(DemoDbContext demoDbContext)
        {
            db = demoDbContext;
        }

        // GET: odata/Shippers
        [HttpGet]
        [EnableQuery]
        [Route("[controller]")]
        public IQueryable<Shipper> GetShippers()
        {
            return db.Shippers;
        }

        // GET: odata/Shippers(5)
        [HttpGet]
        [EnableQuery]
        [Route("[controller]({key})")]
        public SingleResult<Shipper> GetShipper([FromODataUri] int key)
        {
            return SingleResult.Create(db.Shippers.Where(shipper => shipper.ShipperID == key));
        }

        // PUT: odata/Shippers(5)
        [HttpPut]
        [Route("[controller]({key})")]
        public IActionResult Put([FromODataUri] int key, Shipper shipper)
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
        [HttpPost]
        [Route("[controller]")]
        public IActionResult Post(Shipper shipper)
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
        [Route("[controller]({key})")]
        public IActionResult Patch([FromODataUri] int key, Delta<Shipper> patch)
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
        [HttpDelete]
        [Route("[controller]({key})")]
        public IActionResult Delete([FromODataUri] int key)
        {
            Shipper shipper = db.Shippers.Find(key);
            if (shipper == null)
            {
                return NotFound();
            }

            db.Shippers.Remove(shipper);
            db.SaveChanges();

            return StatusCode(204);
        }

        // GET: odata/Shippers(5)/Orders
        [HttpGet]
        [EnableQuery]
        [Route("[controller]({key})/Orders")]
        public IQueryable<Order> GetOrders([FromODataUri] int key)
        {
            return db.Shippers.Where(m => m.ShipperID == key).SelectMany(m => m.Orders);
        }

        private bool ShipperExists(int key)
        {
            return db.Shippers.Count(e => e.ShipperID == key) > 0;
        }
    }
}
