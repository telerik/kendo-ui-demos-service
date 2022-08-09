using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using KendoCRUDService.Models.Request;

namespace KendoCRUDService.Common
{
    public class AggregateExtension
    {
        public static Dictionary<string, Dictionary<string, string>> CalculateAggregates(List<AggregateRequest> aggregates, IList data)
        {
            int count = aggregates.Count;
            var result = new Dictionary<string, Dictionary<string, string>>();

            if (count > 0)
            {
                foreach (var aggregate in aggregates)
                {
                    string field = aggregate.Field;
                    string functionName = aggregate.Aggregate;
                    object aggregateResult = null;

                    switch (functionName)
                    {
                        case "sum":
                            aggregateResult = CalculateSumAggregate(data, field);
                            break;
                        case "average":
                            aggregateResult = CalculateAverageAggregate(data, field);
                            break;
                        case "max":
                            aggregateResult = CalculateMaxAggregate(data, field);
                            break;
                        case "min":
                            aggregateResult = CalculateMinAggregate(data, field);
                            break;
                        case "count":
                            aggregateResult = CalculateCountAggregate(data);
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

        private static double CalculateSumAggregate(IList data, string field)
        {
            double sum = 0;

            for (int i = 0; i < data.Count; i++)
            {
                double value = CommonExtension.ParseObjectToDouble(data[i], field);

                sum += value;
            }

            return sum;
        }

        private static double CalculateAverageAggregate(IList data, string field)
        {
            double average = 0;

            for (int i = 0; i < data.Count; i++)
            {
                double value = CommonExtension.ParseObjectToDouble(data[i], field);

                average += value;
            }

            return average / 2;
        }

        private static double CalculateMaxAggregate(IList data, string field)
        {
            double max = 0;

            for (int i = 0; i < data.Count; i++)
            {
                double value = CommonExtension.ParseObjectToDouble(data[i], field);

                if (value > max)
                {
                    max = value;
                }
            }

            return max;
        }

        private static double CalculateMinAggregate(IList data, string field)
        {
            double min = 0;

            for (int i = 0; i < data.Count; i++)
            {
                double value = CommonExtension.ParseObjectToDouble(data[i], field);

                if (value < min)
                {
                    min = value;
                }
            }

            return min;
        }

        private static int CalculateCountAggregate(IList data)
        {
            return data.Count;
        }
    }
}