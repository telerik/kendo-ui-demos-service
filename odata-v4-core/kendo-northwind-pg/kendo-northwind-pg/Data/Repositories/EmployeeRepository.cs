using kendo_northwind_pg.Data.Models;
using kendo_northwind_pg.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;
using System.Linq;

namespace kendo_northwind_pg.Data.Repositories
{

    public class EmployeeRepository
    {
        private readonly ISession _session;
        private readonly IServiceScopeFactory _scopeFactory;
        private ConcurrentDictionary<string, IList<Employee>> _employeeDirectories;
        private IHttpContextAccessor _contextAccessor;

        public EmployeeRepository(IHttpContextAccessor httpContextAccessor, IServiceScopeFactory scopeFactory)
        {
            _session = httpContextAccessor.HttpContext.Session;
            _contextAccessor = httpContextAccessor;
            _scopeFactory = scopeFactory;
            _employeeDirectories = new ConcurrentDictionary<string, IList<Employee>>();
        }

        public IList<Employee> All()
        {
            var userKey = SessionUtils.GetUserKey(_contextAccessor);

            return _employeeDirectories.GetOrAdd(userKey, key =>
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<DemoDbContext>();
                    return context.Employees.Include(p => p.Orders).Include(d=>d.EmployeeTerritories).Select(x => new Employee
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
                    }).ToList();
                }
            });
        }

        public Employee One(Func<Employee, bool> predicate)
        {
            return All().FirstOrDefault(predicate);
        }

        public void Insert(Employee employee)
        {
            var first = All().OrderByDescending(e => e.EmployeeID).FirstOrDefault();

            var id = 0;

            if (first != null)
            {
                id = first.EmployeeID;
            }

            employee.EmployeeID = id + 1;

            All().Insert(0, employee);
        }

        public void Update(Employee employee)
        {
            var target = One(e => e.EmployeeID == employee.EmployeeID);

            if (target != null)
            {
                target.FirstName = employee.FirstName;
                target.LastName = employee.LastName;
                target.Address = employee.Address;
                target.City = employee.City;
                target.Country = employee.Country;
                target.Extension = employee.Extension;
                target.BirthDate = employee.BirthDate;
                target.HireDate = employee.HireDate;
                target.ReportsTo = employee.ReportsTo;
                target.HomePhone = employee.HomePhone;
                target.Notes = employee.Notes;
                target.Photo = employee.Photo;
                target.PhotoPath = employee.PhotoPath;
                target.PostalCode = employee.PostalCode;
                target.Region = employee.Region;
                target.Title = employee.Title;
                target.TitleOfCourtesy = employee.TitleOfCourtesy;
            }
        }

        public IEnumerable<Employee> Where(Func<Employee, bool> predicate)
        {
            return All().Where(predicate);
        }


        public void Delete(Employee employee)
        {
            var target = One(p => p.EmployeeID == employee.EmployeeID);
            if (target != null)
            {
                All().Remove(target);

                var employees = All().Where(m => m.ReportsTo == employee.EmployeeID).ToList();

                foreach (var subordinate in employees)
                {
                    Delete(subordinate);
                }
            }
        }
    }
}
