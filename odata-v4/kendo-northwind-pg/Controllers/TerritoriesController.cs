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
    builder.EntitySet<Territory>("Territories");
    builder.EntitySet<Region>("Regions"); 
    builder.EntitySet<Employee>("Employees"); 
    config.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    public class TerritoriesController : ODataController
    {
        private NorthwindEntities db = new NorthwindEntities();

        // GET: odata/Territories
        [EnableQuery]
        public IQueryable<Territory> GetTerritories()
        {
            return db.Territories;
        }

        // GET: odata/Territories(5)
        [EnableQuery]
        public SingleResult<Territory> GetTerritory([FromODataUri] string key)
        {
            return SingleResult.Create(db.Territories.Where(territory => territory.TerritoryID == key));
        }

        // PUT: odata/Territories(5)
        public IHttpActionResult Put([FromODataUri] string key, Territory territory)
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
        public IHttpActionResult Post(Territory territory)
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
        public IHttpActionResult Patch([FromODataUri] string key, Delta<Territory> patch)
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
        public IHttpActionResult Delete([FromODataUri] string key)
        {
            Territory territory = db.Territories.Find(key);
            if (territory == null)
            {
                return NotFound();
            }

            db.Territories.Remove(territory);
            db.SaveChanges();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // GET: odata/Territories(5)/Region
        [EnableQuery]
        public SingleResult<Region> GetRegion([FromODataUri] string key)
        {
            return SingleResult.Create(db.Territories.Where(m => m.TerritoryID == key).Select(m => m.Region));
        }

        // GET: odata/Territories(5)/Employees
        [EnableQuery]
        public IQueryable<Employee> GetEmployees([FromODataUri] string key)
        {
            return db.Territories.Where(m => m.TerritoryID == key).SelectMany(m => m.Employees);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool TerritoryExists(string key)
        {
            return db.Territories.Count(e => e.TerritoryID == key) > 0;
        }
    }
}
