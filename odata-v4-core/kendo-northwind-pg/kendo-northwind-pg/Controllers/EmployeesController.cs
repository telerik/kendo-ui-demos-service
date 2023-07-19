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
    public class EmployeesController : ODataController
    {
        private DemoDbContext db;

        public EmployeesController(DemoDbContext demoDbContext)
        {
            db = demoDbContext;
        }

        // GET: odata/Employees
        [HttpGet]
        [EnableQuery]
        [Route("odata/[controller]")]
        public IQueryable<Employee> GetEmployees()
        {
            return db.Employees;
        }

        [HttpGet]
        [Route("odata/[controller]/Default.TopEmployees()")]
        public IQueryable<Employee> TopEmployees()
        {
            return db.Employees.Where(x => x.ReportsTo == null);
        }

        // GET: odata/Employees(5)
        [HttpGet]
        [EnableQuery]
        [Route("odata/[controller]({key})")]
        public SingleResult<Employee> GetEmployee([FromODataUri] int key)
        {
            return SingleResult.Create(db.Employees.Where(employee => employee.EmployeeID == key));
        }

        // PUT: odata/Employees(5)
        [HttpPut]
        [Route("odata/[controller]({key})")]
        public IActionResult Put([FromODataUri] int key, Employee employee)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (key != employee.EmployeeID)
            {
                return BadRequest();
            }

            db.Entry(employee).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmployeeExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(employee);
        }

        // POST: odata/Employees
        [HttpPost]
        [Route("odata/[controller]")]
        public IActionResult Post(Employee employee)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Employees.Add(employee);
            db.SaveChanges();

            return Created(employee);
        }

        // PATCH: odata/Employees(5)
        [Route("odata/[controller]({key})")]
        [AcceptVerbs("PATCH", "MERGE")]
        public IActionResult Patch([FromODataUri] int key, Delta<Employee> patch)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Employee employee = db.Employees.Find(key);
            if (employee == null)
            {
                return NotFound();
            }

            patch.Patch(employee);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmployeeExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(employee);
        }

        // DELETE: odata/Employees(5)
        [HttpDelete]
        [Route("odata/[controller]({key})")]
        public IActionResult Delete([FromODataUri] int key)
        {
            Employee employee = db.Employees.Find(key);
            if (employee == null)
            {
                return NotFound();
            }

            db.Employees.Remove(employee);
            db.SaveChanges();

            return StatusCode(204);
        }

        // GET: odata/Employees(5)/Subordinates
        [HttpGet]
        [EnableQuery]
        [Route("odata/[controller]({key})/Subordinates")]
        public IQueryable<Employee> GetSubordinates([FromODataUri] int key)
        {
            var employees = db.Employees.Where(m => m.EmployeeID == key).SelectMany(m => m.InverseReportsToNavigation);
            foreach (var employee in employees)
            {
                employee.hasChildren = db.Employees.Where(s => s.ReportsTo == employee.EmployeeID).Count() > 0;
            }
            return employees;
        }

        // GET: odata/Employees(5)/Employee1
        [HttpGet]
        [EnableQuery]
        [Route("odata/[controller]({key})/Employee1")]
        public SingleResult<Employee> GetEmployee1([FromODataUri] int key)
        {
            return SingleResult.Create(db.Employees.Where(m => m.EmployeeID == key).Select(m => m.ReportsToNavigation));
        }

        // GET: odata/Employees(5)/Orders
        [HttpGet]
        [EnableQuery]
        [Route("odata/[controller]({key})/Orders")]
        public IQueryable<Order> GetOrders([FromODataUri] int key)
        {
            return db.Employees.Where(m => m.EmployeeID == key).SelectMany(m => m.Orders);
        }

        // GET: odata/Employees(5)/Territories
        [HttpGet]
        [EnableQuery]
        [Route("odata/[controller]({key})/Territories")]
        public IQueryable<Territory> GetTerritories([FromODataUri] int key)
        {
            return db.Employees.Where(m => m.EmployeeID == key).SelectMany(m => m.EmployeeTerritories.Select(x=> x.Territory));
        }

        private bool EmployeeExists(int key)
        {
            return db.Employees.Count(e => e.EmployeeID == key) > 0;
        }
    }
}
