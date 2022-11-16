using Newtonsoft.Json;

namespace KendoCoreService.Models.Request
{
    public class Filter
    {
        [JsonProperty("logic")]
        public string? Logic { get; set; }

        [JsonProperty("field")]
        public string? Field { get; set; }

        [JsonProperty("operator")]
        public string? Operator { get; set; }

        [JsonProperty("value")]
        public object? Value { get; set; }

        [JsonProperty("filters")]
        public List<Filter>? Filters { get; set; }

        public bool IsDescriptor
        {
            get
            {
                return Field != null && Operator != null;
            }
        }
    }
}
