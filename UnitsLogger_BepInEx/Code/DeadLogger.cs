using BepInEx;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace UnitsLogger_BepInEx
{
    public partial class DeadLogger
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

        public static void SavingAll(Actor actor, string folder_path)
        {
            string unit_folder_path = Path.Combine(folder_path, actor.data.id);
            if (!Directory.Exists(unit_folder_path))
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(unit_folder_path);
                directoryInfo.Create();
            }

            actor.prepareForSave();
            SavingInitial(actor, unit_folder_path);
            SavingDead(actor, unit_folder_path);
            SavingLife(actor, unit_folder_path);
        }

        public static void SavingInitial(Actor actor, string unit_folder_path)
        {
            LifeLogger logger = actor.GetLogger();
            UnitAvatarSaver saver = new UnitAvatarSaver();

            string unit_file_path = Path.Combine(unit_folder_path, "Сводка о начале пути юнита" + ".txt");

            StringBuilder unit_statistic = new StringBuilder();
            unit_statistic.Append($"{"Изначальные характеристики юнита".GetLocal()}:");
            unit_statistic.Append($"\rДата, когда юнит стал отслеживаемым - {logger.initial_time.GetDateFromTime()}");
            unit_statistic.Append($"\r{"Имя".GetLocal()} - {logger.initial_name}");
            if (logger.initial_traits.Count != 0) unit_statistic.Append($"\r{"traits".GetLocal()} - {string.Join(", ", logger.initial_traits.Select(t => ("trait_" + t).GetLocal()))}");
            unit_statistic.Append($"\r{"Профессия".GetLocal()} - {logger.initial_profession.ToString().GetLocal()}" + (logger.initial_is_group_leader ? $", {"Генерал".GetLocal()}" : ""));
            if (!logger.initial_mood.IsNullOrWhiteSpace()) unit_statistic.Append($"\r{"creature_statistics_mood".GetLocal()} - {("mood_" + logger.initial_mood).GetLocal()}");
            if (logger.initial_kills != 0) unit_statistic.Append($"\r{"creature_statistics_kills".GetLocal()} - {logger.initial_kills}");
            if (logger.initial_children != 0) unit_statistic.Append($"\r{"creature_statistics_children".GetLocal()} - {logger.initial_children}");
            unit_statistic.Append($"\r{"opinion_world_era".GetLocal()} - {(logger.initial_era + "_title").GetLocal()}");

            if (logger.initial_items.Count != 0)
            {
                unit_statistic.Append($"\r\r{"inventory".GetLocal()}:");

                foreach (var item in logger.initial_items)
                {
                    var item_asset = AssetManager.items.get(item.id);
                    unit_statistic.Append($"\r");
                    unit_statistic.Append($"\r{"Тип".GetLocal()} - {("item_" + item.id).GetLocal()}");
                    unit_statistic.Append($"\r{"item_material".GetLocal()} - {("item_mat_" + item.material).GetLocal()}");
                    if (!item.name.IsNullOrWhiteSpace()) unit_statistic.Append($"\r{"Имя".GetLocal()} - {item.name}");
                    if (item.kills != 0) unit_statistic.Append($"\r{"creature_statistics_kills".GetLocal()} - {item.kills}");
                    unit_statistic.Append($"\r{"Год создания".GetLocal()} - {item.year}");
                    if (!item.by.IsNullOrWhiteSpace()) unit_statistic.Append($"\r{"Создатель".GetLocal()} - {item.by}");
                    if (!item.from.IsNullOrWhiteSpace()) unit_statistic.Append($"\r{"Создан в государстве".GetLocal()} - {item.from}");

                    if (item.modifiers.Count != 0)
                    {
                        List<(string, string)> items_modifiers = new List<(string, string)>();

                        foreach (var item_modifier in item.modifiers)
                        {
                            (string, string) modifier = item_modifier.DecodeModifier();
                            items_modifiers.Add(modifier);
                        }

                        if (items_modifiers.Count != 0) unit_statistic.Append($"\r{"Модификаторы".GetLocal()}: {string.Join(", ", items_modifiers.Select(m => (m.Item2 != "") ? $"{("mod_" + m.Item1).GetLocal()} {m.Item2}" : $"{("mod_" + m.Item1).GetLocal()}"))}");
                    }
                }
            }

            unit_statistic.Append($"\r\rИзначальные характеристики:\r");
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

                unit_statistic.Append($"{stat.id.GetLocal()} - {text}\r");
            }

            using (StreamWriter writer = new StreamWriter(unit_file_path))
            {
                writer.WriteLine(unit_statistic.ToString());
            }

            string avatar_path = Path.Combine(unit_folder_path, "Image Life.png");
            if (logger.initial_texture != null)
            {
                saver.SaveAvatarImage(logger.initial_texture, avatar_path);
            }

            logger = null;
            saver = null;
        }

        public static void SavingLife(Actor actor, string unit_folder_path)
        {
            LifeLogger logger = actor.GetLogger();

            string unit_file_path = Path.Combine(unit_folder_path, "Сводка о жизни юнита" + ".txt");

            StringBuilder unit_statistic = new StringBuilder();
            unit_statistic.Append($"История жизни:\r");

            List<(double, (int, int), string, DataType)> sortedList = logger.main_dict; // Ранее это вызывало бесконечную рекурсию, из-за которой игра завершалась
            sortedList = sortedList.OrderBy(item => item.Item1).ToList();

            foreach (var stat in sortedList)
            {
                unit_statistic.Append($"Дата: {stat.Item1.GetDateFromTime()}, место: X{stat.Item2.Item1}, Y{stat.Item2.Item2} - ");

                switch (stat.Item4)
                {
                    case DataType.Names:
                        unit_statistic.Append($"юнит сменил имя на {stat.Item3}");
                        break;

                    case DataType.ReplenishHunger:
                        unit_statistic.Append($"юнит полностью пополнил сытость");
                        break;

                    case DataType.CastSpell:
                        unit_statistic.Append($"юнит кастанул заклинание {stat.Item3}");
                        break;

                    case DataType.CrabBurrow:
                        unit_statistic.Append($"юнит {stat.Item3}");
                        break;

                    case DataType.Children:
                        unit_statistic.Append($"юнит родил ребёнка {stat.Item3}");
                        break;

                    case DataType.CitizenJobStart:
                        unit_statistic.Append($"юнит начал работать как {stat.Item3.GetLocal()}");
                        break;

                    case DataType.FoundedCities:
                        unit_statistic.Append($"юнит основал город под названием {stat.Item3}");
                        break;

                    case DataType.PlantCrops:
                        unit_statistic.Append($"юнит посадил пшеницу");
                        break;

                    case DataType.ManufacturedItem:
                        unit_statistic.Append($"юнит произвёл предмет: {stat.Item3.GetLocal()}");
                        break;

                    case DataType.ExtractResources:
                        unit_statistic.Append($"юнит добыл ресурсы из {stat.Item3.GetLocal()}");
                        break;

                    case DataType.MineResources:
                        unit_statistic.Append($"юнит добыл в шахте минерал {stat.Item3.GetLocal()}");
                        break;

                    case DataType.CreateRoad:
                        unit_statistic.Append($"юнит построил дорогу на месте тайла со следующими характеристиками: {stat.Item3}");
                        break;

                    case DataType.MakeFarm:
                        unit_statistic.Append($"юнит вспахал поле на месте тайла со следующими характеристиками: {stat.Item3}");
                        break;

                    case DataType.BuildedConstruction:
                        unit_statistic.Append($"юнит построил {stat.Item3.GetLocal()}");
                        break;

                    case DataType.KilledUnits:
                        unit_statistic.Append($"юнит убил {stat.Item3}");
                        break;

                    case DataType.CleanedConstruction:
                        unit_statistic.Append($"юнит убрал руины {stat.Item3.GetLocal()}");
                        break;

                    case DataType.GetResources:
                        unit_statistic.Append($"юнит добыл {stat.Item3}");
                        break;

                    case DataType.GiveResources:
                        unit_statistic.Append($"юнит положил в хранилище города: {stat.Item3}");
                        break;

                    case DataType.CitizenJobEnd:
                        unit_statistic.Append($"юнит закончил свою работу как {stat.Item3.GetLocal()}");
                        break;

                    case DataType.Food:
                        unit_statistic.Append($"юнит съел {stat.Item3.GetLocal()}");
                        break;

                    case DataType.EatenBuildings:
                        unit_statistic.Append($"юнит съел {stat.Item3.GetLocal()}");
                        break;

                    case DataType.Culturships:
                        unit_statistic.Append($"юнит сменил культуру на {stat.Item3}");
                        break;

                    case DataType.ReceivedTraits:
                        unit_statistic.Append($"юнит получил черту {("trait_" + stat.Item3).GetLocal()}");
                        break;

                    case DataType.LostTraits:
                        unit_statistic.Append($"юнит потерял черту {("trait_" + stat.Item3).GetLocal()}");
                        break;

                    case DataType.Townships:
                        unit_statistic.Append($"юнит сменил город на {stat.Item3}");
                        break;

                    case DataType.Сitizenships:
                        unit_statistic.Append($"юнит сменил государство на {stat.Item3}");
                        break;

                    case DataType.Professions:
                        unit_statistic.Append($"юнит сменил профессию на {stat.Item3.GetLocal()}");
                        break;

                    case DataType.Moods:
                        unit_statistic.Append($"юнит сменил настроение на {("mood_" + stat.Item3).GetLocal()}");
                        break;

                    case DataType.SocialCharacteristics:
                        unit_statistic.Append($"юнит увеличил характеристику {stat.Item3.GetLocal()} на 1");
                        break;

                    case DataType.NewEra:
                        unit_statistic.Append($"мировая эпоха сменилась на {(stat.Item3 + "_title").GetLocal()}");
                        break;
                }
                unit_statistic.Append($"\r\r");
            }

            unit_statistic.Append($"{World.world.getCurWorldTime().GetDateFromTime()} - юнит умер\r");


            using (StreamWriter writer = new StreamWriter(unit_file_path))
            {
                writer.WriteLine(unit_statistic.ToString());
            }

            logger = null;
        }

        public static void SavingDead(Actor actor, string unit_folder_path)
        {
            ActorLogged actor_logged = new ActorLogged(actor);
            UnitAvatarSaver saver = new UnitAvatarSaver();

            string avatar_path = Path.Combine(unit_folder_path, "Image Dead.png");
            saver.SaveAvatarImage(actor, avatar_path);

            string unit_file_path = Path.Combine(unit_folder_path, "Сводка о смерти юнита".GetLocal() + ".txt");

            StringBuilder unit_statistic = new StringBuilder();

            unit_statistic.Append($"{"Сводка о смерти юнита".GetLocal()}:");
            unit_statistic.Append($"\rID - {actor_logged.id}");
            unit_statistic.Append($"\r{"Имя".GetLocal()} - {actor_logged.name}");
            unit_statistic.Append($"\rГоды жизни - с {actor_logged.born_in} по {actor_logged.dead_in}");
            unit_statistic.Append($"\r{"world_current_age_title".GetLocal()} - {(World.world_era.id + "_title").GetLocal()}");
            if (actor_logged.traits.Count != 0) unit_statistic.Append($"\r{"traits".GetLocal()} - {string.Join(", ", actor_logged.traits.Select(t => t))}");
            unit_statistic.Append($"\r{"Профессия".GetLocal()} - {actor_logged.profession}" + (actor.is_group_leader ? $", {"Генерал".GetLocal()}" : ""));
            unit_statistic.Append($"\r{"Место смерти".GetLocal()} - X: {actor_logged.place_of_death.Item1}, Y: {actor_logged.place_of_death.Item2}");
            if (actor_logged.resources.Count != 0) unit_statistic.Append($"\r{"resources".GetLocal()} - {string.Join(", ", actor_logged.resources.Select(r => $"{r.Key}: {r.Value}"))}");
            if (!actor_logged.favorite_food.IsNullOrWhiteSpace()) unit_statistic.Append($"\r{"creature_statistics_favorite_food".GetLocal()} - {actor_logged.favorite_food}");
            if (!actor_logged.mood.IsNullOrWhiteSpace()) unit_statistic.Append($"\r {"creature_statistics_mood".GetLocal()} - {actor_logged.mood}");
            unit_statistic.Append($"\r{"Пол".GetLocal()} - {actor_logged.gender}");
            if (actor_logged.kills != 0) unit_statistic.Append($"\r{"creature_statistics_kills".GetLocal()} - {actor_logged.kills}");
            unit_statistic.Append($"\r{"Биологический вид".GetLocal()} - {actor_logged.species}");
            if (actor.hasClan()) unit_statistic.Append($"\r {"influence".GetLocal()} - {actor_logged.influence}");
            if (actor.asset.needFood) unit_statistic.Append($"\r{"hunger".GetLocal()} - {actor_logged.hunger}%");
            if (actor_logged.level != 0) unit_statistic.Append($"\r{"creature_statistics_character_level".GetLocal()} - {actor_logged.level}");
            if (actor_logged.experience != 0) unit_statistic.Append($"\r{"creature_statistics_character_experience".GetLocal()} - {actor_logged.experience}");
            if (!actor_logged.personality.IsNullOrWhiteSpace()) unit_statistic.Append($"\r{"creature_statistics_personality".GetLocal()} - {actor_logged.personality}");
            if (actor_logged.children != 0) unit_statistic.Append($"\r{"creature_statistics_children".GetLocal()} - {actor_logged.children}");


            unit_statistic.Append($"\r\r{"Подробные сведения о месте смерти".GetLocal()}:\r");
            if (actor.currentTile.zone.city != null) unit_statistic.Append($"{"Город, на территории которого умер юнит".GetLocal()} - {actor.currentTile.zone.city.getCityName()}\r");
            if (actor.currentTile.zone.culture != null) unit_statistic.Append($"{"Культура, на территории которой умер юнит".GetLocal()} - {actor.currentTile.zone.culture.name}\r");
            if (actor.currentTile.zone.city?.kingdom != null) unit_statistic.Append($"{"Государство, на территории которого умер юнит".GetLocal()} - {actor.currentTile.zone.city.kingdom.name}\r");
            if (actor.currentTile.zone.city?.getRoyalClan() != null) unit_statistic.Append($"{"Клан, на территории которого умер юнит".GetLocal()} - {actor.currentTile.zone.city.getRoyalClan().name}\r");
            unit_statistic.Append($"{"Зона, где умер юнит".GetLocal()} - X: {actor.currentTile.zone.x}, Y: {actor.currentTile.zone.y}\r");
            if (actor.currentTile.zone.abandoned?.Count != 0) unit_statistic.Append($"{"Заброшенные строения на зоне".GetLocal()} - {string.Join(", ", actor.currentTile.zone.abandoned.Select(b => ("building_" + b.asset.id).GetLocal()))}\r");
            if (actor.currentTile.zone.food?.Count != 0) unit_statistic.Append($"{"Съедобные строения на зоне".GetLocal()} - {string.Join(", ", actor.currentTile.zone.food?.Select(b => ("building_" + b.asset.id).GetLocal()))}\r");
            if (actor.currentTile.zone.minerals?.Count != 0) unit_statistic.Append($"{"Минералы на зоне".GetLocal()} - {string.Join(", ", actor.currentTile.zone.minerals?.Select(b => ("building_" + b.asset.id).GetLocal()))}\r");
            if (actor.currentTile.zone.plants?.Count != 0) unit_statistic.Append($"{"Растения на зоне".GetLocal()} - {string.Join(", ", actor.currentTile.zone.plants?.Select(b => ("building_" + b.asset.id).GetLocal()))}\r");
            if (actor.currentTile.zone.trees?.Count != 0) unit_statistic.Append($"{"Деревья на зоне".GetLocal()} - {string.Join(", ", actor.currentTile.zone.trees?.Select(b => ("building_" + b.asset.id).GetLocal()))}\r");
            if (actor.currentTile.zone.wheat?.Count != 0) unit_statistic.Append($"{"Пшеница на зоне".GetLocal()} - {string.Join(", ", actor.currentTile.zone.wheat?.Select(b => ("building_" + b.asset.id).GetLocal()))}\r");
            if (actor.currentTile.zone.ruins?.Count != 0) unit_statistic.Append($"{"Руины на зоне".GetLocal()} - {string.Join(", ", actor.currentTile.zone.ruins?.Select(b => ("building_" + b.asset.id).GetLocal()))}\r");
            if (actor.currentTile.zone.buildings?.Count != 0) unit_statistic.Append($"{"Цивилизационные постройки на зоне".GetLocal()} - {string.Join(", ", actor.currentTile.zone.buildings?.Select(b => ("building_" + b.asset.id).GetLocal()))}\r");
            if (actor.currentTile.zone.buildings_all?.Count != 0) unit_statistic.Append($"{"Все строения на зоне".GetLocal()} - {string.Join(", ", actor.currentTile.zone.buildings_all?.Select(b => ("building_" + b.asset.id).GetLocal()))}\r");


            //unit_statistic += !(logger.received_names.Count == 0) ? $"\r\r{"Случаи смены имён"}: {string.Join(", ", logger.received_names.Select(n => $"{n.Value} - {n.Key.GetDateFromTime()}"))}" : "";
            //unit_statistic += !(logger.received_traits.Count == 0) ? $"\r{"Случаи получения черт"}: {string.Join(", ", logger.received_traits.Select(n => $"{("trait_" + n.Item2).GetLocalization()} - {n.Item1.GetDateFromTime()}"))}" : "";
            //unit_statistic += !(logger.lost_traits.Count == 0) ? $"\r{"Случаи потери черт"}: {string.Join(", ", logger.lost_traits.Select(n => $"{("trait_" + n.Item2).GetLocalization()} - {n.Item1.GetDateFromTime()}"))}" : "";
            //unit_statistic += !(logger.received_professions.Count == 0) ? $"\r{"Случаи смены профессии"}: {string.Join(", ", logger.received_professions.Select(n => $"{("profession_" + n.Item2.ToString()).GetLocalization()} - {n.Item1.GetDateFromTime()}"))}" : "";
            //unit_statistic += !(logger.received_citizenships.Count == 0) ? $"\r{"Случаи смены гражданства"}: {string.Join(", ", logger.received_citizenships.Select(n => $"{n.Item2} - {n.Item1.GetDateFromTime()}"))}" : "";
            //unit_statistic += !(logger.received_townships.Count == 0) ? $"\r{"Случаи смены места жительства"}: {string.Join(", ", logger.received_townships.Select(n => $"{n.Item2} - {n.Item1.GetDateFromTime()}"))}" : "";
            //unit_statistic += !(logger.received_culturships.Count == 0) ? $"\r{"Случаи смены культурной принадлежности"}: {string.Join(", ", logger.received_culturships.Select(n => $"{n.Item2} - {n.Item1.GetDateFromTime()}"))}" : "";

            if (actor_logged.kingdom != null || actor_logged.city != null || actor_logged.clan != null || actor_logged.culture != null)
            {
                List<(string, string)> civil_affiliation = new List<(string, string)>();

                if (actor_logged.kingdom != null) civil_affiliation.Add(("kingdom".GetLocal(), actor_logged.kingdom.name));
                if (actor_logged.city != null) civil_affiliation.Add(("village".GetLocal(), actor_logged.city.name));
                if (actor_logged.clan != null) civil_affiliation.Add(("clan".GetLocal(), actor_logged.clan.name));
                if (actor_logged.culture != null) civil_affiliation.Add(("culture".GetLocal(), actor_logged.culture.name));


                if (civil_affiliation.Count != 0) unit_statistic.Append($"\r\r{"Гражданская принадлежность".GetLocal()}: {string.Join(", ", civil_affiliation.Select(c => $"{c.Item1} - {c.Item2}"))}");

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

            if (actor.stats.getList().Count != 0) unit_statistic.Append($"\r\r{"Характеристики".GetLocal()}:\r");

            foreach (var stat in actor.stats.getList())
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

                unit_statistic.Append($"{stat.id.GetLocal()}: {text}\r");
            }

            if (actor_logged.items.Count != 0)
            {
                unit_statistic.Append($"\r\r{"inventory".GetLocal()}:");

                foreach (var item in actor_logged.items)
                {
                    var item_asset = AssetManager.items.get(item.id);
                    unit_statistic.Append($"\r");
                    unit_statistic.Append($"\r{"Тип".GetLocal()} - {("item_" + item.id).GetLocal()}");
                    unit_statistic.Append($"\r{"item_material".GetLocal()} - {("item_mat_" + item.material).GetLocal()}");
                    if (!item.name.IsNullOrWhiteSpace()) unit_statistic.Append($"\r{"Имя".GetLocal()} - {item.name}");
                    if (item.kills != 0) unit_statistic.Append($"\r{"creature_statistics_kills".GetLocal()} - {item.kills}");
                    unit_statistic.Append($"\r{"Год создания".GetLocal()} - {item.year}");
                    if (!item.by.IsNullOrWhiteSpace()) unit_statistic.Append($"\r{"Создатель".GetLocal()} - {item.by}");
                    if (!item.from.IsNullOrWhiteSpace()) unit_statistic.Append($"\r{"Создан в государстве".GetLocal()} - {item.from}");

                    if (item.modifiers.Count != 0)
                    {
                        List<(string, string)> items_modifiers = new List<(string, string)>();

                        foreach (var item_modifier in item.modifiers)
                        {
                            (string, string) modifier = item_modifier.DecodeModifier();
                            items_modifiers.Add(modifier);
                        }

                        if (items_modifiers.Count != 0) unit_statistic.Append($"\r{"Модификаторы".GetLocal()}: {string.Join(", ", items_modifiers.Select(m => (m.Item2 != "") ? $"{("mod_" + m.Item1).GetLocal()} {m.Item2}" : $"{("mod_" + m.Item1).GetLocal()}"))}");
                    }
                }
            }

            using (StreamWriter writer = new StreamWriter(unit_file_path))
            {
                writer.WriteLine(unit_statistic.ToString());
            }

            actor_logged.ClearAll();
            actor_logged = null;
            saver = null;
        }
    }
}
