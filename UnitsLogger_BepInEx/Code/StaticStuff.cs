using System.Collections.Generic;

namespace UnitsLogger_BepInEx
{
    public static class StaticStuff
    {
        public static string GetDateFromTime(this double time) => World.world.mapStats.getDate(time);

        public static (string, char) DecodeModifier(this string modifier)
        {
            List<char> chars = new List<char>();

            (string, char) output = ("", ' ');

            foreach (var item in modifier)
            {
                if (!char.IsDigit(item))
                {
                    chars.Add(item);
                }

                else if (char.IsDigit(item))
                {
                    output.Item2 = item;
                }
            }

            output.Item1 = string.Concat(chars);

            return output;
        }

        #region Localization
        public static string ProfessionsLocalizationRu(this object input)
        {
            // Создаем новый словарь
            var dictionary = new Dictionary<object, string>
                {
                    { UnitProfession.Null, "Нулевая" },
                    { UnitProfession.Baby, "Ребёнок" },
                    { UnitProfession.Unit, "Юнит" },
                    { UnitProfession.Warrior, "Воин" },
                    { UnitProfession.King, "Король" },
                    { UnitProfession.Leader, "Лидер Поселения" }
                };

            if (dictionary.ContainsKey(input))
            {
                // Если ключ найден, возвращаем соответствующее значение
                return dictionary[input];
            }

            else
            {
                return input.ToString();
            }
        }

        public static string ProfessionsLocalizationEn(this object input)
        {
            return input.ToString();
        }

        public static string GendersLocalizationRu(this object input)
        {
            // Создаем новый словарь
            var dictionary = new Dictionary<object, string>
                {
                    { ActorGender.Female, "Женщина" },
                    { ActorGender.Male, "Мужчина" },
                    { ActorGender.Unknown, "Неизвестно" }
                };

            if (dictionary.ContainsKey(input))
            {
                // Если ключ найден, возвращаем соответствующее значение
                return dictionary[input];
            }

            else
            {
                return input.ToString();
            }
        }

        public static string GendersLocalizationEn(this object input)
        {
            return input.ToString();
        }

        public static string QualityLocalizationRu(this object input)
        {
            // Создаем новый словарь
            var dictionary = new Dictionary<object, string>
                {
                    { ItemQuality.Normal, "Нормальное" },
                    { ItemQuality.Epic, "Эпическое" },
                    { ItemQuality.Rare, "Редкое" },
                    { ItemQuality.Legendary, "Легендарное" }
                };

            if (dictionary.ContainsKey(input))
            {
                // Если ключ найден, возвращаем соответствующее значение
                return dictionary[input];
            }

            else
            {
                return input.ToString();
            }
        }

        public static string QualityLocalizationEn(this object input)
        {
            return input.ToString();
        }
        #endregion

        #region GetActorData
        public static ActorData GetActorData(this Actor actor) => (ActorData)Reflection.GetField(actor.GetType(), actor, "data");

        public static ActorData GetActorData(this ActorBase actor) => (ActorData)Reflection.GetField(actor.GetType(), actor, "data");
        #endregion

        #region GetBaseStats
        public static BaseStats GetBaseStats(this Actor actor) => (BaseStats)Reflection.GetField(actor.GetType(), actor, "stats");

        public static BaseStats GetBaseStats(this ActorBase actor) => (BaseStats)Reflection.GetField(actor.GetType(), actor, "stats");

        public static BaseStats GetBaseStats(this BaseSimObject actor) => (BaseStats)Reflection.GetField(actor.GetType(), actor, "stats");
        #endregion


        #region SetIsTracked
        public static void SetIsTracked(this BaseSystemData data, bool is_tracked)
        {
            data.set("tracked", is_tracked);
        }

        public static void SetIsTracked(this BaseObjectData data, bool is_tracked)
        {
            data.set("tracked", is_tracked);
        }

        public static void SetIsTracked(this ActorData data, bool is_tracked)
        {
            data.set("tracked", is_tracked);
        }

        public static void SetIsTracked(this BaseSimObject actor, bool is_tracked)
        {
            actor.base_data.set("tracked", is_tracked);
        }

        public static void SetIsTracked(this Actor actor, bool is_tracked)
        {
            actor.base_data.set("tracked", is_tracked);
        }

        public static void SetIsTracked(this ActorBase actor, bool is_tracked)
        {
            actor.base_data.set("tracked", is_tracked);
        }
        #endregion

        #region GetIsTracked
        public static bool GetIsTracked(this BaseSystemData data)
        {
            data.get("tracked", out bool result);
            return result;
        }

        public static bool GetIsTracked(this BaseObjectData data)
        {
            data.get("tracked", out bool result);
            return result;
        }

        public static bool GetIsTracked(this ActorData data)
        {
            data.get("tracked", out bool result);
            return result;
        }

        public static bool GetIsTracked(this Actor actor)
        {
            actor.base_data.get("tracked", out bool result);
            return result;
        }

        public static bool GetIsTracked(this BaseSimObject actor)
        {
            actor.base_data.get("tracked", out bool result);
            return result;
        }

        public static bool GetIsTracked(this ActorBase actor)
        {
            actor.base_data.get("tracked", out bool result);
            return result;
        }
        #endregion
    }
}
