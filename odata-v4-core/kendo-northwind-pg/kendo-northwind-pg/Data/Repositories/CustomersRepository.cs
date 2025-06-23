using kendo_northwind_pg.Data.Models;
using kendo_northwind_pg.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;
using System.Diagnostics.Metrics;
using System.Net;
using System.Numerics;

namespace kendo_northwind_pg.Data.Repositories
{
    public class CustomersRepository
    {
        private readonly ISession _session;
        private readonly IServiceScopeFactory _scopeFactory;
        private IHttpContextAccessor _contextAccessor;
        private ConcurrentDictionary<string, IList<Customer>> _Customers;


        public CustomersRepository(IHttpContextAccessor httpContextAccessor, IServiceScopeFactory scopeFactory)
        {
            _session = httpContextAccessor.HttpContext.Session;
            _contextAccessor = httpContextAccessor;
            _scopeFactory = scopeFactory;
            _Customers = new ConcurrentDictionary<string, IList<Customer>>();
        }

        public IList<Customer> All()
        {
            var userKey = SessionUtils.GetUserKey(_contextAccessor);

            return _Customers.GetOrAdd(userKey, key =>
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<DemoDbContext>();
                    return context.Customers
                    .Include(p => p.Orders).Select(p => new Customer
                    {
                        CustomerID = p.CustomerID,
                        CompanyName = p.CompanyName,
                        ContactName = p.ContactName,
                        ContactTitle = p.ContactTitle,
                        Address = p.Address,
                        City = p.City,
                        Country = p.Country,
                        Fax = p.Fax,
                        Phone = p.Phone,
                        PostalCode = p.PostalCode,
                        Region = p.Region,
                        Bool = p.Bool,
                        CustomerCustomerDemo = p.CustomerCustomerDemo,
                        Orders = p.Orders
                    }).ToList();
                }

            });
        }
        public static string GenerateRandomCustomerID()
        {
            return Guid.NewGuid().ToString().Substring(0, 5);
        }
        public Customer One(Func<Customer, bool> predicate)
        {
            return All().FirstOrDefault(predicate);
        }

        public IEnumerable<Customer> Where(Func<Customer, bool> predicate)
        {
            return All().Where(predicate);
        }

        public void Insert(Customer Customer)
        {
            var first = All().OrderByDescending(p => p.CustomerID).FirstOrDefault();
            Customer.CustomerID = GenerateRandomCustomerID();

            All().Insert(0, Customer);
        }

        public void Insert(IEnumerable<Customer> Customers)
        {
            foreach (var Customer in Customers)
            {
                Insert(Customer);
            }
        }

        public void Update(Customer Customer)
        {
            var target = One(p => p.CustomerID == Customer.CustomerID);
            if (target != null)
            {
                target.CompanyName = Customer.CompanyName;
                target.ContactName = Customer.ContactName;
                target.Address = Customer.Address;
                target.City = Customer.City;
                target.Fax = Customer.Fax;
                target.Phone = Customer.Phone;
                target.PostalCode = Customer.PostalCode;
                target.Region = Customer.Region;
                target.Bool = Customer.Bool;
                target.ContactTitle = Customer.ContactTitle;
                target.Country = Customer.Country;
            }
        }

        public void Update(IEnumerable<Customer> Customers)
        {
            foreach (var Customer in Customers)
            {
                Update(Customer);
            }
        }

        public void Delete(Customer Customer)
        {
            var target = One(p => p.CustomerID == Customer.CustomerID);
            if (target != null)
            {
                All().Remove(target);
            }
        }

        public void Delete(IEnumerable<Customer> Customers)
        {
            foreach (var Customer in Customers)
            {
                Delete(Customer);
            }
        }
    }
}
