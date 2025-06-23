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
    public class GanttTasksController : ODataController
    {
        private readonly GanttTaskRepository _ganttTasks;

        public GanttTasksController(GanttTaskRepository ganttTasks)
        {
            _ganttTasks = ganttTasks;
        }

        // GET: odata/GanttTasks
        [HttpGet]
        [EnableQuery]
        [Route("GanttTasks")]
        public IEnumerable<GanttTask> Get()
        {
            return _ganttTasks.All();
        }

        // GET: odata/GanttTasks(5)
        [HttpGet]
        [EnableQuery]
        [Route("GanttTasks({key})")]
        public IEnumerable<GanttTask> Get([FromODataUri] int key)
        {
            return _ganttTasks.Where(gp => gp.Id == key);
        }

        // PUT: odata/GanttDependencies(5)
        [HttpPut]
        [Route("GanttTasks({key})")]
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

            _ganttTasks.Update(ganttTask);

            return Updated(ganttTask);
        }

        // POST: odata/GanttDependencies
        [HttpPost]
        [Route("GanttTasks")]
        public IActionResult Post(GanttTask ganttTask)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _ganttTasks.Insert(ganttTask);

            return Created(ganttTask);
        }

        // PATCH: odata/GanttDependencies(5)
        [AcceptVerbs("PATCH", "MERGE")]
        [Route("GanttTasks({key})")]
        public IActionResult Patch([FromODataUri] int key, Delta<GanttTask> patch)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            GanttTask ganttTask = _ganttTasks.Where(x => x.Id == key).First(); 
            if (ganttTask == null)
            {
                return NotFound();
            }

            patch.Patch(ganttTask);

            return Updated(ganttTask);
        }

        // DELETE: odata/GanttDependencies(5)
        [HttpDelete]
        [Route("GanttTasks({key})")]
        public IActionResult Delete([FromODataUri] int key)
        {
            GanttTask ganttTask = _ganttTasks.Where(x => x.Id == key).First();
            if (ganttTask == null)
            {
                return NotFound();
            }

            _ganttTasks.Delete(ganttTask);

            return StatusCode(204);
        }
    }
}
