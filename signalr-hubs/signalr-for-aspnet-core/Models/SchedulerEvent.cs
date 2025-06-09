
using System;

namespace signalr_for_aspnet_core.Models;

public abstract class SchedulerEvent
{
    public string Title { get; set; }
    public string Description { get; set; }

    public string StartTimezone { get; set; }

    private DateTime start;
    public DateTime Start
    {
        get
        {
            return start;
        }
        set
        {
            start = value.ToUniversalTime();
        }
    }


    private DateTime end;
    public DateTime End
    {
        get
        {
            return end;
        }
        set
        {
            end = value.ToUniversalTime();
        }
    }


    public string EndTimezone { get; set; }
    public string RecurrenceRule { get; set; }
    public int? RecurrenceID { get; set; }
    public string RecurrenceException { get; set; }
    public bool IsAllDay { get; set; }
}
