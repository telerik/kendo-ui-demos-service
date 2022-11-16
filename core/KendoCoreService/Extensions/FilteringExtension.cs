using KendoCoreService.Models.Request;

namespace KendoCoreService.Extensions
{
    public static class FilteringExtension
    {

        public static IQueryable<T> Filter<T>(this IQueryable<T> data, Filter filter)
        {
            var filterExpression = ExpressionExtension.GenerateFilterExpression<T>(filter);
           
            if(filterExpression != null)
            {
                return data.Where(filterExpression);
            }

            return data;
        }

        public static string GetOperator(string key)
        {
            Dictionary<string, string> operators = new Dictionary<string, string>()
            {
                { "eq", "Equals" },
                { "neq", "DoesNotEqual" },
                { "doesnotcontain", "DoesNotContain" },
                { "contains", "Contains" },
                { "startswith", "StartsWith" },
                { "endswith", "EndsWith" },
                { "isnull", "IsNull" },
                { "isnotnull", "IsNotNull" },
                { "isempty", "IsEmpty" },
                { "isnotempty", "IsNotEmpty" },
                { "gt", "GreaterThan" },
                { "gte", "GreaterThanOrEqual" },
                { "lt", "LessThan" },
                { "lte", "LessThanOrEqual" }
            };

            return operators[key];
        }
    }
}
