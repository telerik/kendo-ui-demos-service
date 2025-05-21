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
    public class GanttDependenciesController : ODataController
    {
        private readonly DemoDbContext db;

        public GanttDependenciesController(DemoDbContext demoDbContext)
        {
            db = demoDbContext;
        }

        // GET: odata/GanttDependencies
        [HttpGet]
        [EnableQuery]
        [Route("GanttDependencies")]
        public IQueryable<GanttDependency> Get()
        {
            return db.GanttDependencies;
        }

        // GET: odata/GanttDependencies(5)
        [HttpGet]
        [EnableQuery]
        [Route("GanttDependencies({key})")]
        public IQueryable<GanttDependency> Get([FromODataUri] int key)
        {
            return db.GanttDependencies.Where(gp => gp.Id == key);
        }

        // PUT: odata/GanttDependencies(5)
        [HttpPut]
        [Route("GanttDependencies({key})")]
        public IActionResult Put([FromODataUri] int key, GanttDependency ganttDependency)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (key != ganttDependency.Id)
            {
                return BadRequest();
            }

            db.Entry(ganttDependency).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DependencyExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(ganttDependency);
        }

        // POST: odata/GanttDependencies
        [HttpPost]
        [Route("GanttDependencies")]
        public IActionResult Post(GanttDependency ganttDependency)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.GanttDependencies.Add(ganttDependency);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (DependencyExists(ganttDependency.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return Created(ganttDependency);
        }

        // PATCH: odata/GanttDependencies(5)
        [AcceptVerbs("PATCH", "MERGE")]
        [Route("GanttDependencies({key})")]
        public IActionResult Patch([FromODataUri] int key, Delta<GanttDependency> patch)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            GanttDependency ganttDependency = db.GanttDependencies.Find(key);
            if (ganttDependency == null)
            {
                return NotFound();
            }

            patch.Patch(ganttDependency);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DependencyExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(ganttDependency);
        }

        // DELETE: odata/GanttDependencies(5)
        [HttpDelete]
        [Route("GanttDependencies({key})")]
        public IActionResult Delete([FromODataUri] int key)
        {
            GanttDependency ganttDependency = db.GanttDependencies.Find(key);
            if (ganttDependency == null)
            {
                return NotFound();
            }

            db.GanttDependencies.Remove(ganttDependency);
            db.SaveChanges();

            return StatusCode(204);
        }

        private bool DependencyExists(int key)
        {
            return db.GanttDependencies.Count(e => e.Id == key) > 0;
        }
    }
}
