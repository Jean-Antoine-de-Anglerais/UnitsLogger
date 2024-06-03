using System.Collections.Generic;

namespace UnitsLogger_BepInEx
{
    public static class Localizer
    {
        public static void SetLocalization(string planguage, string id, string name)
        {
            string language = Reflection.GetField(LocalizedTextManager.instance.GetType(), LocalizedTextManager.instance, "language") as string;
            string templanguage;

            templanguage = language;

            if (templanguage != "ru" && templanguage != "en")
            {
                templanguage = "en";
            }

            if (planguage == templanguage)
            {
                Dictionary<string, string> localizedText = Reflection.GetField(LocalizedTextManager.instance.GetType(), LocalizedTextManager.instance, "localizedText") as Dictionary<string, string>;
                if (!localizedText.ContainsKey(id))
                {
                    localizedText.Add(id, name);
                }
                else if (localizedText.ContainsKey(id))
                {
                    localizedText.Remove(id);
                    localizedText.Add(id, name);
                }
            }
        }

        public static string GetLocalization(this string id)
        {
            Dictionary<string, string> localizedText = Reflection.GetField(LocalizedTextManager.instance.GetType(), LocalizedTextManager.instance, "localizedText") as Dictionary<string, string>;

            if (localizedText.ContainsKey(id))
            {
                return localizedText[id];
            }

            else
            {
                return id;
            }
        }
    }
}
