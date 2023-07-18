using KendoCRUDService.Data.Models;
using KendoCRUDService.Models;
using KendoCRUDService.SessionExtensions;
using Microsoft.EntityFrameworkCore;

namespace KendoCRUDService.Data.Repositories
{

    public class EmployeeRepository
    {
        private readonly ISession _session;
        private readonly IDbContextFactory<DemoDbContext> _contextFactory;

        public EmployeeRepository(IHttpContextAccessor httpContextAccessor, IDbContextFactory<DemoDbContext> contextFactory)
        {
            _session = httpContextAccessor.HttpContext.Session;
            _contextFactory = contextFactory;
        }

        public IList<EmployeeViewModel> All()
        {
            IList<EmployeeViewModel> result = _session.GetObjectFromJson<IList<EmployeeViewModel>>("Employees");

            if (result == null)
            {
                using (var db = _contextFactory.CreateDbContext())
                {

                    result = db.Employees.Select(e => new EmployeeViewModel
                    {
                        EmployeeId = e.EmployeeID,
                        FullName = e.FirstName + " " + e.LastName,
                        ReportsTo = e.ReportsTo,
                        HasEmployees = db.Employees.Any(x=> x.ReportsTo == e.EmployeeID)
                    }).ToList();

                    _session.SetObjectAsJson("Employees", result);
                }
            }

            return result;
        }

        public IList<EmployeeCompleteViewModel> AllComplete()
        {
            IList<EmployeeCompleteViewModel> result = _session.GetObjectFromJson<IList<EmployeeCompleteViewModel>>("EmployeesComplete");

            if (result == null)
            {
                result = _contextFactory.CreateDbContext().Employees.Select(m => new EmployeeCompleteViewModel
                {
                    FirstName = m.FirstName,
                    LastName = m.LastName,
                    Country = m.Country,
                    City = m.City,
                    Title = m.Title
                }).ToList();

                _session.SetObjectAsJson("EmployeesComplete", result);
            }

            return result;
        }
    }
}
