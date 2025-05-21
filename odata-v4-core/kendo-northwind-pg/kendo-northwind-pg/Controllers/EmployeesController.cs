using kendo_northwind_pg.Data;
using kendo_northwind_pg.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
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
        [Route("Employees")]
        public IQueryable<Employee> Get()
        {
            return db.Employees;
        }

        [HttpGet]
        [EnableQuery]
        [Route("TopEmployees")]
        public IQueryable<Employee> TopEmployees()
        {
            return db.Employees.Where(x => x.ReportsTo == null).Select(x=> new Employee
            {
                Address = x.Address,
                BirthDate = x.BirthDate,
                City = x.City,
                Country = x.Country,
                EmployeeID = x.EmployeeID,
                EmployeeTerritories = x.EmployeeTerritories,
                Extension = x.Extension,
                FirstName = x.FirstName,
                hasChildren = x.InverseReportsToNavigation.Any(),
                HireDate = x.HireDate,
                HomePhone = x.HomePhone,
                ReportsTo = x.ReportsTo,
                InverseReportsToNavigation = x.InverseReportsToNavigation,
                LastName = x.LastName,
                Notes = x.Notes,
                Orders = x.Orders,
                Photo = x.Photo, 
                PhotoPath = x.PhotoPath,
                PostalCode = x.PostalCode,
                Region = x.Region,
                ReportsToNavigation = x.ReportsToNavigation,
                Title = x.Title,
                TitleOfCourtesy = x.TitleOfCourtesy                
            });
        }

        // GET: odata/Employees(5)
        [HttpGet]
        [EnableQuery]
        [Route("Employees({key})")]
        public IQueryable<Employee> Get([FromODataUri] int key)
        {
            return db.Employees.Where(employee => employee.EmployeeID == key);
        }

        // PUT: odata/Employees(5)
        [HttpPut]
        [Route("Employees({key})")]
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
        [Route("Employees")]
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
        [Route("Employees({key})")]
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
        [Route("[controller]({key})")]
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

        // GET: odata/Subordinates(5)
        [HttpGet]
        [EnableQuery]
        [Route("EmployeeSubordinates({key})")]
        public IEnumerable<Employee> GetSubordinates([FromODataUri] int key)
        {
            return db.Employees.Where(x => x.ReportsTo == key).Select(x => new Employee
            {
                Address = x.Address,
                BirthDate = x.BirthDate,
                City = x.City,
                Country = x.Country,
                EmployeeID = x.EmployeeID,
                EmployeeTerritories = x.EmployeeTerritories,
                Extension = x.Extension,
                FirstName = x.FirstName,
                hasChildren = x.InverseReportsToNavigation.Any(),
                HireDate = x.HireDate,
                HomePhone = x.HomePhone,
                ReportsTo = x.ReportsTo,
                InverseReportsToNavigation = x.InverseReportsToNavigation,
                LastName = x.LastName,
                Notes = x.Notes,
                Orders = x.Orders,
                Photo = x.Photo,
                PhotoPath = x.PhotoPath,
                PostalCode = x.PostalCode,
                Region = x.Region,
                ReportsToNavigation = x.ReportsToNavigation,
                Title = x.Title,
                TitleOfCourtesy = x.TitleOfCourtesy
            });
        }

        // GET: odata/EmployeeManager(5)
        [HttpGet]
        [EnableQuery]
        [Route("EmployeeManager({key})")]
        public IQueryable<Employee> GetManager([FromODataUri] int key)
        {
            return db.Employees.Where(m => m.EmployeeID == key).Select(m => m.ReportsToNavigation);
        }

        // GET: odata/EmployeeOrders(5)
        [HttpGet]
        [EnableQuery]
        [Route("EmployeeOrders({key})")]
        public IQueryable<Order> GetOrders([FromODataUri] int key)
        {
            return db.Employees.Where(m => m.EmployeeID == key).SelectMany(m => m.Orders);
        }

        // GET: odata/EmployeeTerritories(5)
        [HttpGet]
        [EnableQuery]
        [Route("EmployeeTerritories({key})")]
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
