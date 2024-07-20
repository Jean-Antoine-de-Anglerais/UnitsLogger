using System.Collections.Generic;

namespace UnitsLogger_BepInEx
{
    public static class StaticStuff
    {
        public static string GetDateFromTime(this double time) => World.world.mapStats.getDate(time);

        public static (string, string) DecodeModifier(this string modifier)
        {
            List<char> chars = new List<char>();

            List<char> numbers = new List<char>();

            (string, string) output = ("", "");

            foreach (var item in modifier)
            {
                if (!char.IsDigit(item))
                {
                    chars.Add(item);
                }

                else if (char.IsDigit(item))
                {
                    numbers.Add(item);
                }
            }

            output.Item1 = string.Concat(chars);
            output.Item2 = string.Concat(numbers);

            return output;
        }

        public static LifeLogger GetLogger(this Actor actor) => actor.gameObject.GetComponent<LifeLogger>();

        #region Localization
        //public static string ProfessionsLocalizationRu(this object input)
        //{
        //    // Создаем новый словарь
        //    var dictionary = new Dictionary<object, string>
        //        {
        //            { UnitProfession.Null, "Нулевая" },
        //            { UnitProfession.Baby, "Ребёнок" },
        //            { UnitProfession.Unit, "Юнит" },
        //            { UnitProfession.Warrior, "Воин" },
        //            { UnitProfession.King, "Король" },
        //            { UnitProfession.Leader, "Лидер Поселения" }
        //        };
        //
        //    if (dictionary.ContainsKey(input))
        //    {
        //        // Если ключ найден, возвращаем соответствующее значение
        //        return dictionary[input];
        //    }
        //
        //    else
        //    {
        //        return input.ToString();
        //    }
        //}
        //
        //public static string ProfessionsLocalizationEn(this object input)
        //{
        //    return input.ToString();
        //}

        //public static string GendersLocalizationRu(this object input)
        //{
        //    // Создаем новый словарь
        //    var dictionary = new Dictionary<object, string>
        //        {
        //            { ActorGender.Female, "Женщина" },
        //            { ActorGender.Male, "Мужчина" },
        //            { ActorGender.Unknown, "Неизвестно" }
        //        };
        //
        //    if (dictionary.ContainsKey(input))
        //    {
        //        // Если ключ найден, возвращаем соответствующее значение
        //        return dictionary[input];
        //    }
        //
        //    else
        //    {
        //        return input.ToString();
        //    }
        //}
        //
        //public static string GendersLocalizationEn(this object input)
        //{
        //    return input.ToString();
        //}

        //public static string QualityLocalizationRu(this object input)
        //{
        //    // Создаем новый словарь
        //    var dictionary = new Dictionary<object, string>
        //        {
        //            { ItemQuality.Normal, "Нормальное" },
        //            { ItemQuality.Epic, "Эпическое" },
        //            { ItemQuality.Rare, "Редкое" },
        //            { ItemQuality.Legendary, "Легендарное" }
        //        };
        //
        //    if (dictionary.ContainsKey(input))
        //    {
        //        // Если ключ найден, возвращаем соответствующее значение
        //        return dictionary[input];
        //    }
        //
        //    else
        //    {
        //        return input.ToString();
        //    }
        //}
        //
        //public static string QualityLocalizationEn(this object input)
        //{
        //    return input.ToString();
        //}

        //public static string JobsLocalizationRu(this CitizenJobAsset input)
        //{
        //    string input_id = input.id;
        //    // Создаем новый словарь
        //    var dictionary = new Dictionary<string, string>
        //        {
        //            { "fireman", "Пожарный" },
        //            { "builder", "Строитель" },
        //            { "gatherer_bushes", "Собиратель Ягод" },
        //            { "gatherer_herbs", "Собиратель Трав" },
        //            { "farmer", "Фермер" },
        //            { "hunter", "Охотник" },
        //            { "woodcutter", "Лесоруб" },
        //            { "miner", "Шахтёр" },
        //            { "miner_deposit", "Рудокоп" },
        //            { "blacksmith", "Кузнец" },
        //            { "road_builder", "Строитель Дорог" },
        //            { "cleaner", "Уборщик" },
        //            { "settler", "Колонист" },
        //            { "attacker", "Воин" }
        //        };
        //
        //    if (dictionary.ContainsKey(input_id))
        //    {
        //        // Если ключ найден, возвращаем соответствующее значение
        //        return dictionary[input_id];
        //    }
        //
        //    else
        //    {
        //        return input_id;
        //    }
        //}
        //
        //public static string JobsLocalizationEn(this CitizenJobAsset input)
        //{
        //    string input_id = input.id;
        //    if (input_id != "miner_deposit")
        //    {
        //        if (string.IsNullOrEmpty(input_id))
        //            return input_id;
        //
        //        char[] chars = input_id.ToCharArray();
        //        bool capitalizeNext = false;
        //
        //        // Переводим первую букву в верхний регистр
        //        chars[0] = char.ToUpper(chars[0]);
        //
        //        // Заменяем символы "_" на пробелы и переводим следующий символ в верхний регистр
        //        for (int i = 1; i < chars.Length; i++)
        //        {
        //            if (chars[i] == '_')
        //            {
        //                chars[i] = ' ';
        //                capitalizeNext = true;
        //            }
        //            else if (capitalizeNext)
        //            {
        //                chars[i] = char.ToUpper(chars[i]);
        //                capitalizeNext = false;
        //            }
        //        }
        //
        //        return new string(chars);
        //    }
        //
        //    else
        //    {
        //        return "Deposit Miner";
        //    }
        //}
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

