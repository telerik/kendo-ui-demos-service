using System;
using System.Collections.Generic;

namespace signalr_for_aspnet_core.Models
{
    public partial class GanttResourceAssignment
    {
        public int ID { get; set; }
        public int ResourceID { get; set; }
        public int TaskID { get; set; }
        public decimal Units { get; set; }
    }
}
