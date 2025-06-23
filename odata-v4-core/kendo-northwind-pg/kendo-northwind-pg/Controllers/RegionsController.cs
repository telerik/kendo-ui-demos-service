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
    public class RegionsController : ODataController
    {
        private readonly RegionsRepository _regions;

        public RegionsController(RegionsRepository regions)
        {
            _regions = regions;
        }

        // GET: odata/Regions
        [HttpGet]
        [EnableQuery]
        [Route("Regions")]
        public IEnumerable<Region> GetRegions()
        {
            return _regions.All();
        }

        // GET: odata/Regions(5)
        [HttpGet]
        [EnableQuery]
        [Route("Regions({key})")]
        public IEnumerable<Region> GetRegion([FromODataUri] int key)
        {
            return _regions.Where(region => region.RegionID == key);
        }

        // PUT: odata/Regions(5)
        [HttpPut]
        [Route("Regions({key})")]
        public IActionResult Put([FromODataUri] int key, Region region)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (key != region.RegionID)
            {
                return BadRequest();
            }

            _regions.Update(region);

            return Updated(region);
        }

        // POST: odata/Regions
        [HttpPost]
        [Route("Regions")]
        public IActionResult Post(Region region)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _regions.Insert(region);

            return Created(region);
        }

        // PATCH: odata/Regions(5)
        [AcceptVerbs("PATCH", "MERGE")]
        [Route("Regions({key})")]
        public IActionResult Patch([FromODataUri] int key, Delta<Region> patch)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Region region = _regions.Where(region => region.RegionID == key).First();

            if (region == null)
            {
                return NotFound();
            }

            patch.Patch(region);

            return Updated(region);
        }

        // DELETE: odata/Regions(5)
        [HttpDelete]
        [Route("Regions({key})")]
        public IActionResult Delete([FromODataUri] int key)
        {
            Region region = _regions.Where(region => region.RegionID == key).First();
            if (region == null)
            {
                return NotFound();
            }

            _regions.Delete(region);

            return StatusCode(204);
        }

        // GET: odata/Regions(5)/Territories
        [HttpGet]
        [EnableQuery]
        [Route("Regions({key})/Territories")]
        public IQueryable<Territory> GetTerritories([FromODataUri] int key)
        {
            return _regions.Where(m => m.RegionID == key).SelectMany(m => m.Territories).AsQueryable();
        }
    }
}
