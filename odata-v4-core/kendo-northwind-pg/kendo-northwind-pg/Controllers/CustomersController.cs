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
    public class CustomersController : ODataController
    {
        private readonly CustomersRepository _customersRepository;

        public CustomersController(CustomersRepository customersRepository)
        {
            _customersRepository = customersRepository;
        }

        // GET: odata/Customers
        [HttpGet]
        [EnableQuery]
        [Route("Customers")]
        public IEnumerable<Customer> Get()
        {
            return _customersRepository.All();
        }

        // GET: odata/Customers(5)
        [HttpGet]
        [EnableQuery]
        [Route("Customers({key})")]
        public IEnumerable<Customer> GetCustomer([FromODataUri] string key)
        {
            return _customersRepository.Where(customer => customer.CustomerID == key);
        }

        // PUT: odata/Customers(5)
        [HttpPut]
        [Route("Customers({key})")]
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

            _customersRepository.Update(customer);

            return Updated(customer);
        }

        // POST: odata/Customers
        [HttpPost]
        [Route("Customers")]
        public IActionResult Post(Customer customer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _customersRepository.Insert(customer);

            return Created(customer);
        }

        // PATCH: odata/Customers(5)
        [Route("Customers({key})")]
        [AcceptVerbs("PATCH", "MERGE")]
        public IActionResult Patch([FromODataUri] string key, Delta<Customer> patch)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Customer customer = _customersRepository.Where(x => x.CustomerID == key).First();
            if (customer == null)
            {
                return NotFound();
            }

            patch.Patch(customer);

            return Updated(customer);
        }

        // DELETE: odata/Customers(5)
        [HttpDelete]
        [Route("Customers({key})")]
        public IActionResult Delete([FromODataUri] string key)
        {
            Customer customer = _customersRepository.Where(x => x.CustomerID == key).First();
            if (customer == null)
            {
                return NotFound();
            }

            _customersRepository.Delete(customer);

            return StatusCode(204);
        }

        // GET: odata/Customers(5)/Orders
        [HttpGet]
        [EnableQuery]
        [Route("Customers({key})/Orders")]
        public IEnumerable<Order> GetOrders([FromODataUri] string key)
        {
            return _customersRepository.Where(m => m.CustomerID == key).SelectMany(m => m.Orders);
        }

        // GET: odata/Customers(5)/CustomerDemographics
        [HttpGet]
        [EnableQuery]
        [Route("Customers({key})/CustomerDemographics")]
        public IEnumerable<CustomerDemographic> GetCustomerDemographics([FromODataUri] string key)
        {
            return _customersRepository.Where(m => m.CustomerID == key).SelectMany(m => m.CustomerCustomerDemo.Select(x=> x.CustomerType));
        }
    }
}
