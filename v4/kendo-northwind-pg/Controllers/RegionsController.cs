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
    builder.EntitySet<Region>("Regions");
    builder.EntitySet<Territory>("Territories"); 
    config.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class RegionsController : ODataController
    {
        private NorthwindEntities db = new NorthwindEntities();

        // GET: odata/Regions
        [EnableQuery]
        public IQueryable<Region> GetRegions()
        {
            return db.Regions;
        }

        // GET: odata/Regions(5)
        [EnableQuery]
        public SingleResult<Region> GetRegion([FromODataUri] int key)
        {
            return SingleResult.Create(db.Regions.Where(region => region.RegionID == key));
        }

        // PUT: odata/Regions(5)
        public IHttpActionResult Put([FromODataUri] int key, Region region)
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
        public IHttpActionResult Post(Region region)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Regions.Add(region);

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
        public IHttpActionResult Patch([FromODataUri] int key, Delta<Region> patch)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Region region = db.Regions.Find(key);
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
        public IHttpActionResult Delete([FromODataUri] int key)
        {
            Region region = db.Regions.Find(key);
            if (region == null)
            {
                return NotFound();
            }

            db.Regions.Remove(region);
            db.SaveChanges();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // GET: odata/Regions(5)/Territories
        [EnableQuery]
        public IQueryable<Territory> GetTerritories([FromODataUri] int key)
        {
            return db.Regions.Where(m => m.RegionID == key).SelectMany(m => m.Territories);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool RegionExists(int key)
        {
            return db.Regions.Count(e => e.RegionID == key) > 0;
        }
    }
}
