using KendoCRUDService.Data.Models;
using KendoCRUDService.Data.Repositories;
using KendoCRUDService.Models;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace KendoCRUDService.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly EmployeeRepository _employeeRepository;

        public EmployeesController( EmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        public ActionResult Index(int? EmployeeId)
        {
            IEnumerable<EmployeeViewModel> result;

            if (EmployeeId == null)
            {
                result = _employeeRepository.All().Where(e => e.ReportsTo == null);
            }
            else
            {
                result = _employeeRepository.All().Where(e => e.ReportsTo == Convert.ToInt32(EmployeeId));
            }

            return Json(result);
        }

        public ActionResult Search(int? EmployeeId, string text)
        {
            IEnumerable<EmployeeViewModel> result;

            result = _employeeRepository.All()
                    .Where(e => EmployeeId == null ? e.ReportsTo == null : e.ReportsTo == Convert.ToInt32(EmployeeId))
                    .Where(e => string.IsNullOrEmpty(text) || e.FullName.Contains(text) ||
                        (_employeeRepository.All()
                            .Where(q => q.ReportsTo == e.EmployeeId && (q.FullName.Contains(text) ||
                                (_employeeRepository.All()
                                .Where(p => p.ReportsTo == q.EmployeeId && (p.FullName.Contains(text)))
                            ).Count() > 0))
                        ).Count() > 0);

            return Json(result);
        }

        public ActionResult Unique(string field)
        {
            var result = _employeeRepository.AllComplete().Distinct(new EmployeeComparer(field));

            return Json(result);
        }
    }

    public class EmployeeComparer : IEqualityComparer<EmployeeCompleteViewModel>
    {
        private string field;

        private PropertyInfo prop;

        public EmployeeComparer(string field)
        {
            this.field = field;
            prop = typeof(EmployeeCompleteViewModel).GetProperty(field);
        }

        public bool Equals(EmployeeCompleteViewModel x, EmployeeCompleteViewModel y)
        {
            return prop.GetValue(x, null).Equals(prop.GetValue(y, null));
        }

        public int GetHashCode(EmployeeCompleteViewModel obj)
        {
            return prop.GetValue(obj, null).GetHashCode();
        }
    }
}
