using BepInEx;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace UnitsLogger_BepInEx
{
    public class DeadLogger
    {
        public static string Prepare()
        {
            string main_folder_path = Path.Combine(Application.streamingAssetsPath, "Saved Units");
            if (!Directory.Exists(main_folder_path))
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(main_folder_path);
                directoryInfo.Create();
            }

            string folder_path = Path.Combine(main_folder_path, World.world.mapStats.name);
            if (!Directory.Exists(folder_path))
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(folder_path);
                directoryInfo.Create();
            }

            return folder_path;
        }

        public static void SavingLife(LifeLogger logger, UnitAvatarSaver saver, string unit_folder_path)
        {
            string unit_file_path = Path.Combine(unit_folder_path, "Сводка о жизни юнита" + ".txt");

            string unit_statistic = $"{"Изначальные характеристики юнита".GetLocalization()}:";
            unit_statistic += $"\r{"Имя".GetLocalization()} - {logger.initial_name}";
            unit_statistic += !(logger.initial_traits.Count == 0) ? $"\r{"traits".GetLocalization()} - {string.Join(", ", logger.initial_traits.Select(t => ("trait_" + t).GetLocalization()))}" : "";
            unit_statistic += $"\r{"Профессия".GetLocalization()} - {logger.initial_profession.ToString().GetLocalization()}" + (logger.initial_is_group_leader ? $", {"Генерал".GetLocalization()}" : "");
            unit_statistic += !logger.initial_mood.IsNullOrWhiteSpace() ? $"\r{"creature_statistics_mood".GetLocalization()} - {("mood_" + logger.initial_mood).GetLocalization()}" : "";
            unit_statistic += !(logger.initial_kills == 0) ? $"\r{"creature_statistics_kills".GetLocalization()} - {logger.initial_kills}" : "";
            unit_statistic += !(logger.initial_children == 0) ? $"\r{"creature_statistics_children".GetLocalization()} - {logger.initial_children}" : "";

            unit_statistic += $"\r\rИзначальные характеристики:\r";
            foreach (var stat in logger.initial_characteristics)
            {
                switch (stat.Key)
                {
                    case "armor":
                        unit_statistic += $"{stat.Key.GetLocalization()} - {stat.Value}%\r";
                        break;
                    case "critical_chance":
                        unit_statistic += $"{stat.Key.GetLocalization()} - {stat.Value * 100f}%\r";
                        break;
                    default:
                        unit_statistic += $"{stat.Key.GetLocalization()} - {stat.Value}\r";
                        break;
                }
            }

            unit_statistic += $"\r\rИстория жизни:\r";

            List<(double, string, DataType)> sortedList = logger.main_dict; // Ранее это вызывало бесконечную рекурсию, из-за которой игра завершалась
            sortedList = sortedList.OrderBy(item => item.Item1).ToList();


            foreach (var stat in sortedList)
            {
                if (stat.Item3 == DataType.Names)
                {
                    unit_statistic += $"{stat.Item1.GetDateFromTime()} - юнит сменил имя на {stat.Item2}\r";
                }
                else if (stat.Item3 == DataType.Culturships)
                {
                    unit_statistic += $"{stat.Item1.GetDateFromTime()} - юнит сменил культуру на {stat.Item2}\r";
                }
                else if (stat.Item3 == DataType.ReceivedTraits)
                {
                    unit_statistic += $"{stat.Item1.GetDateFromTime()} - юнит получил черту {("trait_" + stat.Item2).GetLocalization()}\r";
                }
                else if (stat.Item3 == DataType.LostTraits)
                {
                    unit_statistic += $"{stat.Item1.GetDateFromTime()} - юнит потерял черту {("trait_" + stat.Item2).GetLocalization()}\r";
                }
                else if (stat.Item3 == DataType.Townships)
                {
                    unit_statistic += $"{stat.Item1.GetDateFromTime()} - юнит сменил город на {stat.Item2}\r";
                }
                else if (stat.Item3 == DataType.Сitizenships)
                {
                    unit_statistic += $"{stat.Item1.GetDateFromTime()} - юнит сменил государство на {stat.Item2}\r";
                }
                else if (stat.Item3 == DataType.Professions)
                {
                    unit_statistic += $"{stat.Item1.GetDateFromTime()} - юнит сменил профессию на {("profession_" + stat.Item2).GetLocalization()}\r";
                }
                else if (stat.Item3 == DataType.Food)
                {
                    unit_statistic += $"{stat.Item1.GetDateFromTime()} - юнит съел {stat.Item2.GetLocalization()}\r";
                }
                else if (stat.Item3 == DataType.Moods)
                {
                    unit_statistic += $"{stat.Item1.GetDateFromTime()} - юнит сменил настроение на {("mood_" + stat.Item2).GetLocalization()}\r";
                }
                else if (stat.Item3 == DataType.KilledUnits)
                {
                    unit_statistic += $"{stat.Item1.GetDateFromTime()} - юнит убил {stat.Item2}\r";
                }
                else if (stat.Item3 == DataType.SocialCharacteristics)
                {
                    unit_statistic += $"{stat.Item1.GetDateFromTime()} - юнит увеличил характеристику {stat.Item2.GetLocalization()} на 1\r";
                }
            }

            unit_statistic += $"{(World.world.getCurWorldTime()).GetDateFromTime()} - юнит умер\r";


            using (StreamWriter writer = new StreamWriter(unit_file_path))
            {
                writer.WriteLine(unit_statistic);
            }

            string avatar_path = Path.Combine(unit_folder_path, "Image Life.png");
            saver.SaveAvatarImage(logger.initial_texture, avatar_path);
        }

        public static void SavingDead(Actor actor, string folder_path)
        {
            actor.prepareForSave();
            ActorLogged actor_logged = new ActorLogged(actor);
            UnitAvatarSaver saver = new UnitAvatarSaver();
            LifeLogger logger = actor.gameObject.GetComponent<LifeLogger>();

            string unit_folder_path = Path.Combine(folder_path, actor_logged.id);
            if (!Directory.Exists(unit_folder_path))
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(unit_folder_path);
                directoryInfo.Create();
            }
            SavingLife(logger, saver, unit_folder_path);

            string avatar_path = Path.Combine(unit_folder_path, "Image Dead.png");
            saver.SaveAvatarImage(actor, avatar_path);

            string unit_file_path = Path.Combine(unit_folder_path, "Сводка о смерти юнита".GetLocalization() + ".txt");

            string unit_statistic = $"{"Сводка о смерти юнита".GetLocalization()}:";
            unit_statistic += $"\rID - {actor_logged.id}";
            unit_statistic += $"\r{"Имя".GetLocalization()} - {actor_logged.name}";
            unit_statistic += $"\rГоды жизни - с {actor_logged.born_in} по {actor_logged.dead_in}";
            unit_statistic += !(actor_logged.traits.Count == 0) ? $"\r{"traits".GetLocalization()} - {string.Join(", ", actor_logged.traits.Select(t => t))}" : "";
            unit_statistic += $"\r{"Профессия".GetLocalization()} - {actor_logged.profession}" + (actor.is_group_leader ? $", {"Генерал".GetLocalization()}" : "");
            unit_statistic += $"\r{"Место смерти".GetLocalization()} - X: {actor_logged.place_of_death.Item1}, Y: {actor_logged.place_of_death.Item2}";
            unit_statistic += !(actor_logged.resources.Count == 0) ? $"\r{"resources".GetLocalization()} - {string.Join(", ", actor_logged.resources.Select(r => $"{r.Key}: {r.Value}"))}" : "";
            unit_statistic += !actor_logged.favorite_food.IsNullOrWhiteSpace() ? $"\r{"creature_statistics_favorite_food".GetLocalization()} - {actor_logged.favorite_food}" : "";
            unit_statistic += !actor_logged.mood.IsNullOrWhiteSpace() ? $"\r {"creature_statistics_mood".GetLocalization()} - {actor_logged.mood}" : "";
            unit_statistic += $"\r{"Пол".GetLocalization()} - {actor_logged.gender}";
            unit_statistic += !(actor_logged.kills == 0) ? $"\r{"creature_statistics_kills".GetLocalization()} - {actor_logged.kills}" : "";
            unit_statistic += $"\r{"Биологический вид".GetLocalization()} - {actor_logged.species}";
            unit_statistic += actor.hasClan() ? $"\r {"influence".GetLocalization()} - {actor_logged.influence}" : "";
            unit_statistic += actor.asset.needFood ? $"\r{"hunger".GetLocalization()} - {actor_logged.hunger}%" : "";
            unit_statistic += !(actor_logged.level == 0) ? $"\r{"creature_statistics_character_level".GetLocalization()} - {actor_logged.level}" : "";
            unit_statistic += !(actor_logged.experience == 0) ? $"\r{"creature_statistics_character_experience".GetLocalization()} - {actor_logged.experience}" : "";
            unit_statistic += !actor_logged.personality.IsNullOrWhiteSpace() ? $"\r{"creature_statistics_personality".GetLocalization()} - {actor_logged.personality}" : "";
            unit_statistic += !(actor_logged.children == 0) ? $"\r{"creature_statistics_children".GetLocalization()} - {actor_logged.children}" : "";

            //unit_statistic += !(logger.received_names.Count == 0) ? $"\r\r{"Случаи смены имён"}: {string.Join(", ", logger.received_names.Select(n => $"{n.Value} - {n.Key.GetDateFromTime()}"))}" : "";
            //unit_statistic += !(logger.received_traits.Count == 0) ? $"\r{"Случаи получения черт"}: {string.Join(", ", logger.received_traits.Select(n => $"{("trait_" + n.Item2).GetLocalization()} - {n.Item1.GetDateFromTime()}"))}" : "";
            //unit_statistic += !(logger.lost_traits.Count == 0) ? $"\r{"Случаи потери черт"}: {string.Join(", ", logger.lost_traits.Select(n => $"{("trait_" + n.Item2).GetLocalization()} - {n.Item1.GetDateFromTime()}"))}" : "";
            //unit_statistic += !(logger.received_professions.Count == 0) ? $"\r{"Случаи смены профессии"}: {string.Join(", ", logger.received_professions.Select(n => $"{("profession_" + n.Item2.ToString()).GetLocalization()} - {n.Item1.GetDateFromTime()}"))}" : "";
            //unit_statistic += !(logger.received_citizenships.Count == 0) ? $"\r{"Случаи смены гражданства"}: {string.Join(", ", logger.received_citizenships.Select(n => $"{n.Item2} - {n.Item1.GetDateFromTime()}"))}" : "";
            //unit_statistic += !(logger.received_townships.Count == 0) ? $"\r{"Случаи смены места жительства"}: {string.Join(", ", logger.received_townships.Select(n => $"{n.Item2} - {n.Item1.GetDateFromTime()}"))}" : "";
            //unit_statistic += !(logger.received_culturships.Count == 0) ? $"\r{"Случаи смены культурной принадлежности"}: {string.Join(", ", logger.received_culturships.Select(n => $"{n.Item2} - {n.Item1.GetDateFromTime()}"))}" : "";

            if (actor_logged.kingdom != null || actor_logged.city != null || actor_logged.clan != null || actor_logged.culture != null)
            {
                Dictionary<string, string> civil_affiliation = new Dictionary<string, string>();

                if (actor_logged.kingdom != null) { civil_affiliation.Add("kingdom".GetLocalization(), actor_logged.kingdom.name); }
                if (actor_logged.city != null) { civil_affiliation.Add("village".GetLocalization(), actor_logged.city.name); }
                if (actor_logged.clan != null) { civil_affiliation.Add("clan".GetLocalization(), actor_logged.clan.name); }
                if (actor_logged.culture != null) { civil_affiliation.Add("culture".GetLocalization(), actor_logged.culture.name); }


                unit_statistic += !(civil_affiliation.Count == 0) ? $"\r\r{"Гражданская принадлежность".GetLocalization()}: {string.Join(", ", civil_affiliation.Select(c => $"{c.Key} - {c.Value}"))}" : "";

                //unit_statistic += $"\r\rГражданская принадлежность:";
                //unit_statistic += !(actor_logged.kingdom == null) ? $"\rКоролевство - {actor_logged.kingdom.name}" : "";
                //unit_statistic += !(actor_logged.city == null) ? $"\rГород - {actor_logged.city.name}" : "";
                //unit_statistic += !(actor_logged.clan == null) ? $"\rКлан - {actor_logged.clan.name}" : "";
                //unit_statistic += !(actor_logged.culture == null) ? $"\rКультура - {actor_logged.culture.name}" : "";
            }

            /*if (actor.asset.unit)
            {
                Dictionary<string, int> political_characteristics = new Dictionary<string, int>();

                if (actor_logged.diplomacy != 0) { political_characteristics.Add("diplomacy".GetLocalization(), actor_logged.diplomacy); }
                if (actor_logged.intelligence != 0) { political_characteristics.Add("intelligence".GetLocalization(), actor_logged.intelligence); }
                if (actor_logged.stewardship != 0) { political_characteristics.Add("stewardship".GetLocalization(), actor_logged.stewardship); }
                if (actor_logged.warfare != 0) { political_characteristics.Add("warfare".GetLocalization(), actor_logged.warfare); }

                unit_statistic += !(political_characteristics.Count == 0) ? $"\r\r{"Политические характеристики".GetLocalization()}: {string.Join(", ", political_characteristics.Select(c => $"{c.Key} {c.Value}"))}" : "";
                //unit_statistic += $"\r\rПолитические характеристики: {(!(actor_logged.diplomacy == 0) ? $"{"diplomacy".GetLocalization()} {actor_logged.diplomacy}," : "")} {(!(actor_logged.intelligence == 0) ? $"{"intelligence".GetLocalization()} {actor_logged.intelligence}," : "")} {(!(actor_logged.stewardship == 0) ? $"{"stewardship".GetLocalization()} {actor_logged.stewardship}," : "")} {(!(actor_logged.warfare == 0) ? $"{"warfare".GetLocalization()} {actor_logged.warfare}" : "")}";
            }*/

            unit_statistic += !(actor.GetBaseStats().getList().Count == 0) ? $"\r\r{"Характеристики".GetLocalization()}:\r" : "";

            foreach (var stat in actor.GetBaseStats().getList())
            {
                switch (stat.id)
                {
                    case "armor":
                        unit_statistic += $"{stat.id.GetLocalization()} - {stat.value}%\r";
                        break;
                    case "critical_chance":
                        unit_statistic += $"{stat.id.GetLocalization()} - {stat.value * 100f}%\r";
                        break;
                    default:
                        unit_statistic += $"{stat.id.GetLocalization()} - {stat.value}\r";
                        break;
                }
            }

            if (actor_logged.items.Count != 0)
            {
                unit_statistic += $"\r\r{"inventory".GetLocalization()}:";

                foreach (var item in actor_logged.items)
                {
                    var item_asset = AssetManager.items.get(item.id);
                    unit_statistic += $"\r";
                    unit_statistic += $"\r{"Тип".GetLocalization()} - {("item_" + item.id).GetLocalization()}";
                    unit_statistic += $"\r{"item_material".GetLocalization()} - {("item_mat_" + item.material).GetLocalization()}";
                    unit_statistic += !item.name.IsNullOrWhiteSpace() ? $"\r{"Имя".GetLocalization()} - {item.name}" : "";
                    unit_statistic += !(item.kills == 0) ? $"\r{"creature_statistics_kills".GetLocalization()} - {item.kills}" : "";
                    unit_statistic += $"\r{"Год создания".GetLocalization()} - {item.year}";
                    unit_statistic += !item.by.IsNullOrWhiteSpace() ? $"\r{"Создатель".GetLocalization()} - {item.by}" : "";
                    unit_statistic += !item.from.IsNullOrWhiteSpace() ? $"\r{"Создан в государстве".GetLocalization()} - {item.from}" : "";

                    if (item.modifiers.Count != 0)
                    {
                        List<(string, char)> items_modifiers = new List<(string, char)>();

                        foreach (var item_modifier in item.modifiers)
                        {
                            (string, char) modifier = item_modifier.DecodeModifier();
                            items_modifiers.Add(modifier);
                        }

                        unit_statistic += items_modifiers.Count != 0 ? $"\r{"Модификаторы".GetLocalization()}: {string.Join(", ", items_modifiers.Select(m => (m.Item2 != ' ') ? $"{("mod_" + m.Item1).GetLocalization()} {m.Item2}" : $"{("mod_" + m.Item1).GetLocalization()}"))}" : "";
                    }
                }
            }

            using (StreamWriter writer = new StreamWriter(unit_file_path))
            {
                writer.WriteLine(unit_statistic);
            }

            actor_logged.ClearAll();
            actor_logged = null;
            saver = null;
            logger = null;
        }
    }
}
