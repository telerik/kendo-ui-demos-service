using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using KendoCRUDService.Models;
using System.Web.Caching;
using KendoCRUDService.Models.EF;

namespace KendoCRUDService.Hubs
{
    public class MeetingHub : Hub
    {
        public IEnumerable<MeetingSignalR> Read()
        {
            var meetings = HttpContext.Current.Cache["meetings"] as IEnumerable<MeetingSignalR>;

            if (meetings == null)
            {
                using (var context = new SampleEntities())
                {
                    var createdAt = DateTime.Now;

                    meetings = context.Meetings
                                      .ToList() // Execute the query because Linq to SQL doesn't get Guid.NewGuid()
                                      .Select(meeting => new MeetingSignalR
                                      {
                                            ID = Guid.NewGuid(),
                                            Title = meeting.Title,
                                            Start = DateTime.SpecifyKind(meeting.Start, DateTimeKind.Utc),
                                            End = DateTime.SpecifyKind(meeting.End, DateTimeKind.Utc),
                                            StartTimezone = meeting.StartTimezone,
                                            EndTimezone = meeting.EndTimezone,
                                            Description = meeting.Description,
                                            IsAllDay = meeting.IsAllDay,
                                            RoomID = meeting.RoomID,
                                            RecurrenceRule = meeting.RecurrenceRule,
                                            RecurrenceException = meeting.RecurrenceException,
                                            RecurrenceID = meeting.RecurrenceID,
                                            Attendees = meeting.MeetingAttendees.Select(m => m.AttendeeID).ToArray(),
                                            CreatedAt = createdAt = createdAt.AddMilliseconds(1)
                                      })
                                      .ToList();
                }

                HttpContext.Current.Cache.Add("meetings",
                    meetings,
                    null,
                    Cache.NoAbsoluteExpiration,
                    TimeSpan.FromMinutes(30),
                    System.Web.Caching.CacheItemPriority.Normal,
                    null
                );
            }

            return meetings;
        }

        public void Update(MeetingSignalR meeting)
        {
            Clients.Others.update(meeting);
        }

        public void Destroy(MeetingSignalR meeting)
        {
            Clients.Others.destroy(meeting);
        }

        public MeetingSignalR Create(MeetingSignalR meeting)
        {
            meeting.ID = Guid.NewGuid();
            meeting.CreatedAt = DateTime.Now;

            Clients.Others.create(meeting);

            return meeting;
        }
    }
}