using KendoCRUDService.Data.Repositories;
using KendoCRUDService.Models;
using Microsoft.AspNetCore.Mvc;

namespace KendoCRUDService.Controllers
{
    public class MeetingsController : Controller
    {
        private readonly MeetingsRepository _meetingsRepository;

        public MeetingsController(MeetingsRepository meetingsRepository)
        {
            _meetingsRepository = meetingsRepository;
        }

        public ActionResult Index()
        {
            return Json(_meetingsRepository.All());
        }

        public JsonResult Update(IEnumerable<MeetingViewModel> models)
        {
            if (models != null)
            {
                foreach (var meeting in models)
                {
                    _meetingsRepository.Update(meeting);
                }
            }

            return Json(models);
        }

        public JsonResult Destroy(IEnumerable<MeetingViewModel> models)
        {
            if (models != null)
            {
                foreach (var meeting in models)
                {
                    _meetingsRepository.Delete(meeting);
                }
            }

            return Json(models);
        }

        public JsonResult Create(IEnumerable<MeetingViewModel> models)
        {
            if (models != null)
            {
                foreach (var meeting in models)
                {
                    _meetingsRepository.Insert(meeting);
                }
            }

            return Json(models);
        }

    }
}
