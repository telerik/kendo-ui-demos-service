using System;
using System.Collections.Generic;

namespace kendo_northwind_pg.Data.Models;

public partial class GanttTask
{
    public int Id { get; set; }

    public int? ParentId { get; set; }

    public int OrderId { get; set; }

    public string Title { get; set; }

    public DateTime Start { get; set; }

    public DateTime End { get; set; }

    public double PercentComplete { get; set; }

    public bool Expanded { get; set; }

    public bool Summary { get; set; }

    public virtual ICollection<GanttTask> InverseParent { get; set; } = new List<GanttTask>();

    public virtual GanttTask Parent { get; set; }
}