        #region Определить положение юнита
        public static (int, int) GetActorPosition(this Actor actor) => (actor.currentTile.pos.x, actor.currentTile.pos.y);

        public static (int, int) GetActorPosition(this ActorBase actor) => (actor.currentTile.pos.x, actor.currentTile.pos.y);

        public static (int, int) GetActorPosition(this BaseSimObject actor) => (actor.currentTile.pos.x, actor.currentTile.pos.y);

        public static (int, int) GetActorPosition(this BaseMapObject actor) => (actor.currentTile.pos.x, actor.currentTile.pos.y);
        #endregion

        #region Всякая штука
        public static Clan checkGreatClan(Actor pParent1, Actor pParent2)
        {
            string text = string.Empty;
            if (string.IsNullOrEmpty(text))
            {
                if (pParent1.isKing())
                {
                    text = pParent1.data.clan;
                }
                else if (pParent2 != null && pParent2.isKing())
                {
                    text = pParent2.data.clan;
                }
            }
            if (string.IsNullOrEmpty(text))
            {
                if (pParent1.isCityLeader() && pParent2 != null && pParent2.isCityLeader())
                {
                    text = ((!Toolbox.randomBool()) ? pParent2.data.clan : pParent1.data.clan);
                }
                else if (pParent1 != null && pParent1.isCityLeader())
                {
                    text = pParent1.data.clan;
                }
                else if (pParent2 != null && pParent2.isCityLeader())
                {
                    text = pParent2.data.clan;
                }
            }
            Clan result = null;
            if (!string.IsNullOrEmpty(text))
            {
                result = MapBox.instance.clans.get(text);
            }
            return result;
        }

        public static Culture getBabyCulture(Actor pActor1, Actor pActor2)
        {
            string text = pActor1.data.culture;
            string text2 = text;
            if (pActor2 != null)
            {
                text2 = pActor2.data.culture;
            }
            if (string.IsNullOrEmpty(text))
            {
                text = pActor1.city?.data.culture;
            }
            if (string.IsNullOrEmpty(text2) && pActor2 != null)
            {
                text2 = pActor2.city?.data.culture;
            }
            Culture culture = pActor1.currentTile.zone.culture;
            if (culture != null && culture.data.race == pActor1.race.id && Toolbox.randomChance(culture.stats.culture_spread_convert_chance.value))
            {
                text = culture.data.id;
            }
            if (Toolbox.randomBool())
            {
                return MapBox.instance.cultures.get(text);
            }
            return MapBox.instance.cultures.get(text2);
        }

        public static void makeChild(this Actor actor, double pWorldTime, Actor partner, ActorData baby)
        {
            if (actor != null)
            {
                if (GetIsTracked(actor))
                {
                    LifeLogger logger = actor.gameObject.GetComponent<LifeLogger>();

                    if (partner != null)
                    {
                        logger?.born_children_with_partner.Add((pWorldTime, actor.GetActorPosition(), baby.getName(), baby.gender, partner.getName(), partner.data.gender, DataType.Children));
                    }
                    else
                    {
                        logger?.born_children.Add((pWorldTime, actor.GetActorPosition(), baby.getName(), baby.gender, DataType.Children));
                    }
                }
            }
        }

        public static void makeChild(this Actor actor, double pWorldTime, Actor partner, Actor baby)
        {
            if (actor != null)
            {
                if (GetIsTracked(actor))
                {
                    LifeLogger logger = actor.gameObject.GetComponent<LifeLogger>();

                    if (partner != null)
                    {
                        logger?.born_children_with_partner.Add((pWorldTime, actor.GetActorPosition(), baby.getName(), baby.data.gender, partner.getName(), partner.data.gender, DataType.Children));
                    }
                    else
                    {
                        logger?.born_children.Add((pWorldTime, actor.GetActorPosition(), baby.getName(), baby.data.gender, DataType.Children));
                    }
                }
            }
        }

        public static string getName(this ActorData data)
        {
            if (string.IsNullOrEmpty(data.name))
            {
                Actor actor = World.world.units.get(data.id);

                if (actor != null)
                {
                    data.generateName(actor.asset, actor.race);
                }

                else
                {
                    var asset = AssetManager.actor_library.get(data.asset_id);
                    data.generateName(asset);
                }
            }
            return data.name;
        }

        public static void generateName(this ActorData data, ActorAsset pAsset)
        {
            if (pAsset.canBeCitizen)
            {
                data.setName(NameGenerator.getName(pAsset.nameTemplate, data.gender));
            }
            else
            {
                data.setName(NameGenerator.getName(pAsset.nameTemplate));
            }
        }
        #endregion
    }
}
