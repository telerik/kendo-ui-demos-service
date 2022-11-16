using Newtonsoft.Json;

namespace KendoCoreService.Models.Request
{
    public class Sort
    {
        [JsonProperty("field")]
        public string? Field { get; set; }

        [JsonProperty("dir")]
        public string? Dir { get; set; }
    }
}
