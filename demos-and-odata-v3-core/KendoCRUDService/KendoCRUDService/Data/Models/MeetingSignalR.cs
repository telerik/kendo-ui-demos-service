namespace KendoCRUDService.Data.Models
{
    public class MeetingSignalR : SchedulerEvent
    {
        public Guid? ID { get; set; }
        public int? RoomID { get; set; }
        public IEnumerable<int> Attendees { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
