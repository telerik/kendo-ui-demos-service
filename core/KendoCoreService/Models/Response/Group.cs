using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace KendoCoreService.Models.Response
{
    public class Group
    {
        [JsonProperty("field")]
        public string? Field { get; set; }

        [JsonProperty("value")]
        public object? Value { get; set; }

        [JsonProperty("hasSubgroups")]
        public bool HasSubgroups { get; set; }

        [JsonProperty("items")]
        public IList? Items { get; set; }

        [JsonProperty("itemCount")]
        public int ItemCount { get; set; }

        [JsonProperty("subgroupCount")]
        public int SubgroupCount { get; set; }

        [JsonProperty("aggregates")]
        public Dictionary<string, Dictionary<string, string>>? Aggregates { get; set; }
    }
}
