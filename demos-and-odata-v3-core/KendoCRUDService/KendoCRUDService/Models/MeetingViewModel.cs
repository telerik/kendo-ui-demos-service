﻿using KendoCRUDService.Data.Models;

namespace KendoCRUDService.Models
{
    public class MeetingViewModel : SchedulerEvent
    {
        public int MeetingID { get; set; }
        public int? RoomID { get; set; }
        public IEnumerable<int> Attendees { get; set; }

        public Meeting ToEntity()
        {
            return new Meeting
            {
                MeetingID = MeetingID,
                Title = Title,
                Start = Start,
                StartTimezone = StartTimezone,
                End = End,
                EndTimezone = EndTimezone,
                Description = Description,
                IsAllDay = IsAllDay,
                RecurrenceRule = RecurrenceRule,
                RecurrenceException = RecurrenceException,
                RecurrenceID = RecurrenceID,
                RoomID = RoomID
            };
        }
    }
}
