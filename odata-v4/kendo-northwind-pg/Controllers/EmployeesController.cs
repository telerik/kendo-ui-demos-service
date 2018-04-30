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
using Microsoft.AspNet.OData.Routing;

namespace kendo_northwind_pg.Controllers
{
    /*
    The WebApiConfig class may require additional changes to add a route for this controller. Merge these statements into the Register method of the WebApiConfig class as applicable. Note that OData URLs are case sensitive.

    using System.Web.OData.Builder;
    using System.Web.OData.Extensions;
    using kendo_northwind_pg.Models;
    ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
    builder.EntitySet<Employee>("Employees");
    builder.EntitySet<Order>("Orders"); 
    builder.EntitySet<Territory>("Territories"); 
    config.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class EmployeesController : ODataController
    {
        private NorthwindEntities db = new NorthwindEntities();

        // GET: odata/Employees
        [EnableQuery]
        public IQueryable<Employee> GetEmployees()
        {
            return db.Employees;
        }

        [HttpGet]
        [ODataRoute("Employees/Default.TopEmployees()")]
        public IQueryable<Employee> TopEmployees()
        {
            return db.Employees.Where(x => x.ReportsTo == null);
        }

        // GET: odata/Employees(5)
        [EnableQuery]
        public SingleResult<Employee> GetEmployee([FromODataUri] int key)
        {
            return SingleResult.Create(db.Employees.Where(employee => employee.EmployeeID == key));
        }

        // PUT: odata/Employees(5)
        public IHttpActionResult Put([FromODataUri] int key, Employee employee)
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
        public IHttpActionResult Post(Employee employee)
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
        [AcceptVerbs("PATCH", "MERGE")]
        public IHttpActionResult Patch([FromODataUri] int key, Delta<Employee> patch)
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
        public IHttpActionResult Delete([FromODataUri] int key)
        {
            Employee employee = db.Employees.Find(key);
            if (employee == null)
            {
                return NotFound();
            }

            db.Employees.Remove(employee);
            db.SaveChanges();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // GET: odata/Employees(5)/Employees1
        [EnableQuery]
        public IQueryable<Employee> GetEmployees1([FromODataUri] int key)
        {
            var employees = db.Employees.Where(m => m.EmployeeID == key).SelectMany(m => m.Employees1);
            foreach (var employee in employees)
            {
                employee.hasChildren = db.Employees.Where(s => s.ReportsTo == employee.EmployeeID).Count() > 0;
            }
            return employees;
        }

        // GET: odata/Employees(5)/Employee1
        [EnableQuery]
        public SingleResult<Employee> GetEmployee1([FromODataUri] int key)
        {
            return SingleResult.Create(db.Employees.Where(m => m.EmployeeID == key).Select(m => m.Employee1));
        }

        // GET: odata/Employees(5)/Orders
        [EnableQuery]
        public IQueryable<Order> GetOrders([FromODataUri] int key)
        {
            return db.Employees.Where(m => m.EmployeeID == key).SelectMany(m => m.Orders);
        }

        // GET: odata/Employees(5)/Territories
        [EnableQuery]
        public IQueryable<Territory> GetTerritories([FromODataUri] int key)
        {
            return db.Employees.Where(m => m.EmployeeID == key).SelectMany(m => m.Territories);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool EmployeeExists(int key)
        {
            return db.Employees.Count(e => e.EmployeeID == key) > 0;
        }
    }
}
