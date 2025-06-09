using System;
using System.Collections.Generic;

namespace signalr_for_aspnet_core.Models;

public class MeetingSignalR : SchedulerEvent
{
    public Guid? ID { get; set; }
    public int? RoomID { get; set; }
    public IEnumerable<int> Attendees { get; set; }
    public DateTime? CreatedAt { get; set; }
}
