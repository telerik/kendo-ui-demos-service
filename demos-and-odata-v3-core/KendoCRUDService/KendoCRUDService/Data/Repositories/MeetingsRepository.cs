using KendoCRUDService.Data.Models;
using KendoCRUDService.Models;
using KendoCRUDService.SessionExtensions;
using Microsoft.EntityFrameworkCore;

namespace KendoCRUDService.Data.Repositories
{
    public class MeetingsRepository
    {
        private bool UpdateDatabase = false;

        private readonly ISession _session;
        private readonly IDbContextFactory<DemoDbContext> _contextFactory;

        public MeetingsRepository(IHttpContextAccessor httpContextAccessor, IDbContextFactory<DemoDbContext> contextFactory)
        {
            _session = httpContextAccessor.HttpContext.Session;
            _contextFactory = contextFactory;
        }

        public IList<MeetingViewModel> All()
        {
            var result = _session.GetObjectFromJson<IList<MeetingViewModel>>("Meetings");
            if (result == null || UpdateDatabase)
            {
                using (var db = _contextFactory.CreateDbContext())
                {
                    result = db.Meetings.ToList().Select(meeting => new MeetingViewModel
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

                _session.SetObjectAsJson("Meetings", result);
            }

            return result;
        }

        public MeetingViewModel One(Func<MeetingViewModel, bool> predicate)
        {
            return All().FirstOrDefault(predicate);
        }

        public void Insert(MeetingViewModel meeting)
        {
            if (!UpdateDatabase)
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
            else
            {
                using (var db = _contextFactory.CreateDbContext())
                {
                    if (meeting.Attendees == null)
                    {
                        meeting.Attendees = new int[0];
                    }

                    var entity = meeting.ToEntity();

                    foreach (var attendeeId in meeting.Attendees)
                    {
                        entity.MeetingAttendees.Add(new MeetingAttendee
                        {
                            AttendeeID = attendeeId
                        });
                    }

                    db.Meetings.Add(entity);
                    db.SaveChanges();

                    meeting.MeetingID = entity.MeetingID;
                }
            }
        }

        public void Update(MeetingViewModel meeting)
        {
            if (!UpdateDatabase)
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
            else
            {
                using (var db = _contextFactory.CreateDbContext())
                {
                    if (meeting.Attendees == null)
                    {
                        meeting.Attendees = new int[0];
                    }

                    var entity = meeting.ToEntity();

                    db.Meetings.Attach(entity);

                    var attendees = meeting.Attendees.Select(attendee => new MeetingAttendee
                    {
                        AttendeeID = attendee
                    });

                    foreach (var attendee in attendees)
                    {
                        db.MeetingAttendees.Attach(attendee);
                    }

                    entity.MeetingAttendees.Clear();

                    foreach (var attendee in attendees)
                    {
                        entity.MeetingAttendees.Add(attendee);
                    }

                    db.SaveChanges();
                }
            }
        }

        public void Delete(MeetingViewModel meeting)
        {
            if (!UpdateDatabase)
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
            else
            {
                using (var db = _contextFactory.CreateDbContext())
                {
                    if (meeting.Attendees == null)
                    {
                        meeting.Attendees = new int[0];
                    }

                    var entity = meeting.ToEntity();

                    db.Meetings.Attach(entity);

                    var attendees = meeting.Attendees.Select(attendee => new MeetingAttendee
                    {
                        AttendeeID = attendee
                    });

                    foreach (var attendee in attendees)
                    {
                        db.MeetingAttendees.Attach(attendee);
                    }

                    entity.MeetingAttendees.Clear();

                    var recurrenceExceptions = db.Meetings.Where(m => m.RecurrenceID == entity.MeetingID);

                    foreach (var recurrenceException in recurrenceExceptions)
                    {
                        db.Meetings.Remove(recurrenceException);
                    }

                    db.Meetings.Remove(entity);
                    db.SaveChanges();
                }
            }
        }
    }
}
