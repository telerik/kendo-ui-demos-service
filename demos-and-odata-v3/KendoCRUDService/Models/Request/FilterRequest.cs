using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KendoCRUDService.Models.Request
{
    public class FilterRequest
    {
        [JsonProperty(PropertyName = "logic")]
        public string Logic { get; set; }

        [JsonProperty(PropertyName = "field")]
        public string Field { get; set; }

        [JsonProperty(PropertyName = "operator")]
        public string Operator { get; set; }

        [JsonProperty(PropertyName = "value")]
        public object Value { get; set; }

        [JsonProperty(PropertyName = "filters")]
        public List<FilterRequest> Filters { get; set; }

        public bool IsDescriptor
        {
            get
            {
                return this.Field != null && this.Operator != null;
            }
        }
    }
}