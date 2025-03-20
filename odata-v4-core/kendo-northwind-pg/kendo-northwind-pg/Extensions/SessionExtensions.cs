using Newtonsoft.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace kendo_northwind_pg.Extensions
{
    public static class SessionExtensions
    {
        public static void SetObjectAsJson(this ISession session, string key, object value)
        {
            var options = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve,
                WriteIndented = true
            };
            session.SetString(key, System.Text.Json.JsonSerializer.Serialize(value, options));
        }

        public static T GetObjectFromJson<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            var options = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve,
                WriteIndented = true
            };

            return value == null ? default(T) : System.Text.Json.JsonSerializer.Deserialize<T>(value, options);
        }
    }
}
