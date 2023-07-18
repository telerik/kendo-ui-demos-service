using KendoCRUDService.Data.Models;
using KendoCRUDService.Data.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace KendoCRUDService.Controllers
{
    public class GanttTasksController : Controller
    {
        private readonly GanttTaskRepository _ganttTaskRepository;

        public GanttTasksController(GanttTaskRepository ganttTaskRepository)
        {
            _ganttTaskRepository = ganttTaskRepository;
        }

        public ActionResult Index()
        {
            return Json(_ganttTaskRepository.All());
        }

        public JsonResult Update(IEnumerable<GanttTask> models)
        {
            if (models != null)
            {
                _ganttTaskRepository.Update(models);
            }
            return Json(models);
        }

        public ActionResult Destroy(IEnumerable<GanttTask> models)
        {
            if (models != null)
            {
                _ganttTaskRepository.Delete(models);
            }
            return Json(models);
        }

        public ActionResult Create(IEnumerable<GanttTask> models)
        {
            if (models != null)
            {
                _ganttTaskRepository.Insert(models);
            }
            return Json(models);
        }
    }
}
