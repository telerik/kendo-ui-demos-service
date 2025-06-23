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
    public class OrdersController : ODataController
    {
        private readonly OrdersRepository _ordersRepository;

        public OrdersController(OrdersRepository ordersRepository)
        {
            _ordersRepository = ordersRepository;
        }

        // GET: odata/Orders
        [HttpGet]
        [EnableQuery]
        [Route("Orders")]
        public IEnumerable<Order> GetOrders()
        {
            return _ordersRepository.All();
        }

        // GET: odata/Orders(5)
        [HttpGet]
        [EnableQuery]
        [Route("Orders({key})")]
        public IEnumerable<Order> GetOrder([FromODataUri] int key)
        {
            return _ordersRepository.Where(order => order.OrderID == key);
        }

        // PUT: odata/Orders(5)
        [HttpPut]
        [Route("Orders({key})")]
        public IActionResult Put([FromODataUri] int key, Order order)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (key != order.OrderID)
            {
                return BadRequest();
            }

            _ordersRepository.Update(order);

            return Updated(order);
        }

        // POST: odata/Orders
        [HttpPost]
        [Route("Orders")]
        public IActionResult Post(Order order)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _ordersRepository.Insert(order);

            return Created(order);
        }

        // PATCH: odata/Orders(5)
        [AcceptVerbs("PATCH", "MERGE")]
        [Route("Orders({key})")]
        public IActionResult Patch([FromODataUri] int key, Delta<Order> patch)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Order order = _ordersRepository.Where(x => x.OrderID == key).First();
            if (order == null)
            {
                return NotFound();
            }

            patch.Patch(order);

            return Updated(order);
        }

        // DELETE: odata/Orders(5)
        [HttpDelete]
        [Route("Orders({key})")]
        public IActionResult Delete([FromODataUri] int key)
        {
            Order order = _ordersRepository.Where(x => x.OrderID == key).First();
            if (order == null)
            {
                return NotFound();
            }

            _ordersRepository.Delete(order);
            return StatusCode(204);
        }

        // GET: odata/Orders(5)/Customer
        [HttpGet]
        [EnableQuery]
        [Route("Orders({key})/Customer")]
        public IEnumerable<Customer> GetCustomer([FromODataUri] int key)
        {
            return _ordersRepository.Where(m => m.OrderID == key).Select(m => m.Customer);
        }

        // GET: odata/Orders(5)/Employee
        [HttpGet]
        [EnableQuery]
        [Route("Orders({key})/Employee")]
        public IEnumerable<Employee> GetEmployee([FromODataUri] int key)
        {
            return _ordersRepository.Where(m => m.OrderID == key).Select(m => m.Employee);
        }

        // GET: odata/Orders(5)/Order_Details
        [HttpGet]
        [EnableQuery]
        [Route("Orders({key})/Order_Details")]
        public IQueryable<OrderDetail> GetOrder_Details([FromODataUri] int key)
        {
            return _ordersRepository.Where(m => m.OrderID == key).SelectMany(m => m.OrderDetails).AsQueryable();
        }

        // GET: odata/Orders(5)/Shipper
        [HttpGet]
        [EnableQuery]
        [Route("Orders({key})/Shipper")]
        public IEnumerable<Shipper> GetShipper([FromODataUri] int key)
        {
            return _ordersRepository.Where(m => m.OrderID == key).Select(m => m.ShipViaNavigation);
        }
    }
}
