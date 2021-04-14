using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KendoCRUDService.Models;
using KendoCRUDService.Common;

namespace KendoCRUDService.Controllers
{
    public class HierarchicalMeetingsController : Controller
    {        

        public ActionResult Index()
        {
            var result = GetHierarchicalResources();
            return this.Jsonp(result);
        }

        public IEnumerable<MeetingViewModel> GetHierarchicalResources()
        {
            var firstRoomAttendees = new List<int>() { 1, 2 };
            var secondRoomAttendees = new List<int>() { 1, 3 };

            var result = MeetingsRepository.All()
                .Where(s => (s.RoomID == 1 && s.Attendees.All(p => firstRoomAttendees.Contains(p))) ||
                            (s.RoomID == 2 && s.Attendees.All(p => secondRoomAttendees.Contains(p))));
            return result;
        }

        public JsonResult Update()
        {
            var meetings = this.DeserializeObject<IEnumerable<MeetingViewModel>>("models");

            if (meetings != null)
            {
                foreach (var meeting in meetings)
                {
                    MeetingsRepository.Update(meeting);
                }
            }

            return this.Jsonp(meetings);
        }

        public JsonResult Destroy()
        {
            var meetings = this.DeserializeObject<IEnumerable<MeetingViewModel>>("models");

            if (meetings != null)
            {
                foreach (var meeting in meetings)
                {
                    MeetingsRepository.Delete(meeting);
                }
            }

            return this.Jsonp(meetings);
        }

        public JsonResult Create()
        {
            var meetings = this.DeserializeObject<IEnumerable<MeetingViewModel>>("models");

            if (meetings != null)
            {
                foreach (var meeting in meetings)
                {
                    MeetingsRepository.Insert(meeting);
                }
            }

            return this.Jsonp(meetings);
        }
    }
}