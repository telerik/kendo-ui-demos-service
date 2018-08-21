using System;
using System.Collections.Generic;

namespace graphql_aspnet_core.Data
{
    public partial class GanttTask
    {
        public int ID { get; set; }
        public DateTime End { get; set; }
        public bool Expanded { get; set; }
        public int OrderID { get; set; }
        public int? ParentID { get; set; }
        public decimal PercentComplete { get; set; }
        public DateTime Start { get; set; }
        public bool Summary { get; set; }
        public string Title { get; set; }

        public virtual GanttTask Parent { get; set; }
        public virtual ICollection<GanttTask> InverseParent { get; set; }
    }
}
