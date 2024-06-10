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

            string unit_statistic = $"{"Изначальные характеристики юнита".GetLocal()}:";
            unit_statistic += $"\r{"Имя".GetLocal()} - {logger.initial_name}";
            unit_statistic += !(logger.initial_traits.Count == 0) ? $"\r{"traits".GetLocal()} - {string.Join(", ", logger.initial_traits.Select(t => ("trait_" + t).GetLocal()))}" : "";
            unit_statistic += $"\r{"Профессия".GetLocal()} - {("profession_" + logger.initial_profession.ToString()).GetLocal()}" + (logger.initial_is_group_leader ? $", {"Генерал".GetLocal()}" : "");
            unit_statistic += !logger.initial_mood.IsNullOrWhiteSpace() ? $"\r{"creature_statistics_mood".GetLocal()} - {("mood_" + logger.initial_mood).GetLocal()}" : "";
            unit_statistic += !(logger.initial_kills == 0) ? $"\r{"creature_statistics_kills".GetLocal()} - {logger.initial_kills}" : "";
            unit_statistic += !(logger.initial_children == 0) ? $"\r{"creature_statistics_children".GetLocal()} - {logger.initial_children}" : "";

            unit_statistic += $"\r\rИзначальные характеристики:\r";
            foreach (var stat in logger.initial_characteristics.getList())
            {
                BaseStatAsset asset = stat.getAsset();
                if (asset.tooltip_multiply_for_visual_number != 1f)
                {
                    stat.value *= asset.tooltip_multiply_for_visual_number;
                }

                string text;
                if (stat.value != (float)((int)stat.value))
                {
                    text = stat.value.ToString("0.0");
                }
                else
                {
                    text = stat.value.ToString();
                }
                if (asset.show_as_percents)
                {
                    text += "%";
                }

                unit_statistic += $"{stat.id.GetLocal()} - {text}\r";
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
                else if (stat.Item3 == DataType.Food)
                {
                    unit_statistic += $"{stat.Item1.GetDateFromTime()} - юнит съел {stat.Item2.GetLocal()}\r";
                }
                else if (stat.Item3 == DataType.Culturships)
                {
                    unit_statistic += $"{stat.Item1.GetDateFromTime()} - юнит сменил культуру на {stat.Item2}\r";
                }
                else if (stat.Item3 == DataType.ReceivedTraits)
                {
                    unit_statistic += $"{stat.Item1.GetDateFromTime()} - юнит получил черту {("trait_" + stat.Item2).GetLocal()}\r";
                }
                else if (stat.Item3 == DataType.LostTraits)
                {
                    unit_statistic += $"{stat.Item1.GetDateFromTime()} - юнит потерял черту {("trait_" + stat.Item2).GetLocal()}\r";
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
                    unit_statistic += $"{stat.Item1.GetDateFromTime()} - юнит сменил профессию на {("profession_" + stat.Item2).GetLocal()}\r";
                }
                else if (stat.Item3 == DataType.Moods)
                {
                    unit_statistic += $"{stat.Item1.GetDateFromTime()} - юнит сменил настроение на {("mood_" + stat.Item2).GetLocal()}\r";
                }
                else if (stat.Item3 == DataType.KilledUnits)
                {
                    unit_statistic += $"{stat.Item1.GetDateFromTime()} - юнит убил {stat.Item2}\r";
                }
                else if (stat.Item3 == DataType.SocialCharacteristics)
                {
                    unit_statistic += $"{stat.Item1.GetDateFromTime()} - юнит увеличил характеристику {stat.Item2.GetLocal()} на 1\r";
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

            string unit_file_path = Path.Combine(unit_folder_path, "Сводка о смерти юнита".GetLocal() + ".txt");

            string unit_statistic = $"{"Сводка о смерти юнита".GetLocal()}:";
            unit_statistic += $"\rID - {actor_logged.id}";
            unit_statistic += $"\r{"Имя".GetLocal()} - {actor_logged.name}";
            unit_statistic += $"\rГоды жизни - с {actor_logged.born_in} по {actor_logged.dead_in}";
            unit_statistic += !(actor_logged.traits.Count == 0) ? $"\r{"traits".GetLocal()} - {string.Join(", ", actor_logged.traits.Select(t => t))}" : "";
            unit_statistic += $"\r{"Профессия".GetLocal()} - {actor_logged.profession}" + (actor.is_group_leader ? $", {"Генерал".GetLocal()}" : "");
            unit_statistic += $"\r{"Место смерти".GetLocal()} - X: {actor_logged.place_of_death.Item1}, Y: {actor_logged.place_of_death.Item2}";
            unit_statistic += !(actor_logged.resources.Count == 0) ? $"\r{"resources".GetLocal()} - {string.Join(", ", actor_logged.resources.Select(r => $"{r.Key}: {r.Value}"))}" : "";
            unit_statistic += !actor_logged.favorite_food.IsNullOrWhiteSpace() ? $"\r{"creature_statistics_favorite_food".GetLocal()} - {actor_logged.favorite_food}" : "";
            unit_statistic += !actor_logged.mood.IsNullOrWhiteSpace() ? $"\r {"creature_statistics_mood".GetLocal()} - {actor_logged.mood}" : "";
            unit_statistic += $"\r{"Пол".GetLocal()} - {actor_logged.gender}";
            unit_statistic += !(actor_logged.kills == 0) ? $"\r{"creature_statistics_kills".GetLocal()} - {actor_logged.kills}" : "";
            unit_statistic += $"\r{"Биологический вид".GetLocal()} - {actor_logged.species}";
            unit_statistic += actor.hasClan() ? $"\r {"influence".GetLocal()} - {actor_logged.influence}" : "";
            unit_statistic += actor.asset.needFood ? $"\r{"hunger".GetLocal()} - {actor_logged.hunger}%" : "";
            unit_statistic += !(actor_logged.level == 0) ? $"\r{"creature_statistics_character_level".GetLocal()} - {actor_logged.level}" : "";
            unit_statistic += !(actor_logged.experience == 0) ? $"\r{"creature_statistics_character_experience".GetLocal()} - {actor_logged.experience}" : "";
            unit_statistic += !actor_logged.personality.IsNullOrWhiteSpace() ? $"\r{"creature_statistics_personality".GetLocal()} - {actor_logged.personality}" : "";
            unit_statistic += !(actor_logged.children == 0) ? $"\r{"creature_statistics_children".GetLocal()} - {actor_logged.children}" : "";

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

                if (actor_logged.kingdom != null) { civil_affiliation.Add("kingdom".GetLocal(), actor_logged.kingdom.name); }
                if (actor_logged.city != null) { civil_affiliation.Add("village".GetLocal(), actor_logged.city.name); }
                if (actor_logged.clan != null) { civil_affiliation.Add("clan".GetLocal(), actor_logged.clan.name); }
                if (actor_logged.culture != null) { civil_affiliation.Add("culture".GetLocal(), actor_logged.culture.name); }


                unit_statistic += !(civil_affiliation.Count == 0) ? $"\r\r{"Гражданская принадлежность".GetLocal()}: {string.Join(", ", civil_affiliation.Select(c => $"{c.Key} - {c.Value}"))}" : "";

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

            unit_statistic += !(actor.GetBaseStats().getList().Count == 0) ? $"\r\r{"Характеристики".GetLocal()}:\r" : "";

            foreach (var stat in actor.GetBaseStats().getList())
            {
                BaseStatAsset asset = stat.getAsset();
                if (asset.tooltip_multiply_for_visual_number != 1f)
                {
                    stat.value *= asset.tooltip_multiply_for_visual_number;
                }

                string text;
                if (stat.value != (float)((int)stat.value))
                {
                    text = stat.value.ToString("0.0");
                }
                else
                {
                    text = stat.value.ToString();
                }
                if (asset.show_as_percents)
                {
                    text += "%";
                }

                unit_statistic += $"{stat.id.GetLocal()}: {text}\r";
            }

            if (actor_logged.items.Count != 0)
            {
                unit_statistic += $"\r\r{"inventory".GetLocal()}:";

                foreach (var item in actor_logged.items)
                {
                    var item_asset = AssetManager.items.get(item.id);
                    unit_statistic += $"\r";
                    unit_statistic += $"\r{"Тип".GetLocal()} - {("item_" + item.id).GetLocal()}";
                    unit_statistic += $"\r{"item_material".GetLocal()} - {("item_mat_" + item.material).GetLocal()}";
                    unit_statistic += !item.name.IsNullOrWhiteSpace() ? $"\r{"Имя".GetLocal()} - {item.name}" : "";
                    unit_statistic += !(item.kills == 0) ? $"\r{"creature_statistics_kills".GetLocal()} - {item.kills}" : "";
                    unit_statistic += $"\r{"Год создания".GetLocal()} - {item.year}";
                    unit_statistic += !item.by.IsNullOrWhiteSpace() ? $"\r{"Создатель".GetLocal()} - {item.by}" : "";
                    unit_statistic += !item.from.IsNullOrWhiteSpace() ? $"\r{"Создан в государстве".GetLocal()} - {item.from}" : "";

                    if (item.modifiers.Count != 0)
                    {
                        List<(string, char)> items_modifiers = new List<(string, char)>();

                        foreach (var item_modifier in item.modifiers)
                        {
                            (string, char) modifier = item_modifier.DecodeModifier();
                            items_modifiers.Add(modifier);
                        }

                        unit_statistic += items_modifiers.Count != 0 ? $"\r{"Модификаторы".GetLocal()}: {string.Join(", ", items_modifiers.Select(m => (m.Item2 != ' ') ? $"{("mod_" + m.Item1).GetLocal()} {m.Item2}" : $"{("mod_" + m.Item1).GetLocal()}"))}" : "";
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
