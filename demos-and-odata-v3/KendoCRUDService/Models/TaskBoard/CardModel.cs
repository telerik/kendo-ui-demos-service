using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KendoCRUDService.Models
{
    public class ColumnModel
    {
        public int ID { get; set; }
        public string Text { get; set; }
        public int Order { get; set; }
        public string Status { get; set; }
    }
}