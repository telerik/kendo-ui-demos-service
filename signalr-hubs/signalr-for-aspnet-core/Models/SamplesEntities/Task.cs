using System;
using System.Collections.Generic;

namespace signalr_for_aspnet_core.Models
{
    public partial class Task
    {
        public int TaskID { get; set; }
        public string Description { get; set; }
        public DateTime End { get; set; }
        public string EndTimezone { get; set; }
        public bool IsAllDay { get; set; }
        public int? OwnerID { get; set; }
        public string RecurrenceException { get; set; }
        public int? RecurrenceID { get; set; }
        public string RecurrenceRule { get; set; }
        public DateTime Start { get; set; }
        public string StartTimezone { get; set; }
        public string Title { get; set; }

        public virtual Task Recurrence { get; set; }
        public virtual ICollection<Task> InverseRecurrence { get; set; }
    }
}
