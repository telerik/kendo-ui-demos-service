using KendoCRUDService.Models.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KendoCRUDService.Common
{
    public static class SortingExtension
    {
        public static IQueryable<T> Sort<T>(this IQueryable<T> data, List<SortRequest> sorts)
        {
            bool isFirst = true;

            foreach (var sort in sorts)
            {
                var propInfo = CommonExtension.GetPropertyInfo(typeof(T), sort.Field);
                var expr = ExpressionExtension.BuildLambda(typeof(T), propInfo);

                string methodName = string.Empty;

                if (isFirst)
                {
                    isFirst = false;
                    methodName = sort.Dir == "asc" ? "OrderBy" : "OrderByDescending";
                }
                else
                {
                    methodName = sort.Dir == "asc" ? "ThenBy" : "ThenByDescending";
                }

                var method = typeof(Queryable).GetMethods().FirstOrDefault(m => m.Name == methodName && m.GetParameters().Length == 2);
                var genericMethod = method.MakeGenericMethod(typeof(T), propInfo.PropertyType);

                data = (IQueryable<T>)genericMethod.Invoke(null, new object[] { data, expr });
            }

            return data;
        }
    }
}