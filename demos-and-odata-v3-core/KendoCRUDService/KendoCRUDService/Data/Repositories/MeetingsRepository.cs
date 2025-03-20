using KendoCRUDService.Data.Models;
using KendoCRUDService.Models;
using KendoCRUDService.SessionExtensions;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;

namespace KendoCRUDService.Data.Repositories
{
    public class MeetingsRepository
    {
        private bool UpdateDatabase = false;

        private readonly ISession _session;
        private readonly IServiceScopeFactory _scopeFactory;
        private ConcurrentDictionary<string, IList<MeetingViewModel>> _meetings;
        private IHttpContextAccessor _contextAccessor;

        public MeetingsRepository(IHttpContextAccessor httpContextAccessor, IServiceScopeFactory scopeFactory)
        {
            _session = httpContextAccessor.HttpContext.Session;
            _scopeFactory = scopeFactory;
            _contextAccessor = httpContextAccessor;
            _meetings = new ConcurrentDictionary<string, IList<MeetingViewModel>>();
        }

        public IList<MeetingViewModel> All()
        {
            var userKey = SessionUtils.GetUserKey(_contextAccessor);

            return _meetings.GetOrAdd(userKey, key =>
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<DemoDbContext>();
                    return context.Meetings.Include("MeetingAttendees").ToList().Select(meeting => new MeetingViewModel
                    {
                        MeetingID = meeting.MeetingID,
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
                        Attendees = meeting.MeetingAttendees.Select(m => m.AttendeeID).ToArray()
                    }).ToList();
                }
            });
        }

        public MeetingViewModel One(Func<MeetingViewModel, bool> predicate)
        {
            return All().FirstOrDefault(predicate);
        }

        public void Insert(MeetingViewModel meeting)
        {
            var first = All().OrderByDescending(e => e.MeetingID).FirstOrDefault();

            var id = 0;

            if (first != null)
            {
                id = first.MeetingID;
            }

            meeting.MeetingID = id + 1;

            All().Insert(0, meeting);
        }

        public void Update(MeetingViewModel meeting)
        {
            var target = One(e => e.MeetingID == meeting.MeetingID);

            if (target != null)
            {
                target.Title = meeting.Title;
                target.Start = meeting.Start;
                target.End = meeting.End;
                target.StartTimezone = meeting.StartTimezone;
                target.EndTimezone = meeting.EndTimezone;
                target.Description = meeting.Description;
                target.IsAllDay = meeting.IsAllDay;
                target.RecurrenceRule = meeting.RecurrenceRule;
                target.RoomID = meeting.RoomID;
                target.RecurrenceException = meeting.RecurrenceException;
                target.RecurrenceID = meeting.RecurrenceID;
                target.Attendees = meeting.Attendees;
            }
        }

        public void Delete(MeetingViewModel meeting)
        {
            var target = One(p => p.MeetingID == meeting.MeetingID);
            if (target != null)
            {
                All().Remove(target);

                var recurrenceExceptions = All().Where(m => m.RecurrenceID == meeting.MeetingID).ToList();

                foreach (var recurrenceException in recurrenceExceptions)
                {
                    All().Remove(recurrenceException);
                }
            }
        }
    }
}
