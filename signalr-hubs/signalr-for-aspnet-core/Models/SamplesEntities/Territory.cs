using System;
using System.Collections.Generic;

namespace signalr_for_aspnet_core.Models
{
    public partial class Territory
    {
        public Territory()
        {
            EmployeeTerritories = new HashSet<EmployeeTerritory>();
        }

        public string TerritoryID { get; set; }
        public int RegionID { get; set; }
        public string TerritoryDescription { get; set; }

        public virtual ICollection<EmployeeTerritory> EmployeeTerritories { get; set; }
        public virtual Region Region { get; set; }
    }
}
