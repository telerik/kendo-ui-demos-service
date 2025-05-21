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
    public class RegionsController : ODataController
    {
        private readonly DemoDbContext db;

        public RegionsController(DemoDbContext demoDbContext)
        {
            db = demoDbContext;
        }

        // GET: odata/Regions
        [HttpGet]
        [EnableQuery]
        [Route("Regions")]
        public IQueryable<Region> GetRegions()
        {
            return db.Region;
        }

        // GET: odata/Regions(5)
        [HttpGet]
        [EnableQuery]
        [Route("Regions({key})")]
        public IEnumerable<Region> GetRegion([FromODataUri] int key)
        {
            return db.Region.Where(region => region.RegionID == key);
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

            db.Entry(region).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RegionExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

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

            db.Region.Add(region);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (RegionExists(region.RegionID))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

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

            Region region = db.Region.Find(key);
            
            if (region == null)
            {
                return NotFound();
            }

            patch.Patch(region);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RegionExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(region);
        }

        // DELETE: odata/Regions(5)
        [HttpDelete]
        [Route("Regions({key})")]
        public IActionResult Delete([FromODataUri] int key)
        {
            Region region = db.Region.Find(key);
            if (region == null)
            {
                return NotFound();
            }

            db.Region.Remove(region);
            db.SaveChanges();

            return StatusCode(204);
        }

        // GET: odata/Regions(5)/Territories
        [HttpGet]
        [EnableQuery]
        [Route("Regions({key})/Territories")]
        public IQueryable<Territory> GetTerritories([FromODataUri] int key)
        {
            return db.Region.Where(m => m.RegionID == key).SelectMany(m => m.Territories);
        }

        private bool RegionExists(int key)
        {
            return db.Region.Count(e => e.RegionID == key) > 0;
        }
    }
}
