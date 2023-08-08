using KendoCRUDService.Data.Models;

namespace KendoCRUDService.Models
{
    public class TaskViewModel : SchedulerEvent
    {
        public int TaskID { get; set; }
        public int? OwnerID { get; set; }

        public Data.Models.Task ToEntity()
        {
            return new Data.Models.Task
            {
                TaskID = TaskID,
                Title = Title,
                Start = Start,
                StartTimezone = StartTimezone,
                End = End,
                EndTimezone = EndTimezone,
                Description = Description,
                RecurrenceRule = RecurrenceRule,
                RecurrenceException = RecurrenceException,
                RecurrenceID = RecurrenceID,
                IsAllDay = IsAllDay,
                OwnerID = OwnerID
            };
        }
    }
}
