using Newtonsoft.Json;

namespace KendoCoreService.Models.Request
{
    public class AggregateRequest
    {
        [JsonProperty("field")]
        public string? Field { get; set; }

        [JsonProperty("aggregate")]
        public string? Aggregate { get; set; }
    }
}
