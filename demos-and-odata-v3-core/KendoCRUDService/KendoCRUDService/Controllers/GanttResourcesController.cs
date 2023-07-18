using KendoCRUDService.Data.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace KendoCRUDService.Controllers
{
    public class GanttResourcesController : Controller
    {
        private readonly GanttResourcesRepository _ganttResourcesRepository;

        public GanttResourcesController(GanttResourcesRepository ganttResourcesRepository)
        {
            _ganttResourcesRepository = ganttResourcesRepository;
        }

        public ActionResult Index()
        {
            return Json(_ganttResourcesRepository.All());
        }
    }
}
