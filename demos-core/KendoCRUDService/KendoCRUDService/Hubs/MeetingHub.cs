using KendoCRUDService.Data;
using KendoCRUDService.Data.Models;
using KendoCRUDService.Extensions;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace KendoCRUDService.Hubs
{
    public class MeetingHub : BaseHub
    {

        private readonly ISession _session;
        private readonly IDbContextFactory<DemoDbContext> _contextFactory;

        public MeetingHub(IHttpContextAccessor httpContextAccessor, IDbContextFactory<DemoDbContext> contextFactory)
        {
            _session = httpContextAccessor.HttpContext.Session;
            _contextFactory = contextFactory;
        }

        public IEnumerable<MeetingSignalR> Read()
        {
            var meetings = _session.GetObjectFromJson<IEnumerable<MeetingSignalR>>("meetings");

            if (meetings == null)
            {
                using (var context = _contextFactory.CreateDbContext())
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

                _session.SetObjectAsJson("meetings", meetings);
            }

            return meetings;
        }

        public void Update(MeetingSignalR meeting)
        {
            Clients.OthersInGroup(GetGroupName()).SendAsync("update", meeting);
        }

        public void Destroy(MeetingSignalR meeting)
        {
            Clients.OthersInGroup(GetGroupName()).SendAsync("destroy", meeting);
        }

        public MeetingSignalR Create(MeetingSignalR meeting)
        {
            meeting.ID = Guid.NewGuid();
            meeting.CreatedAt = DateTime.Now;

            Clients.OthersInGroup(GetGroupName()).SendAsync("create", meeting);


            return meeting;
        }
    }
}
