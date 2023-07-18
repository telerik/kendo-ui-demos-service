using KendoCRUDService.Data.Repositories;
using KendoCRUDService.Models;
using Microsoft.AspNetCore.Mvc;

namespace KendoCRUDService.Controllers
{
    public class HierarchicalMeetingsController : Controller
    {
        private readonly MeetingsRepository _meetingsRepository;

        public HierarchicalMeetingsController(MeetingsRepository meetingsRepository)
        {
            _meetingsRepository = meetingsRepository;
        }

        public ActionResult Index()
        {
            var result = GetHierarchicalResources();
            return Json(result);
        }

        public IEnumerable<MeetingViewModel> GetHierarchicalResources()
        {
            var firstRoomAttendees = new List<int>() { 1, 2 };
            var secondRoomAttendees = new List<int>() { 1, 3 };

            var result = _meetingsRepository.All()
                .Where(s => (s.RoomID == 1 && s.Attendees.All(p => firstRoomAttendees.Contains(p))) ||
                            (s.RoomID == 2 && s.Attendees.All(p => secondRoomAttendees.Contains(p))));
            return result;
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
