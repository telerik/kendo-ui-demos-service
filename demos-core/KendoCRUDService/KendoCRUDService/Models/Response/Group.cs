using System.Collections;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace KendoCRUDService.Models.Response
{
    public class Group
    {
        [JsonPropertyName("field")]
        public string? Field { get; set; }

        [JsonPropertyName("value")]
        public object? Value { get; set; }

        [JsonPropertyName("hasSubgroups")]
        public bool HasSubgroups { get; set; }

        [JsonPropertyName("items")]
        public IList? Items { get; set; }

        [JsonPropertyName("itemCount")]
        public int ItemCount { get; set; }

        [JsonPropertyName("subgroupCount")]
        public int SubgroupCount { get; set; }

        [JsonPropertyName("aggregates")]
        public Dictionary<string, Dictionary<string, string>>? Aggregates { get; set; }
    }
}
