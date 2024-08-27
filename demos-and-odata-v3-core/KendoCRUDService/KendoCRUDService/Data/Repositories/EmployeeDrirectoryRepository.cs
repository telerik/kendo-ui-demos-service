using KendoCRUDService.Data.Models;
using KendoCRUDService.SessionExtensions;
using Microsoft.EntityFrameworkCore;

namespace KendoCRUDService.Data.Repositories
{
    public class EmployeeDirectoryRepository
    {
        private bool UpdateDatabase = false;
        private readonly ISession _session;
        private readonly IDbContextFactory<DemoDbContext> _contextFactory;

        public EmployeeDirectoryRepository(IHttpContextAccessor httpContextAccessor, IDbContextFactory<DemoDbContext> contextFactory)
        {
            _session = httpContextAccessor.HttpContext.Session;
            _contextFactory = contextFactory;
        }

        public IList<EmployeeDirectoryModel> All()
        {
            var result = _session.GetObjectFromJson<IList<EmployeeDirectoryModel>>("EmployeeDirectory");

            if (result == null || UpdateDatabase)
            {
                using (var db = _contextFactory.CreateDbContext())
                {
                    result = db.EmployeeDirectories.ToList().Select(employee => new EmployeeDirectoryModel
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

                _session.SetObjectAsJson("EmployeeDirectory", result);
            }

            return result;
        }

        public EmployeeDirectoryModel One(Func<EmployeeDirectoryModel, bool> predicate)
        {
            return All().FirstOrDefault(predicate);
        }

        public void Insert(EmployeeDirectoryModel employee)
        {
            if (!UpdateDatabase)
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
            else
            {
                using (var db = _contextFactory.CreateDbContext())
                {
                    var entity = employee.ToEntity();

                    db.EmployeeDirectories.Add(entity);
                    db.SaveChanges();

                    employee.EmployeeId = entity.EmployeeID;
                }
            }
        }

        public void Update(EmployeeDirectoryModel employee)
        {
            if (!UpdateDatabase)
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
            else
            {
                using (var db = _contextFactory.CreateDbContext())
                {
                    var entity = employee.ToEntity();
                    db.EmployeeDirectories.Attach(entity);
                    db.SaveChanges();
                }
            }
        }

        public void Delete(EmployeeDirectoryModel employee)
        {
            if (!UpdateDatabase)
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
            else
            {
                using (var db = _contextFactory.CreateDbContext())
                {
                    var entity = employee.ToEntity();
                    db.EmployeeDirectories.Attach(entity);

                    var employees = db.EmployeeDirectories.Where(t => t.ReportsTo == employee.EmployeeId);

                    foreach (var subordinate in employees)
                    {
                        Delete(new EmployeeDirectoryModel { EmployeeId = subordinate.EmployeeID });
                    }

                    db.EmployeeDirectories.Remove(entity);
                    db.SaveChanges();
                }
            }
        }
    }
}
