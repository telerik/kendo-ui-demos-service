using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KendoCRUDService.Models
{
    public class EmployeeNodeModel
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