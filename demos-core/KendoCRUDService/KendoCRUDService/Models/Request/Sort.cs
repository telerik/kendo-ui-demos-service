using System.Text.Json.Serialization;

namespace KendoCRUDService.Models.Request
{
    public class Sort
    {
        [JsonPropertyName("field")]
        public string? Field { get; set; }

        [JsonPropertyName("dir")]
        public string? Dir { get; set; }
    }
}
