using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace KendoCRUDService.Models.Request
{
    public class Request
    {
        [JsonPropertyName("skip")]
        public int Skip { get; set; }

        [JsonPropertyName("take")]
        public int Take { get; set; }

        [JsonPropertyName("groupPaging")]
        public bool GroupPaging { get; set; }

        [JsonPropertyName("sort")]
        public List<Sort>? Sorts { get; set; }

        [JsonPropertyName("filter")]
        public Filter? Filter { get; set; }

        [JsonPropertyName("group")]
        public List<GroupRequest>? Groups { get; set; }

        [JsonPropertyName("aggregate")]
        public List<AggregateRequest>? Aggregates { get; set; }
    }
}
