using KendoCRUDService.Data.Models;
using KendoCRUDService.Extensions;
using KendoCRUDService.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;

namespace KendoCRUDService.Data.Repositories
{

    public class EmployeeRepository
    {
        private readonly ISession _session;
        private readonly IDbContextFactory<DemoDbContext> _contextFactory;
        private readonly IServiceScopeFactory _scopeFactory;
        private IHttpContextAccessor _contextAccessor;
        private ConcurrentDictionary<string, IList<EmployeeViewModel>> _employees;
        private ConcurrentDictionary<string, IList<EmployeeCompleteViewModel>> _employeesComplete;

        public EmployeeRepository(IHttpContextAccessor httpContextAccessor, IServiceScopeFactory scopeFactory, IDbContextFactory<DemoDbContext> contextFactory)
        {
            _session = httpContextAccessor.HttpContext.Session;
            _contextFactory = contextFactory;
            _scopeFactory = scopeFactory;
            _contextAccessor = httpContextAccessor;
            _employees = new ConcurrentDictionary<string, IList<EmployeeViewModel>>();
            _employeesComplete = new ConcurrentDictionary<string, IList<EmployeeCompleteViewModel>>();
        }

        public IList<EmployeeViewModel> All()
        {
            var userKey = SessionUtils.GetUserKey(_contextAccessor);
            IList<EmployeeViewModel> result = _session.GetObjectFromJson<IList<EmployeeViewModel>>("Employees");
            return _employees.GetOrAdd(userKey, key =>
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<DemoDbContext>();
                    return context.Employees.Select(e => new EmployeeViewModel
                        {
                            EmployeeId = e.EmployeeID,
                            FullName = e.FirstName + " " + e.LastName,
                            ReportsTo = e.ReportsTo,
                            HasEmployees = context.Employees.Any(x => x.ReportsTo == e.EmployeeID)
                        }).ToList();
                }
            });
        }

        public IList<EmployeeCompleteViewModel> AllComplete()
        {
            var userKey = SessionUtils.GetUserKey(_contextAccessor);

            return _employeesComplete.GetOrAdd(userKey, key =>
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<DemoDbContext>();
                    return context.Employees.Select(m => new EmployeeCompleteViewModel
                    {
                        FirstName = m.FirstName,
                        LastName = m.LastName,
                        Country = m.Country,
                        City = m.City,
                        Title = m.Title
                    }).ToList();
                }
            });
        }
    }
}
