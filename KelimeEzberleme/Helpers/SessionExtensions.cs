using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace KelimeEzberleme.Helpers
{
    public static class SessionExtensions
    {
        // Nesneyi JSON olarak string'e çevirip Session'a kaydeder
        public static void SetObjectAsJson(this ISession session, string key, object value)
        {
            if (value == null)
            {
                session.Remove(key);
                return;
            }

            string jsonData = JsonConvert.SerializeObject(value);
            session.SetString(key, jsonData);
        }

        // Session'dan JSON string alır ve nesneye deserialize eder
        public static T GetObjectFromJson<T>(this ISession session, string key)
        {
            var jsonData = session.GetString(key);
            if (string.IsNullOrEmpty(jsonData))
            {
                return default;
            }

            return JsonConvert.DeserializeObject<T>(jsonData);
        }
    }
}
