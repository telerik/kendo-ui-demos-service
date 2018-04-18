using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KendoCRUDService.Models
{
    public class MeetingSignalR : SchedulerEvent
    {
        public Guid? ID { get; set; }
        public int? RoomID { get; set; }
        public IEnumerable<int> Attendees { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}