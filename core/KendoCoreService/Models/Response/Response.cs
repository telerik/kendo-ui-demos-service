using System.Collections;

namespace KendoCoreService.Models.Response
{
    public class Response
    {
        public Response(IList data, Dictionary<string, Dictionary<string, string>> aggregates, int total, bool isGrouped)
        {
            if(isGrouped)
            {
                Groups = data;
            }
            else
            {
                Data = data;
            }

            Total = total;
            Aggregates = aggregates;
        }

        private IList Data { get; set; }

        private IList Groups { get; set; }

        private Dictionary<string, Dictionary<string, string>> Aggregates { get; set; }

        private int Total { get; set; }

        public object ToResult()
        {
            return new
            {
                Data,
                Groups,
                Total,
                Aggregates
            };
        }
    }
}
