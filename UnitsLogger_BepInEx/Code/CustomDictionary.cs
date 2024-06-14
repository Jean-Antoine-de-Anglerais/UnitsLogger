using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;
using UnitsLogger_BepInEx.Resources;

namespace UnitsLogger_BepInEx
{
    public static class CustomDictionary
    {
        private static readonly Dictionary<string, string> ru;
        private static readonly Dictionary<string, string> en;

        static CustomDictionary()
        {
            if (LocalizedTextManager.instance.language == "ru")
            {
                ru = JsonConvert.DeserializeObject<Dictionary<string, string>>(Encoding.UTF8.GetString(Resource.ru).Trim('\uFEFF'));
            }

            else
            {
                en = JsonConvert.DeserializeObject<Dictionary<string, string>>(Encoding.UTF8.GetString(Resource.en).Trim('\uFEFF'));
            }
        }

        public static string GetLocal(this string key)
        {
            var lang = LocalizedTextManager.instance.language;
            var dictionary = lang == "ru" ? ru : en;
            return dictionary.TryGetValue(key, out var result) ? result : key;
        }

        public static void SetLocal(this string key, string value)
        {
            var lang = LocalizedTextManager.instance.language;
            var dictionary = lang == "ru" ? ru : en;
            dictionary[key] = value;
        }
    }
}
