using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitsLogger_BepInEx.Resources;

namespace UnitsLogger_BepInEx
{
    public static class CustomDictionary
    {
        public static Dictionary<string, string> ru = JsonConvert.DeserializeObject<Dictionary<string, string>>(Encoding.UTF8.GetString(Resource.ru).Trim('\uFEFF'));
        public static Dictionary<string, string> en = JsonConvert.DeserializeObject<Dictionary<string, string>>(Encoding.UTF8.GetString(Resource.en).Trim('\uFEFF'));

        public static string GetLocal(this string key)
        {
            if ((string)Reflection.GetField(LocalizedTextManager.instance.GetType(), LocalizedTextManager.instance, "language") == "ru")
            {
                return ru.ContainsKey(key) ? ru[key] : key;
            }
            else
            {
                return en.ContainsKey(key) ? en[key] : key;
            }
        }

        public static void SetLocal(this string key, string value)
        {
            if ((string)Reflection.GetField(LocalizedTextManager.instance.GetType(), LocalizedTextManager.instance, "language") == "ru")
            {
                ru[key] = value;
            }
            else
            {
                en[key] = value;
            }
        }
    }
}
