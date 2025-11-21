using KendoCRUDService.Data.Models;
using KendoCRUDService.Extensions;
using KendoCRUDService.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace KendoCRUDService.Data.Repositories
{
    public class EmployeeDirectoryRepository
    {
        private readonly ISession _session;
        private readonly IServiceScopeFactory _scopeFactory;
        private IHttpContextAccessor _contextAccessor;

        private readonly IUserDataCache _userCache;
        private readonly ILogger<EmployeeDirectoryRepository> _logger;

        private static readonly TimeSpan Ttl = TimeSpan.FromMinutes(15);
        private const string LogicalName = "EmployeeDirectoryModels";

        public EmployeeDirectoryRepository(IHttpContextAccessor httpContextAccessor, IServiceScopeFactory scopeFactory, IUserDataCache userCache,
            ILogger<EmployeeDirectoryRepository> logger)
        {
            _session = httpContextAccessor.HttpContext.Session;
            _contextAccessor = httpContextAccessor;
            _scopeFactory = scopeFactory;
            _userCache = userCache;
            _logger = logger;
        }

        public IList<EmployeeDirectoryModel> All()
        {
            var userKey = SessionUtils.GetUserKey(_contextAccessor);

            return _userCache.GetOrCreateList<EmployeeDirectoryModel>(userKey, LogicalName, () =>
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
            }, Ttl, sliding: true);
        }

        public EmployeeDirectoryModel One(Func<EmployeeDirectoryModel, bool> predicate)
        {
            return All().FirstOrDefault(predicate);
        }

        public void Insert(EmployeeDirectoryModel employee)
        {
            var entries = All();
            var first = entries.OrderByDescending(e => e.EmployeeId).FirstOrDefault();

            var id = 0;

            if (first != null)
            {
                id = first.EmployeeId;
            }

            employee.EmployeeId = id + 1;

            entries.Insert(0, employee);

            UpdateContent(entries.ToList());
        }

        private void UpdateContent(List<EmployeeDirectoryModel> entries)
        {
            var userKey = SessionUtils.GetUserKey(_contextAccessor);
            _userCache.GetOrCreateList<EmployeeDirectoryModel>(
                userKey,
                LogicalName,
                () =>
                {
                    return entries;
                },
                Ttl,
                sliding: true
            );
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
                var entries = All().ToList();
                entries.Remove(target);

                var employees = entries.Where(m => m.ReportsTo == employee.EmployeeId).ToList();

                foreach (var subordinate in employees)
                {
                    Delete(subordinate);
                }
                UpdateContent(entries);
            }
        }
    }
}
