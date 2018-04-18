using System;
using System.Linq;
using System.Web.Mvc;
using KendoCRUDService.Models;
using KendoCRUDService.Common;
using System.Collections.Generic;
using System.Reflection;

namespace KendoCRUDService.Controllers
{
    public class EmployeesController : Controller
    {
        public ActionResult Index(int? EmployeeId)
        {
            IEnumerable<EmployeeModel> result;

            if (EmployeeId == null)
            {
                result = EmployeeRepository.All().Where(e => e.ReportsTo == null);
            }
            else
            {
                result = EmployeeRepository.All().Where(e => e.ReportsTo == Convert.ToInt32(EmployeeId));
            }

            return this.Jsonp(result);
        }

        public ActionResult Unique(string field)
        {
            var result = EmployeeRepository.AllComplete().Distinct(new EmployeeComparer(field));

            return this.Jsonp(result);
        }
    }

    public class EmployeeComparer : IEqualityComparer<EmployeeCompleteModel>
    {
        private string field;

        private PropertyInfo prop;
        
        public EmployeeComparer(string field)
        {
            this.field = field;
            prop = typeof(EmployeeCompleteModel).GetProperty(field);
        }

        public bool Equals(EmployeeCompleteModel x, EmployeeCompleteModel y)
        {
            return prop.GetValue(x, null).Equals(prop.GetValue(y, null));
        }

        public int GetHashCode(EmployeeCompleteModel obj)
        {
            return prop.GetValue(obj, null).GetHashCode();
        }
    }
}
