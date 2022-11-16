using Newtonsoft.Json;

namespace KendoCoreService.Extensions
{
    public static class SessionExtension
    {
        public static T GetList<T>(this ISession session, string key)
        {
            var list = session.GetString(key);
            if(list == null)
            {
                return default(T);
            }

            return JsonConvert.DeserializeObject<T>(list);
        }

        public static void SetList<T>(this ISession session, string key, IList<T> value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }
    }
}
