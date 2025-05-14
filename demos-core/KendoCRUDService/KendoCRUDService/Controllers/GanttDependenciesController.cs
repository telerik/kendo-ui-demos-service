using KendoCRUDService.Data.Models;
using KendoCRUDService.Data.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace KendoCRUDService.Controllers
{
    public class GanttDependenciesController : Controller
    {
        private readonly GanttDependencyRepository _ganttDependencyRepository;

        public GanttDependenciesController(GanttDependencyRepository ganttDependencyRepository)
        {
            _ganttDependencyRepository = ganttDependencyRepository;
        }

        public ActionResult Index()
        {
            return Json(_ganttDependencyRepository.All());
        }

        public JsonResult Update([FromBody] IEnumerable<GanttDependency> models)
        {
            if (models != null)
            {
                _ganttDependencyRepository.Update(models);
            }
            return Json(models);
        }

        public ActionResult Destroy([FromBody] IEnumerable<GanttDependency> models)
        {
            if (models != null)
            {
                _ganttDependencyRepository.Delete(models);
            }
            return Json(models);
        }

        public ActionResult Create([FromBody] IEnumerable<GanttDependency> models)
        {
            if (models != null)
            {
                _ganttDependencyRepository.Insert(models);
            }
            return Json(models);
        }
    }
}
