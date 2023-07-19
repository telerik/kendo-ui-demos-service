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
    public class TerritoriesController : ODataController
    {
        private readonly DemoDbContext db;

        public TerritoriesController(DemoDbContext demoDbContext)
        {
            db = demoDbContext;
        }

        // GET: odata/Territories
        [HttpGet]
        [EnableQuery]
        [Route("odata/[controller]")]
        public IQueryable<Territory> GetTerritories()
        {
            return db.Territories;
        }

        // GET: odata/Territories(5)
        [HttpGet]
        [EnableQuery]
        [Route("odata/[controller]({key})")]
        public SingleResult<Territory> GetTerritory([FromODataUri] string key)
        {
            return SingleResult.Create(db.Territories.Where(territory => territory.TerritoryID == key));
        }

        // PUT: odata/Territories(5)
        [HttpPut]
        [Route("odata/[controller]({key})")]
        public IActionResult Put([FromODataUri] string key, Territory territory)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (key != territory.TerritoryID)
            {
                return BadRequest();
            }

            db.Entry(territory).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TerritoryExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(territory);
        }

        // POST: odata/Territories
        [HttpPost]
        [Route("odata/[controller]")]
        public IActionResult Post(Territory territory)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Territories.Add(territory);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (TerritoryExists(territory.TerritoryID))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return Created(territory);
        }

        // PATCH: odata/Territories(5)
        [AcceptVerbs("PATCH", "MERGE")]
        [Route("odata/[controller]({key})")]
        public IActionResult Patch([FromODataUri] string key, Delta<Territory> patch)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Territory territory = db.Territories.Find(key);
            if (territory == null)
            {
                return NotFound();
            }

            patch.Patch(territory);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TerritoryExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(territory);
        }

        // DELETE: odata/Territories(5)
        [HttpDelete]
        [Route("odata/[controller]({key})")]
        public IActionResult Delete([FromODataUri] string key)
        {
            Territory territory = db.Territories.Find(key);
            if (territory == null)
            {
                return NotFound();
            }

            db.Territories.Remove(territory);
            db.SaveChanges();

            return StatusCode(204);
        }

        // GET: odata/Territories(5)/Region
        [HttpGet]
        [EnableQuery]
        [Route("odata/[controller]({key})/Region")]
        public SingleResult<Region> GetRegion([FromODataUri] string key)
        {
            return SingleResult.Create(db.Territories.Where(m => m.TerritoryID == key).Select(m => m.Region));
        }

        // GET: odata/Territories(5)/Employees
        [HttpGet]
        [EnableQuery]
        [Route("odata/[controller]({key})/Employees")]
        public IQueryable<Employee> GetEmployees([FromODataUri] string key)
        {
            return db.Territories.Where(m => m.TerritoryID == key).SelectMany(m => m.EmployeeTerritories.Select(x=> x.Employee));
        }

        private bool TerritoryExists(string key)
        {
            return db.Territories.Count(e => e.TerritoryID == key) > 0;
        }
    }
}
