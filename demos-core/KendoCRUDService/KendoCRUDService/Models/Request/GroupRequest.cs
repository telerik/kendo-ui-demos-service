using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace KendoCRUDService.Models.Request
{
    public class GroupRequest
    {
        [JsonPropertyName("field")]
        public string? Field { get; set; }

        [JsonPropertyName("dir")]
        public string? Dir { get; set; }

        [JsonPropertyName("aggregates")]
        public List<AggregateRequest>? Aggregates { get; set; }
    }
}
