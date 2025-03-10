using KendoCRUDService.Data.Models;
using KendoCRUDService.Data.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace KendoCRUDService.Controllers
{
    public class DiagramShapesController : Controller
    {
        private readonly DiagramShapesRepository _diagramShapesRepository;

        public DiagramShapesController(DiagramShapesRepository diagramShapesRepository)
        {
            _diagramShapesRepository = diagramShapesRepository;
        }

        public ActionResult Index()
        {
            return Json(_diagramShapesRepository.All());
        }

        public JsonResult Update([FromBody] IEnumerable<OrgChartShape> models)
        {
            if (models != null)
            {
                _diagramShapesRepository.Update(models);
            }
            return Json(models);
        }

        public ActionResult Destroy([FromBody] IEnumerable<OrgChartShape> models)
        {
            if (models != null)
            {
                _diagramShapesRepository.Delete(models);
            }
            return Json(models);
        }

        public ActionResult Create([FromBody] IEnumerable<OrgChartShape> models)
        {
            if (models != null)
            {
                _diagramShapesRepository.Insert(models);
            }
            return Json(models);
        }
    }
}
