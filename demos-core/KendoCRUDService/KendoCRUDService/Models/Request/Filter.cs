using System.Text.Json.Serialization;

namespace KendoCRUDService.Models.Request
{
    public class Filter
    {
        [JsonPropertyName("logic")]
        public string? Logic { get; set; }

        [JsonPropertyName("field")]
        public string? Field { get; set; }

        [JsonPropertyName("operator")]
        public string? Operator { get; set; }

        [JsonPropertyName("value")]
        public object? Value { get; set; }

        [JsonPropertyName("filters")]
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
