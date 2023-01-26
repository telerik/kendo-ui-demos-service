using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace KendoCoreService.Models.Request
{
    public class Request
    {
        [JsonProperty("skip")]
        public int Skip { get; set; }

        [JsonProperty("take")]
        public int Take { get; set; }

        [JsonProperty("groupPaging")]
        public bool GroupPaging { get; set; }

        [JsonProperty("sort")]
        public List<Sort>? Sorts { get; set; }

        [JsonProperty("filter")]
        public Filter? Filter { get; set; }

        [JsonProperty("group")]
        public List<GroupRequest>? Groups { get; set; }

        [JsonProperty("aggregate")]
        public List<AggregateRequest>? Aggregates { get; set; }
    }
}
