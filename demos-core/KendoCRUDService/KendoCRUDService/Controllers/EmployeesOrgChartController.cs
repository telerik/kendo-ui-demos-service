using KendoCRUDService.Data.Repositories;
using KendoCRUDService.Models;
using KendoCRUDService.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace KendoCRUDService.Controllers
{
    public class EmployeesOrgChartController : Controller
    {
        private readonly EmployeeOrgChartRepository _employeesRepo;
        public EmployeesOrgChartController(EmployeeOrgChartRepository employeesRepo)
        {
            _employeesRepo = employeesRepo;
        }

        public ActionResult Index()
        {
            var result = from node in _employeesRepo.AllNodes()
                         select new
                         {
                             Id = node.Id,
                             Position = node.Position,
                             FullName = node.FullName,
                             Avatar = node.Avatar,
                             ParentId = node.ParentId,
                         };

            return Json(result);
        }

        public ActionResult LoadOnDemand(int? Id)
        {
            IEnumerable<EmployeeNodeViewModel> result;

            result = _employeesRepo.AllNodes().Where(e => e.ParentId == Id)
                .Select(e => new EmployeeNodeViewModel
                {
                    Id = e.Id,
                    Position = e.Position,
                    FullName = e.FullName,
                    Avatar = e.Avatar,
                    ParentId = e.ParentId,
                    Expanded = e.Expanded,
                    HasChildren = _employeesRepo.AllNodes().Where(s => s.ParentId == e.Id).Count() > 0
                });

            return Json(result);
        }

        public JsonResult Create(EmployeeNodeViewModel model)
        {
            int lastID = _employeesRepo.AllNodes().Select(m => m.Id).Max();
            if (model.Id == 0)
            {
                model.Id = lastID + 1;
            }
            _employeesRepo.AllNodes().Add(model);

            return Json(model);
        }

        public JsonResult Destroy(EmployeeNodeViewModel model)
        {
            var target = One(m => m.Id == model.Id);
            _employeesRepo.AllNodes().Remove(target);

            return Json(target);
        }

        public JsonResult Update(EmployeeNodeViewModel model)
        {
            var target = One(m => m.Id == model.Id);

            target.Position = model.Position;
            target.FullName = model.FullName;
            target.ParentId = model.ParentId;
            target.Avatar = model.Avatar;

            return Json(target);
        }


        public EmployeeNodeViewModel One(Func<EmployeeNodeViewModel, bool> predicate)
        {
            return _employeesRepo.AllNodes().FirstOrDefault(predicate);
        }

       
    }
}
