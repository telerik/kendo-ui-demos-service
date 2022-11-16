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

        public static IQueryable<T> GroupSort<T>(this IQueryable<T> data, string dir)
        {
            var propInfo = CommonExtension.GetPropertyInfo(typeof(T), "Key");
            var expr = ExpressionExtension.BuildLambda(typeof(T), propInfo);
            var methodName = dir == "asc" ? "OrderBy" : "OrderByDescending";


            var method = typeof(Queryable).GetMethods().FirstOrDefault(m => m.Name == methodName && m.GetParameters().Length == 2);
            var genericMethod = method.MakeGenericMethod(typeof(T), propInfo.PropertyType);

            return (IQueryable<T>)genericMethod.Invoke(null, new object[] { data, expr });
        }
    }
}
