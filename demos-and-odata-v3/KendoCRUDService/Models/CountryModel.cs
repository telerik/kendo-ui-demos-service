using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KendoCRUDService.Models
{
    public class CountryModel
    {
        public byte CountryID { get; set; }
        public string CountryNameShort { get; set; }
        public string CountryNameLong { get; set; }
    }
}