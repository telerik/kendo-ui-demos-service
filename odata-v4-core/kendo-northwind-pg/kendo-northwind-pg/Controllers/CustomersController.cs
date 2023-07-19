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
    public class CustomersController : ODataController
    {
        private readonly DemoDbContext db;

        public CustomersController(DemoDbContext dbContext)
        {
            db = dbContext;
        }

        // GET: odata/Customers
        [HttpGet]
        [EnableQuery]
        [Route("odata/[controller]")]
        public IQueryable<Customer> GetCustomers()
        {
            return db.Customers;
        }

        // GET: odata/Customers(5)
        [HttpGet]
        [EnableQuery]
        [Route("odata/[controller]({key})")]
        public SingleResult<Customer> GetCustomer([FromODataUri] string key)
        {
            return SingleResult.Create(db.Customers.Where(customer => customer.CustomerID == key));
        }

        // PUT: odata/Customers(5)
        [HttpPut]
        [Route("odata/[controller]({key})")]
        public IActionResult Put([FromODataUri] string key, Customer customer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (key != customer.CustomerID)
            {
                return BadRequest();
            }

            db.Entry(customer).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(customer);
        }

        // POST: odata/Customers
        [HttpPost]
        [Route("odata/[controller]")]
        public IActionResult Post(Customer customer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Customers.Add(customer);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (CustomerExists(customer.CustomerID))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return Created(customer);
        }

        // PATCH: odata/Customers(5)
        [Route("odata/[controller]({key})")]
        [AcceptVerbs("PATCH", "MERGE")]
        public IActionResult Patch([FromODataUri] string key, Delta<Customer> patch)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Customer customer = db.Customers.Find(key);
            if (customer == null)
            {
                return NotFound();
            }

            patch.Patch(customer);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(customer);
        }

        // DELETE: odata/Customers(5)
        [HttpDelete]
        [Route("odata/[controller]({key})")]
        public IActionResult Delete([FromODataUri] string key)
        {
            Customer customer = db.Customers.Find(key);
            if (customer == null)
            {
                return NotFound();
            }

            db.Customers.Remove(customer);
            db.SaveChanges();

            return StatusCode(204);
        }

        // GET: odata/Customers(5)/Orders
        [HttpGet]
        [EnableQuery]
        [Route("odata/[controller]({key})/Orders")]
        public IQueryable<Order> GetOrders([FromODataUri] string key)
        {
            return db.Customers.Where(m => m.CustomerID == key).SelectMany(m => m.Orders);
        }

        // GET: odata/Customers(5)/CustomerDemographics
        [HttpGet]
        [EnableQuery]
        [Route("odata/[controller]({key})/CustomerDemographics")]
        public IQueryable<CustomerDemographic> GetCustomerDemographics([FromODataUri] string key)
        {
            return db.Customers.Where(m => m.CustomerID == key).SelectMany(m => m.CustomerCustomerDemo.Select(x=> x.CustomerType));
        }

        private bool CustomerExists(string key)
        {
            return db.Customers.Count(e => e.CustomerID == key) > 0;
        }
    }
}
