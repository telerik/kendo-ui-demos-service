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
    public class TerritoriesController : ODataController
    {
        private readonly TerritoriesRepository _territories;
        public TerritoriesController(TerritoriesRepository territories)
        {
            _territories = territories;
        }

        // GET: odata/Territories
        [HttpGet]
        [EnableQuery]
        [Route("Territories")]
        public IEnumerable<Territory> GetTerritories()
        {
            return _territories.All();
        }

        // GET: odata/Territories(5)
        [HttpGet]
        [EnableQuery]
        [Route("Territories({key})")]
        public IEnumerable<Territory> GetTerritory([FromODataUri] string key)
        {
            return _territories.Where(territory => territory.TerritoryID == key);
        }

        // PUT: odata/Territories(5)
        [HttpPut]
        [Route("Territories({key})")]
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

            _territories.Update(territory);

            return Updated(territory);
        }

        // POST: odata/Territories
        [HttpPost]
        [Route("Territories")]
        public IActionResult Post(Territory territory)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _territories.Insert(territory);

            return Created(territory);
        }

        // PATCH: odata/Territories(5)
        [AcceptVerbs("PATCH", "MERGE")]
        [Route("Territories({key})")]
        public IActionResult Patch([FromODataUri] string key, Delta<Territory> patch)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Territory territory = _territories.Where(x => x.TerritoryID == key).First();
            if (territory == null)
            {
                return NotFound();
            }

            patch.Patch(territory);

            return Updated(territory);
        }

        // DELETE: odata/Territories(5)
        [HttpDelete]
        [Route("Territories({key})")]
        public IActionResult Delete([FromODataUri] string key)
        {
            Territory territory = _territories.Where(x => x.TerritoryID == key).First();
            if (territory == null)
            {
                return NotFound();
            }

            _territories.Delete(territory);

            return StatusCode(204);
        }

        // GET: odata/Territories(5)/Region
        [HttpGet]
        [EnableQuery]
        [Route("Territories({key})/Region")]
        public IQueryable<Region> GetRegion([FromODataUri] string key)
        {
            return _territories.Where(m => m.TerritoryID == key).Select(m => m.Region).AsQueryable();
        }

        // GET: odata/Territories(5)/Employees
        [HttpGet]
        [EnableQuery]
        [Route("Territories({key})/Employees")]
        public IQueryable<Employee> GetEmployees([FromODataUri] string key)
        {
            return _territories.Where(m => m.TerritoryID == key).SelectMany(m => m.EmployeeTerritories.Select(x=> x.Employee)).AsQueryable();
        }
    }
}
