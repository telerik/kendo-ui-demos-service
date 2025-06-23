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
    public class ShippersController : ODataController
    {
        private readonly ShippersRepository _shippers;

        public ShippersController(ShippersRepository shippers)
        {
            _shippers = shippers;
        }

        // GET: odata/Shippers
        [HttpGet]
        [EnableQuery]
        [Route("Shippers")]
        public IEnumerable<Shipper> GetShippers()
        {
            return _shippers.All();
        }

        // GET: odata/Shippers(5)
        [HttpGet]
        [EnableQuery]
        [Route("Shippers({key})")]
        public IEnumerable<Shipper> GetShipper([FromODataUri] int key)
        {
            return _shippers.Where(shipper => shipper.ShipperID == key);
        }

        // PUT: odata/Shippers(5)
        [HttpPut]
        [Route("Shippers({key})")]
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

            _shippers.Update(shipper);

            return Updated(shipper);
        }

        // POST: odata/Shippers
        [HttpPost]
        [Route("Shippers")]
        public IActionResult Post(Shipper shipper)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _shippers.Insert(shipper);

            return Created(shipper);
        }

        // PATCH: odata/Shippers(5)
        [AcceptVerbs("PATCH", "MERGE")]
        [Route("Shippers({key})")]
        public IActionResult Patch([FromODataUri] int key, Delta<Shipper> patch)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Shipper shipper = _shippers.Where(x=>x.ShipperID == key).FirstOrDefault();
            if (shipper == null)
            {
                return NotFound();
            }

            patch.Patch(shipper);

            return Updated(shipper);
        }

        // DELETE: odata/Shippers(5)
        [HttpDelete]
        [Route("Shippers({key})")]
        public IActionResult Delete([FromODataUri] int key)
        {
            Shipper shipper = _shippers.Where(x => x.ShipperID == key).FirstOrDefault();
            if (shipper == null)
            {
                return NotFound();
            }

            _shippers.Delete(shipper);

            return StatusCode(204);
        }

        // GET: odata/Shippers(5)/Orders
        [HttpGet]
        [EnableQuery]
        [Route("Shippers({key})/Orders")]
        public IQueryable<Order> GetOrders([FromODataUri] int key)
        {
            return _shippers.Where(m => m.ShipperID == key).SelectMany(m => m.Orders).AsQueryable();
        }
    }
}
