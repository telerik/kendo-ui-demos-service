using KendoCRUDService.Data.Models;
using KendoCRUDService.Data.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace KendoCRUDService.Controllers
{

    public class EmployeeDirectoryController : Controller
    {
        private readonly EmployeeDirectoryRepository _employeeDirectoryRepository;

        public EmployeeDirectoryController(EmployeeDirectoryRepository employeeDirectoryRepository)
        {
            _employeeDirectoryRepository = employeeDirectoryRepository;
        }

        public JsonResult All()
        {
            return Json(_employeeDirectoryRepository.All());
        }

        public JsonResult Index(int? id)
        {
            var employees = _employeeDirectoryRepository.All();

            return Json(
                employees
                    .Where(e => e.ReportsTo == id)
                    .Select(e => new {
                        EmployeeId = e.EmployeeId,
                        FirstName = e.FirstName,
                        LastName = e.LastName,
                        Position = e.Position,
                        Extension = e.Extension,
                        ReportsTo = e.ReportsTo,
                        hasChildren = employees.Where(s => s.ReportsTo == e.EmployeeId).Count() > 0
                    })
            );
        }

        public JsonResult Update([FromBody] IEnumerable<EmployeeDirectoryModel> models)
        { 
            if (models != null)
            {
                foreach (var employee in models)
                {
                    _employeeDirectoryRepository.Update(employee);
                }
            }

            return Json(models);
        }

        public JsonResult Destroy([FromBody] IEnumerable<EmployeeDirectoryModel> models)
        { 
            if (models != null)
            {
                foreach (var employee in models)
                {
                    _employeeDirectoryRepository.Delete(employee);
                }
            }

            return Json(models);
        }

        public JsonResult Create([FromBody] IEnumerable<EmployeeDirectoryModel> models)
        {
            if (models != null)
            {
                foreach (var employee in models)
                {
                    _employeeDirectoryRepository.Insert(employee);
                }
            }

            return Json(models);
        }
    }
}
