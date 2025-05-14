namespace KendoCRUDService.Models
{
    public class EmployeeNodeViewModel
    {
        public int Id { get; set; }

        public int? ParentId { get; set; }

        public string Avatar { get; set; }

        public bool HasChildren { get; set; }

        public bool Expanded { get; set; }

        public string FullName { get; set; }

        public string Position { get; set; }
    }
}
