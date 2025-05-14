namespace KendoCRUDService.Data.Models
{
    public partial class GanttResourceAssignment
    {
        public int ID { get; set; }
        public int ResourceID { get; set; }
        public int TaskID { get; set; }
        public decimal Units { get; set; }
    }
}
