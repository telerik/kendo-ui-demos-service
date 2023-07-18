using KendoCRUDService.Data.Models;
using KendoCRUDService.Data.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace KendoCRUDService.Controllers
{
    public class DiagramConnectionsController : Controller
    {
        private readonly DiagramConnectionsRepository _diagramConnectionsRepository;
        public DiagramConnectionsController(DiagramConnectionsRepository diagramConnectionsRepository)
        {
            _diagramConnectionsRepository = diagramConnectionsRepository;
        }

        public ActionResult Index()
        {
            return Json(_diagramConnectionsRepository.All());
        }

        public JsonResult Update(IEnumerable<OrgChartConnection> models)
        {
            if (models != null)
            {
                _diagramConnectionsRepository.Update(models);
            }
            return Json(models);
        }

        public ActionResult Destroy(IEnumerable<OrgChartConnection> models)
        {
            if (models != null)
            {
                _diagramConnectionsRepository.Delete(models);
            }
            return Json(models);
        }

        public ActionResult Create(IEnumerable<OrgChartConnection> models)
        {
            if (models != null)
            {
                _diagramConnectionsRepository.Insert(models);
            }
            return Json(models);
        }
    }
}
