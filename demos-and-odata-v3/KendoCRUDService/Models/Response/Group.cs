using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;

namespace KendoCRUDService.Models.Response
{
    public class Group
    {
        [JsonProperty(PropertyName = "field")]
        public string Field { get; set; }

        [JsonProperty(PropertyName = "value")]
        public object Value { get; set; }

        [JsonProperty(PropertyName = "hasSubgroups")]
        public bool HasSubgroups { get; set; }

        [JsonProperty(PropertyName = "items")]
        public IList Items { get; set; }

        [JsonProperty(PropertyName = "itemCount")]
        public int ItemCount { get; set; }

        [JsonProperty(PropertyName = "subgroupCount")]
        public int SubgroupCount { get; set; }

        [JsonProperty(PropertyName = "aggregates")]
        public Dictionary<string, Dictionary<string, string>> Aggregates { get; set; }
    }
}