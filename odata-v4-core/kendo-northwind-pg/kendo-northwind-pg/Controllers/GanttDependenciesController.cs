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
    public class GanttDependenciesController : ODataController
    {
        private readonly GanttDependencyRepository _ganttDependencies;

        public GanttDependenciesController(GanttDependencyRepository ganttDependencies)
        {
            _ganttDependencies = ganttDependencies;
        }

        // GET: odata/GanttDependencies
        [HttpGet]
        [EnableQuery]
        [Route("GanttDependencies")]
        public IEnumerable<GanttDependency> Get()
        {
            return _ganttDependencies.All();
        }

        // GET: odata/GanttDependencies(5)
        [HttpGet]
        [EnableQuery]
        [Route("GanttDependencies({key})")]
        public IEnumerable<GanttDependency> Get([FromODataUri] int key)
        {
            return _ganttDependencies.Where(gp => gp.Id == key);
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

            _ganttDependencies.Update(ganttDependency);

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

            _ganttDependencies.Insert(ganttDependency);

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

            GanttDependency ganttDependency = _ganttDependencies.Where(gp => gp.Id == key).First();
            if (ganttDependency == null)
            {
                return NotFound();
            }

            patch.Patch(ganttDependency);

            return Updated(ganttDependency);
        }

        // DELETE: odata/GanttDependencies(5)
        [HttpDelete]
        [Route("GanttDependencies({key})")]
        public IActionResult Delete([FromODataUri] int key)
        {
            GanttDependency ganttDependency = _ganttDependencies.Where(gp => gp.Id == key).First();
            if (ganttDependency == null)
            {
                return NotFound();
            }

            _ganttDependencies.Delete(ganttDependency);

            return StatusCode(204);
        }
    }
}
