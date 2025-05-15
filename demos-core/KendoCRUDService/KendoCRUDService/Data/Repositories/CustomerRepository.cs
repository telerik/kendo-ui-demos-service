using KendoCRUDService.Models;
using KendoCRUDService.Extensions;
using Microsoft.EntityFrameworkCore;

namespace KendoCRUDService.Data.Repositories
{
    public class CustomerRepository
    {
        private readonly ISession _session;
        private readonly IDbContextFactory<DemoDbContext> _contextFactory;

        public CustomerRepository(IHttpContextAccessor httpContextAccessor, IDbContextFactory<DemoDbContext> contextFactory)
        {
            _session = httpContextAccessor.HttpContext.Session;
            _contextFactory = contextFactory;
        }

        public IList<CustomerViewModel> All()
        {
            IList<CustomerViewModel> result = _session.GetObjectFromJson<IList<CustomerViewModel>>("Customers");

            if (result == null)
            {
                result = _contextFactory.CreateDbContext().Customers.Select(c => new CustomerViewModel
                {
                    CustomerID = c.CustomerID,
                    ContactName = c.ContactName,
                    CompanyName = c.CompanyName,
                    Address = c.Address,
                    City = c.City,
                    Country = c.Country,
                    ContactTitle = c.ContactTitle,
                    PostalCode = c.PostalCode,
                    Bool = c.Bool,
                    Fax = c.Fax,
                    Phone = c.Phone,
                    Region = c.Region
                }).ToList();
            }

            return result;
        }
    }
}
