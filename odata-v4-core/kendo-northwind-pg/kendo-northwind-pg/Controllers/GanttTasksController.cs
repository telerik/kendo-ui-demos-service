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
    public class GanttTasksController : ODataController
    {
        private readonly DemoDbContext db;

        public GanttTasksController(DemoDbContext demoDbContext)
        {
            db = demoDbContext;
        }

        // GET: odata/GanttTasks
        [HttpGet]
        [EnableQuery]
        [Route("odata/GanttTasks")]
        public IQueryable<GanttTask> Get()
        {
            return db.GanttTasks;
        }

        // GET: odata/GanttTasks(5)
        [HttpGet]
        [EnableQuery]
        [Route("odata/GanttTasks({key})")]
        public SingleResult<GanttTask> Get([FromODataUri] int key)
        {
            return SingleResult.Create(db.GanttTasks.Where(gp => gp.Id == key));
        }

        // PUT: odata/GanttDependencies(5)
        [HttpPut]
        [Route("[controller]({key})")]
        public IActionResult Put([FromODataUri] int key, GanttTask ganttTask)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (key != ganttTask.Id)
            {
                return BadRequest();
            }

            db.Entry(ganttTask).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TaskExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(ganttTask);
        }

        // POST: odata/GanttDependencies
        [HttpPost]
        [Route("[controller]")]
        public IActionResult Post(GanttTask ganttTask)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.GanttTasks.Add(ganttTask);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (TaskExists(ganttTask.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return Created(ganttTask);
        }

        // PATCH: odata/GanttDependencies(5)
        [AcceptVerbs("PATCH", "MERGE")]
        [Route("[controller]({key})")]
        public IActionResult Patch([FromODataUri] int key, Delta<GanttTask> patch)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            GanttTask ganttTask = db.GanttTasks.Find(key);
            if (ganttTask == null)
            {
                return NotFound();
            }

            patch.Patch(ganttTask);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TaskExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(ganttTask);
        }

        // DELETE: odata/GanttDependencies(5)
        [HttpDelete]
        [Route("[controller]({key})")]
        public IActionResult Delete([FromODataUri] int key)
        {
            GanttTask ganttTask = db.GanttTasks.Find(key);
            if (ganttTask == null)
            {
                return NotFound();
            }

            db.GanttTasks.Remove(ganttTask);
            db.SaveChanges();

            return StatusCode(204);
        }

        private bool TaskExists(int key)
        {
            return db.GanttDependencies.Count(e => e.Id == key) > 0;
        }
    }
}
