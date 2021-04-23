using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KendoCRUDService.Models
{
    public class CardModel
    {
        public int ID { get; set; }
        public string Category { get; set; }
        public string Title { get; set; }
        public int Order { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
    }
}