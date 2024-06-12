using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Text;
using UnitsLogger_BepInEx.Resources;

namespace UnitsLogger_BepInEx
{
    public static class CustomDictionary
    {
        private static readonly ConcurrentDictionary<string, string> ru;
        private static readonly ConcurrentDictionary<string, string> en;

        static CustomDictionary()
        {
            ru = JsonConvert.DeserializeObject<ConcurrentDictionary<string, string>>(Encoding.UTF8.GetString(Resource.ru).Trim('\uFEFF'));
            en = JsonConvert.DeserializeObject<ConcurrentDictionary<string, string>>(Encoding.UTF8.GetString(Resource.en).Trim('\uFEFF'));
        }

        public static string GetLocal(this string key)
        {
            var lang = (string)Reflection.GetField(LocalizedTextManager.instance.GetType(), LocalizedTextManager.instance, "language");
            var dictionary = lang == "ru" ? ru : en;
            return dictionary.TryGetValue(key, out var result) ? result : key;
        }

        public static void SetLocal(this string key, string value)
        {
            var lang = (string)Reflection.GetField(LocalizedTextManager.instance.GetType(), LocalizedTextManager.instance, "language");
            var dictionary = lang == "ru" ? ru : en;
            dictionary[key] = value;
        }
    }
}
