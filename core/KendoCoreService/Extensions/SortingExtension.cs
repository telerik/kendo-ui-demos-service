using KendoCoreService.Models.Request;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace KendoCoreService.Extensions
{
    public static class SortingExtension
    {
        public static IQueryable<T> Sort<T>(this IQueryable<T> data, List<Sort> sorts)
        {
            bool isFirst = true;

            foreach (var sort in sorts)
            {
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

                data = data.ApplySort(methodName, sort.Field);
            }

            return data;
        }

        public static IQueryable<T> GroupSort<T>(this IQueryable<T> data, string dir)
        {
            var methodName = dir == "asc" ? "OrderBy" : "OrderByDescending";

            return data.ApplySort(methodName, "Key");
        }

        private static IQueryable<T> ApplySort<T>(this IQueryable<T> data, string methodName, string field)
        {
            var propInfo = CommonExtension.GetPropertyInfo(typeof(T), field);
            var expr = ExpressionExtension.BuildLambda(typeof(T), propInfo);

            var method = typeof(Queryable).GetMethods().FirstOrDefault(m => m.Name == methodName && m.GetParameters().Length == 2);
            var genericMethod = method.MakeGenericMethod(typeof(T), propInfo.PropertyType);

            return (IQueryable<T>)genericMethod.Invoke(null, new object[] { data, expr });
        }
    }
}
