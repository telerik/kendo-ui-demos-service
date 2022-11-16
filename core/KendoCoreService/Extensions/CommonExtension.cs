using System;
using System.Linq;
using System.Reflection;

namespace KendoCoreService.Extensions
{
    public static class CommonExtension
    {
        public static object GetValueByPropertyName(object obj, string propertyName)
        {
            return obj.GetType().GetProperty(propertyName).GetValue(obj, null) ?? "null";
        }

        public static PropertyInfo GetPropertyInfo(Type objType, string propertyName)
        {
            return objType.GetProperties().FirstOrDefault(p => p.Name == propertyName);
        }

        public static double ParseObjectToDouble(object data, string field)
        {
            return double.Parse(GetValueByPropertyName(data, field).ToString());
        }

        public static bool IsNumber(this object value)
        {
            return double.TryParse(value.ToString(), out double r);
        }

        public static bool IsNull(this string str)
        {
            return str == "null";
        }

        public static bool IsNotNull(this string str)
        {
            return str != "null";
        }

        public static bool IsEmpty(this string str)
        {
            return str.Length == 0;
        }

        public static bool IsNotEmpty(this string str)
        {
            return str.Length != 0;
        }
    }
}
