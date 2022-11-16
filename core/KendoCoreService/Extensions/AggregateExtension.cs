using KendoCoreService.Models.Request;

namespace KendoCoreService.Extensions
{
    public static class AggregateExtension
    {
        public static Dictionary<string, Dictionary<string, string>> CalculateAggregates<T>(this IQueryable<T> data, List<AggregateRequest> aggregates)
        {
            if (aggregates == null)
            {
                return new Dictionary<string, Dictionary<string, string>>();
            }

            int count = aggregates.Count;
            var result = new Dictionary<string, Dictionary<string, string>>();

            if (count > 0)
            {
                foreach (var aggregate in aggregates)
                {
                    string field = aggregate.Field;
                    string functionName = aggregate.Aggregate;
                    double aggregateResult = 0;

                    switch (functionName)
                    {
                        case "sum":
                            aggregateResult = data.Sum(x => CommonExtension.ParseObjectToDouble(x, field));
                            break;
                        case "average":
                            aggregateResult = data.Average(x => CommonExtension.ParseObjectToDouble(x, field));
                            break;
                        case "max":
                            aggregateResult = data.Max(x => CommonExtension.ParseObjectToDouble(x, field));
                            break;
                        case "min":
                            aggregateResult = data.Min(x => CommonExtension.ParseObjectToDouble(x, field));
                            break;
                        case "count":
                            aggregateResult = data.Select(x => CommonExtension.GetValueByPropertyName(x, field)).Count();
                            break;
                    }

                    string functionResult = aggregateResult.ToString();

                    if (!result.ContainsKey(field))
                    {
                        result.Add(field, new Dictionary<string, string> { { functionName, functionResult } });
                    }
                    else
                    {
                        result[field].Add(functionName, functionResult);
                    }
                }

                return result;
            }

            return new Dictionary<string, Dictionary<string, string>>();
        }
    }
}
