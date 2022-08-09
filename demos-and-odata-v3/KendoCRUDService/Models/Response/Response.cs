using System.Collections;
using System.Collections.Generic;

namespace KendoCRUDService.Models.Response
{
    public class Response
    {
        public Response(IList data, Dictionary<string, Dictionary<string, string>> aggregates, int total, bool isGrouped)
        {
            Data = data;
            Total = total;
            Aggregates = aggregates;
            IsGrouped = isGrouped;
        }

        private IList Data { get; set; }

        private Dictionary<string, Dictionary<string, string>> Aggregates { get; set; }

        private int Total { get; set; }

        private bool IsGrouped { get; set; }

        public object ToResult()
        {
            if(IsGrouped)
            {
                return new
                {
                    Groups = Data,
                    Total = Total,
                    Aggregates = Aggregates
                };
            }

            return new
            {
                Data = Data,
                Total = Total,
                Aggregates = Aggregates
            };
        }
    }
}