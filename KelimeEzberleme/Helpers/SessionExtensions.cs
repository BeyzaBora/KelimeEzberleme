using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace KelimeEzberleme.Helpers
{
    public static class SessionExtensions
    {
        // JSON formatında veri kaydetmek için
        public static void SetObjectAsJson(this ISession session, string key, object value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        // JSON formatındaki veriyi geri almak için
        public static T GetObjectFromJson<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
        }
    }
}
