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

        public ActionResult ValueMapper(int?[] values)
        {
            var indices = new List<List<int?>>();
            var employees = EmployeeRepository.All();

            if (values != null && values.Any())
            {
                foreach (var value in values)
                {
                    var idSequence = new List<int?>();

                    var item = employees.FirstOrDefault(e => e.EmployeeId == value);
                    if (item != null)
                    {
                        idSequence.Insert(0, item.EmployeeId);

                        if (item.ReportsTo != null)
                        {
                            while (true)
                            {
                                var parentItem = employees.FirstOrDefault(e => e.EmployeeId == item.ReportsTo);
                                
                                
                                if (parentItem == null)
                                { 
                                    break;
                                } else if (parentItem.ReportsTo == null)
                                {
                                    idSequence.Insert(0, parentItem.EmployeeId);
                                    break;
                                }

                                idSequence.Insert(0, parentItem.EmployeeId);
                                item = parentItem;
                            }

                        }
                    }

                    indices.Add(idSequence);
                }
            } 

            return this.Jsonp(indices);
        }

        public ActionResult Search(int? EmployeeId, string text)
        {
            IEnumerable<EmployeeModel> result;

            result = EmployeeRepository.All()
                    .Where(e => EmployeeId == null ? e.ReportsTo == null : e.ReportsTo == Convert.ToInt32(EmployeeId))
                    .Where(e => string.IsNullOrEmpty(text) || e.FullName.Contains(text) || 
                        (EmployeeRepository.All()
                            .Where(q => q.ReportsTo == e.EmployeeId && (q.FullName.Contains(text) ||
                                (EmployeeRepository.All()
                                .Where(p => p.ReportsTo == q.EmployeeId && (p.FullName.Contains(text)))
                            ).Count() > 0))
                        ).Count() > 0);

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
