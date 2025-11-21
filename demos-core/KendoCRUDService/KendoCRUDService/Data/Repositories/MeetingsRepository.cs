using KendoCRUDService.Data.Models;
using KendoCRUDService.Models;
using KendoCRUDService.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;

namespace KendoCRUDService.Data.Repositories
{
    public class MeetingsRepository
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private IHttpContextAccessor _contextAccessor;
        private readonly IUserDataCache _userCache;
        private readonly ILogger<MeetingsRepository> _logger;
        private static readonly TimeSpan Ttl = TimeSpan.FromMinutes(15);
        private const string LogicalName = "Meetings";

        public MeetingsRepository(IHttpContextAccessor httpContextAccessor, IServiceScopeFactory scopeFactory, IUserDataCache userCache,
            ILogger<MeetingsRepository> logger)
        {
            _scopeFactory = scopeFactory;
            _contextAccessor = httpContextAccessor;
            _userCache = userCache;
            _logger = logger;
        }

        public IList<MeetingViewModel> All()
        {
            var userKey = SessionUtils.GetUserKey(_contextAccessor);

            return _userCache.GetOrCreateList<MeetingViewModel>(userKey, LogicalName, () =>
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
            }, Ttl, sliding:true);
        }

        public MeetingViewModel One(Func<MeetingViewModel, bool> predicate)
        {
            return All().FirstOrDefault(predicate);
        }

        private void UpdateContent(List<MeetingViewModel> entries)
        {
            var userKey = SessionUtils.GetUserKey(_contextAccessor);
            _userCache.GetOrCreateList<MeetingViewModel>(
                userKey,
                LogicalName,
                () =>
                {
                    return entries;
                },
                Ttl,
                sliding: true
            );
        }

        public void Insert(MeetingViewModel meeting)
        {
            var entities = All().ToList();
            var first = entities.OrderByDescending(e => e.MeetingID).FirstOrDefault();

            var id = 0;

            if (first != null)
            {
                id = first.MeetingID;
            }

            meeting.MeetingID = id + 1;

            entities.Add(meeting);
            UpdateContent(entities);
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
                var entities = All().ToList();
                entities.Remove(target);

                var recurrenceExceptions = entities.Where(m => m.RecurrenceID == meeting.MeetingID).ToList();

                foreach (var recurrenceException in recurrenceExceptions)
                {
                    entities.Remove(recurrenceException);
                }

                UpdateContent(entities);
            }
        }
    }
}
