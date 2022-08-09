using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KendoCRUDService.Models.Request
{
    public class Request
    {
        [JsonProperty(PropertyName = "skip")]
        public int Skip { get; set; }

        [JsonProperty(PropertyName = "take")]
        public int Take { get; set; }

        [JsonProperty(PropertyName = "sort")]
        public List<SortRequest> Sorts { get; set; }

        [JsonProperty(PropertyName = "filter")]
        public FilterRequest Filter { get; set; }

        [JsonProperty(PropertyName = "group")]
        public List<GroupRequest> Groups { get; set; }

        [JsonProperty(PropertyName = "aggregate")]
        public List<AggregateRequest> Aggregates { get; set; }
    }
}