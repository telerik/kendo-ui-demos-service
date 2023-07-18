using KendoCRUDService.Data.Models;
using KendoCRUDService.Data.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace KendoCRUDService.Controllers
{
    public class GanttResourceAssignmentsController : Controller
    {
        private readonly GanttResourceAssignmentsRepository _ganttResourceAssignmentsRepository;

        public GanttResourceAssignmentsController(GanttResourceAssignmentsRepository ganttResourceAssignmentsRepository)
        {
            _ganttResourceAssignmentsRepository = ganttResourceAssignmentsRepository;
        }

        public ActionResult Index()
        {
            return Json(_ganttResourceAssignmentsRepository.All());
        }

        public JsonResult Update(IEnumerable<GanttResourceAssignment> models)
        {
            if (models != null)
            {
                _ganttResourceAssignmentsRepository.Update(models);
            }
            return Json(models);
        }

        public ActionResult Destroy(IEnumerable<GanttResourceAssignment> models)
        {
            if (models != null)
            {
                _ganttResourceAssignmentsRepository.Delete(models);
            }
            return Json(models);
        }

        public ActionResult Create(IEnumerable<GanttResourceAssignment> models)
        {
            if (models != null)
            {
                _ganttResourceAssignmentsRepository.Insert(models);
            }
            return Json(models);
        }
    }
}
