using System.Text.Json.Serialization;

namespace KendoCRUDService.Models.Request
{
    public class AggregateRequest
    {
        [JsonPropertyName("field")]
        public string? Field { get; set; }

        [JsonPropertyName("aggregate")]
        public string? Aggregate { get; set; }
    }
}
