using KendoCRUDService.Data.Models;
using KendoCRUDService.Models;
using KendoCRUDService.SessionExtensions;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;

namespace KendoCRUDService.Data.Repositories
{
    public class EmployeeDirectoryRepository
    {
        private readonly ISession _session;
        private readonly IServiceScopeFactory _scopeFactory;
        private ConcurrentDictionary<string, IList<EmployeeDirectoryModel>> _employeeDirectories;
        private IHttpContextAccessor _contextAccessor;

        public EmployeeDirectoryRepository(IHttpContextAccessor httpContextAccessor, IServiceScopeFactory scopeFactory)
        {
            _session = httpContextAccessor.HttpContext.Session;
            _contextAccessor = httpContextAccessor;
            _scopeFactory = scopeFactory;
            _employeeDirectories = new ConcurrentDictionary<string, IList<EmployeeDirectoryModel>>();
        }

        public IList<EmployeeDirectoryModel> All()
        {
            var userKey = SessionUtils.GetUserKey(_contextAccessor);

            return _employeeDirectories.GetOrAdd(userKey, key =>
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<DemoDbContext>();
                    return context.EmployeeDirectories.ToList().Select(employee => new EmployeeDirectoryModel
                    {
                        EmployeeId = employee.EmployeeID,
                        ReportsTo = employee.ReportsTo,
                        FirstName = employee.FirstName,
                        LastName = employee.LastName,
                        HireDate = employee.HireDate,
                        BirthDate = employee.BirthDate,
                        Phone = employee.Phone,
                        Extension = employee.Extension,
                        Address = employee.Address,
                        City = employee.City,
                        Country = employee.Country,
                        Position = employee.Position
                    }).ToList();
                }
            });
        }

        public EmployeeDirectoryModel One(Func<EmployeeDirectoryModel, bool> predicate)
        {
            return All().FirstOrDefault(predicate);
        }

        public void Insert(EmployeeDirectoryModel employee)
        {
            var first = All().OrderByDescending(e => e.EmployeeId).FirstOrDefault();

            var id = 0;

            if (first != null)
            {
                id = first.EmployeeId;
            }

            employee.EmployeeId = id + 1;

            All().Insert(0, employee);
        }

        public void Update(EmployeeDirectoryModel employee)
        {
            var target = One(e => e.EmployeeId == employee.EmployeeId);

            if (target != null)
            {
                target.FirstName = employee.FirstName;
                target.LastName = employee.LastName;
                target.Address = employee.Address;
                target.City = employee.City;
                target.Country = employee.Country;
                target.Phone = employee.Phone;
                target.Extension = employee.Extension;
                target.BirthDate = employee.BirthDate;
                target.HireDate = employee.HireDate;
                target.Position = employee.Position;
                target.ReportsTo = employee.ReportsTo;
            }
        }

        public void Delete(EmployeeDirectoryModel employee)
        {
            var target = One(p => p.EmployeeId == employee.EmployeeId);
            if (target != null)
            {
                All().Remove(target);

                var employees = All().Where(m => m.ReportsTo == employee.EmployeeId).ToList();

                foreach (var subordinate in employees)
                {
                    Delete(subordinate);
                }
            }
        }
    }
}
