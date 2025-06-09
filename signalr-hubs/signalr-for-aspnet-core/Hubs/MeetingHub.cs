using Microsoft.AspNetCore.SignalR;
using signalr_for_aspnet_core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Task = System.Threading.Tasks.Task;

namespace signalr_for_aspnet_core.Hubs;

public class MeetingHub(SampleEntitiesDataContext context) : Hub
{
    public override Task OnConnectedAsync()
    {
        Groups.AddToGroupAsync(Context.ConnectionId, GetGroupName());
        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception e)
    {
        Groups.RemoveFromGroupAsync(Context.ConnectionId, GetGroupName());
        return base.OnDisconnectedAsync(e);
    }

    public IEnumerable<MeetingSignalR> Read()
    {
        var createdAt = DateTime.Now;
        var meetings = context.Meetings
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
            }).ToList();

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

    public string GetGroupName()
    {
        return GetRemoteIpAddress();
    }

    public string GetRemoteIpAddress()
    {
        return Context.GetHttpContext()?.Connection.RemoteIpAddress?.ToString();
    }
}