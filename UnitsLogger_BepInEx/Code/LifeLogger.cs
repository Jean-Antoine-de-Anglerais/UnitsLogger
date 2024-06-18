using BepInEx;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnitsLogger_BepInEx
{
    public class LifeLogger : MonoBehaviour
    {
        // Случаи, когда юнит получал или менял имя
        public Dictionary<double, (string, (int, int))> received_names = new Dictionary<double, (string, (int, int))>();
        //Все черты, которые юнит получил за свою жизнь (значения - ID черты и точное время получения черты)
        public List<(double, (int, int), string, DataType)> received_traits = new List<(double, (int, int), string, DataType)>();
        //Все черты, которые юнит потерял за свою жизнь
        public List<(double, (int, int), string, DataType)> lost_traits = new List<(double, (int, int), string, DataType)>();
        // Существо, убившее юнита (если есть)
        public Actor killer_actor = new Actor();
        // Здание, убившее юнита (если есть)
        public Building killer_building = new Building();
        //Все предметы, которые юнит получил за свою жизнь
        public List<(double, (int, int), ItemData, DataType)> received_items = new List<(double, (int, int), ItemData, DataType)>();
        //Все предметы, которые юнит потерял за свою жизнь
        public List<(double, (int, int), ItemData, DataType)> lost_items = new List<(double, (int, int), ItemData, DataType)>();
        // Дети, которых юнит родил
        public List<(double, (int, int), string, ActorGender, DataType)> born_children = new List<(double, (int, int), string, ActorGender, DataType)>();
        // Дети, которых юнит родил с партнёром (время, имя ребёнка, пол ребёнка, имя партнёра, пол партнёра)
        public List<(double, (int, int), string, ActorGender, string, ActorGender, DataType)> born_children_with_partner = new List<(double, (int, int), string, ActorGender, string, ActorGender, DataType)>();
        // Случаи, когда юнит получал или менял профессию
        public List<(double, (int, int), UnitProfession, DataType)> received_professions = new List<(double, (int, int), UnitProfession, DataType)>();
        // Случаи, когда юнит получал или менял гражданство
        public List<(double, (int, int), string, DataType)> received_citizenships = new List<(double, (int, int), string, DataType)>();
        // Случаи, когда юнит получал или менял принадлежность к городу
        public List<(double, (int, int), string, DataType)> received_townships = new List<(double, (int, int), string, DataType)>();
        // Случаи, когда юнит получал или менял культуру
        public List<(double, (int, int), string, DataType)> received_culturships = new List<(double, (int, int), string, DataType)>();
        // Случаи, когда юнит получал или менял клан
        // ПОКА НЕ НУЖНО, ТАК КАК ЮНИТ НЕ МОЖЕТ МЕНЯТЬ ПРИНАДЛЕЖНОСТЬ К КЛАНУ
        public List<(double, string)> received_clanships = new List<(double, string)>();
        // Случаи, когда юнит получал или менял настроение
        public List<(double, (int, int), string, DataType)> received_moods = new List<(double, (int, int), string, DataType)>();
        // Случаи, когда юнит что-то ел
        public List<(double, (int, int), string, DataType)> eaten_food = new List<(double, (int, int), string, DataType)>();
        // Случаи производства предметов
        public List<(double, (int, int), ItemDataLogged, DataType)> manufactured_items = new List<(double, (int, int), ItemDataLogged, DataType)>();
        // Случаи убийства кого-либо
        public List<(double, (int, int), string, DataType)> killed_units = new List<(double, (int, int), string, DataType)>();
        // Случаи изменение социальных характеристик, не связанные с чертами и чем-то таким
        public List<(double, (int, int), string, DataType)> social_characteristics = new List<(double, (int, int), string, DataType)>();
        // Причина смерти
        public DeadReason dead_reason = DeadReason.Null;
        // Полученные ресурсы
        public List<(double, (int, int), string, int, DataType)> received_resources = new List<(double, (int, int), string, int, DataType)>();
        // Отданные ресурсы
        public List<(double, (int, int), string, DataType)> given_resources = new List<(double, (int, int), string, DataType)>();
        // Смена эпох
        public List<(double, (int, int), string, DataType)> changing_eras = new List<(double, (int, int), string, DataType)>();
        // Начала городской работы
        public List<(double, (int, int), string, DataType)> citizen_job_starts = new List<(double, (int, int), string, DataType)>();
        // Окончания городской работы
        public List<(double, (int, int), string, DataType)> citizen_job_ends = new List<(double, (int, int), string, DataType)>();
        // Построенные строения
        public List<(double, (int, int), string, DataType)> builded_construction = new List<(double, (int, int), string, DataType)>();
        // Очищенные руины
        public List<(double, (int, int), string, DataType)> cleaned_construction = new List<(double, (int, int), string, DataType)>();
        // Случаи добычи ресурсов из строений
        public List<(double, (int, int), string, DataType)> extract_resources = new List<(double, (int, int), string, DataType)>();
        // Построенные тайлы дороги
        public List<(double, (int, int), string, DataType)> create_road = new List<(double, (int, int), string, DataType)>();
        // Вспаханные тайлы поля
        public List<(double, (int, int), string, DataType)> make_farm = new List<(double, (int, int), string, DataType)>();
        // Основанные государства (дата, координаты юнита, было ли основано королевство, название города, название королевства, координаты чанка королевства
        public List<(double, (int, int), bool, string, string, (int, int), DataType)> founded_cities = new List<(double, (int, int), bool, string, string, (int, int), DataType)>();
        // Съеденные животным постройки
        public List<(double, (int, int), string, DataType)> eaten_buildings = new List<(double, (int, int), string, DataType)>();
        // Добытые в шахте ресурсы
        public List<(double, (int, int), string, DataType)> mine_resources = new List<(double, (int, int), string, DataType)>();
        // Полное пополнение голода (для крабов и существ, кормящихся на воде)
        public List<(double, (int, int), string, DataType)> replenish_hunger = new List<(double, (int, int), string, DataType)>();
        // Случаи каста заклинания спавна скелетов (дата, координаты юнита, координаты заспавненного скелета, тип данных)
        public List<(double, (int, int), (int, int), DataType)> maked_skeletons = new List<(double, (int, int), (int, int), DataType)>();
        // Случаи, когда краб зарывается в землю
        public List<(double, (int, int), string, float, DataType)> crab_burrow = new List<(double, (int, int), string, float, DataType)>();


        // Имя, бывшее у юнита изначально
        public string initial_name = "";
        // Черты, бывшие у юнита изначально 
        public List<string> initial_traits = new List<string>();
        // Предметы, бывшие у юнита изначально
        public List<ItemDataLogged> initial_items = new List<ItemDataLogged>();
        // Изначальное количество детей
        public int initial_children = 0;
        // Изначальная профессия
        public UnitProfession initial_profession = UnitProfession.Null;
        // Изначальное гражданство
        public string initial_citizenship = "";
        // Изначальный город
        public string initial_township = "";
        // Изначальная культура
        public string initial_culturship = "";
        // Изначальный клан
        // ПОКА НЕ НУЖНО, ТАК КАК ЮНИТ НЕ МОЖЕТ МЕНЯТЬ ПРИНАДЛЕЖНОСТЬ К КЛАНУ
        public string initial_clanship = "";
        // Изначальное настроение
        public string initial_mood = "";
        // Изначальное кол-во убийств
        public int initial_kills = 0;
        // Является ли лидером группы
        public bool initial_is_group_leader = false;
        // Изначальная текстура юнита
        public Texture2D initial_texture = new Texture2D(10, 10);
        // Изначальные характеристики юнита
        public BaseStats initial_characteristics = new BaseStats();
        // Изначальная эпоха
        public string initial_era = "";
        // Момент, когда юнита сделали отслеживаемым
        public double initial_time = 0;
        // Изначальное местоположение юнита
        public (int, int) initial_position = (0, 0);

        // Были ли исходные данные уже добавлены
        public bool was_initialized = false;


        // Список, который объединяет все остальные списки и словари в один, для дальнейшей сортировки
        public List<(double, (int, int), string, DataType)> main_dict
        {
            get
            {
                List<(double, (int, int), string, DataType)> temp_dict = new List<(double, (int, int), string, DataType)>();

                List<(double, (int, int), string, DataType)> received_name_list = new List<(double, (int, int), string, DataType)>();
                foreach (var name in received_names)
                {
                    received_name_list.Add((name.Key, (name.Value.Item2.Item1, name.Value.Item2.Item2), name.Value.Item1, DataType.Names));
                }

                List<(double, (int, int), string, DataType)> received_professions_list = new List<(double, (int, int), string, DataType)>();
                foreach (var profession in received_professions)
                {
                    received_professions_list.Add((profession.Item1, (profession.Item2.Item1, profession.Item2.Item2), profession.Item3.ToString().GetLocal(), profession.Item4));
                }

                List<(double, (int, int), string, DataType)> received_resources_list = new List<(double, (int, int), string, DataType)>();
                foreach (var resource in received_resources)
                {
                    received_resources_list.Add((resource.Item1, (resource.Item2.Item1, resource.Item2.Item2), resource.Item4 + " " + resource.Item3.GetLocal(), resource.Item5));
                }

                List<(double, (int, int), string, DataType)> born_children_with_partner_list = new List<(double, (int, int), string, DataType)>();
                foreach (var child in born_children_with_partner)
                {
                    born_children_with_partner_list.Add((child.Item1, (child.Item2.Item1, child.Item2.Item2), $"по имени {child.Item3}, имеющего пол {child.Item4.ToString().GetLocal()}, вступив в отношения с юнитом по имени {child.Item5}, имеющим пол {child.Item6.ToString().GetLocal()}", child.Item7));
                }

                List<(double, (int, int), string, DataType)> born_children_list = new List<(double, (int, int), string, DataType)>();
                foreach (var child in born_children)
                {
                    born_children_list.Add((child.Item1, (child.Item2.Item1, child.Item2.Item2), $"по имени {child.Item3}, имеющего пол {child.Item4.ToString().GetLocal()}", child.Item5));
                }

                List<(double, (int, int), string, DataType)> manufactured_items_list = new List<(double, (int, int), string, DataType)>();
                foreach (var item in manufactured_items)
                {
                    manufactured_items_list.Add((item.Item1, (item.Item2.Item1, item.Item2.Item2), (!item.Item3.name.IsNullOrWhiteSpace() ? $"название {item.Item3.name}, " : "") + $"тип {("item_" + item.Item3.id).GetLocal()}, материал {("item_mat_" + item.Item3.material).GetLocal()}" + (item.Item3.modifiers.Count != 0 ? $", модификаторы {string.Join(", ", item.Item3.modifiers.Select(m => (m.DecodeModifier().Item2 != ' ') ? $"{("mod_" + m.DecodeModifier().Item1).GetLocal()} {m.DecodeModifier().Item2}" : $"{("mod_" + m.DecodeModifier().Item1).GetLocal()}"))}" : ""), item.Item4));
                }

                List<(double, (int, int), string, DataType)> founded_cities_list = new List<(double, (int, int), string, DataType)>();
                foreach (var city in founded_cities)
                {
                    if (city.Item3)
                    {
                        founded_cities_list.Add((city.Item1, (city.Item2.Item1, city.Item2.Item2), city.Item4 + $" и королевство под названием {city.Item5}, на чанке с координатами X{city.Item6.Item1}, Y{city.Item6.Item2}", city.Item7));
                    }
                    else
                    {
                        founded_cities_list.Add((city.Item1, (city.Item2.Item1, city.Item2.Item2), city.Item4 + $", на чанке с координатами X{city.Item6.Item1}, Y{city.Item6.Item2}", city.Item7));
                    }
                }
                
                List<(double, (int, int), string, DataType)> maked_skeletons_list = new List<(double, (int, int), string, DataType)>();
                foreach (var maked in maked_skeletons)
                {
                    maked_skeletons_list.Add((maked.Item1, maked.Item2, $"X{maked.Item3.Item1}, Y{maked.Item3.Item2}", maked.Item4));
                }

                List<(double, (int, int), string, DataType)> crab_burrow_list = new List<(double, (int, int), string, DataType)>();
                foreach (var burrow in crab_burrow)
                {
                    if (burrow.Item3 == "type_repeat")
                    {
                        crab_burrow_list.Add((burrow.Item1, burrow.Item2, $"зарылся в землю ещё на {burrow.Item4}", burrow.Item5));
                    }
                    else if (burrow.Item3 == "type_hunger")
                    {
                        crab_burrow_list.Add((burrow.Item1, burrow.Item2, $"выкопался из земли, потому что проголодался", burrow.Item5));
                    }
                    else if (burrow.Item3 == "type_danger")
                    {
                        crab_burrow_list.Add((burrow.Item1, burrow.Item2, $"выкопался из земли, потому что почувствовал себя в опасности", burrow.Item5));
                    }
                }

                temp_dict.AddRange(received_traits);
                temp_dict.AddRange(received_name_list);
                temp_dict.AddRange(received_professions_list);
                temp_dict.AddRange(received_resources_list);
                temp_dict.AddRange(lost_traits);
                temp_dict.AddRange(born_children_list);
                temp_dict.AddRange(received_citizenships);
                temp_dict.AddRange(received_townships);
                temp_dict.AddRange(received_culturships);
                temp_dict.AddRange(received_moods);
                temp_dict.AddRange(eaten_food);
                temp_dict.AddRange(killed_units);
                temp_dict.AddRange(social_characteristics);
                temp_dict.AddRange(given_resources);
                temp_dict.AddRange(changing_eras);
                temp_dict.AddRange(citizen_job_starts);
                temp_dict.AddRange(citizen_job_ends);
                temp_dict.AddRange(builded_construction);
                temp_dict.AddRange(cleaned_construction);
                temp_dict.AddRange(extract_resources);
                temp_dict.AddRange(create_road);
                temp_dict.AddRange(born_children_with_partner_list);
                temp_dict.AddRange(make_farm);
                temp_dict.AddRange(manufactured_items_list);
                temp_dict.AddRange(founded_cities_list);
                temp_dict.AddRange(eaten_buildings);
                temp_dict.AddRange(mine_resources);
                temp_dict.AddRange(replenish_hunger);
                temp_dict.AddRange(maked_skeletons_list);
                temp_dict.AddRange(crab_burrow_list);

                return temp_dict;
            }

            set
            {
                // Реализация setter, если нужно
            }
        }


        // Используется для инициализации
        /*void Start()
        {
            Actor actor = gameObject.GetComponent<Actor>();
            if (actor != null)
            {
                if (!was_initialized)
                {
                    initial_name = actor.getName();
                    initial_traits.AddRange(actor.data.traits);

                    if (actor.equipment != null)
                    {
                        List<ItemData> dataForSave = actor.equipment.getDataForSave();
                        if (dataForSave.Count > 0)
                        {
                            foreach (var data in dataForSave)
                            {
                                ItemDataLogged data_logged = new ItemDataLogged(data);
                                initial_items.Add(data_logged);
                            }
                        }
                    }

                    initial_children = actor.data.children;
                    initial_profession = actor.data.profession;
                    initial_citizenship = actor.kingdom?.data.name;
                    initial_township = actor.city?.data.name;
                    initial_culturship = actor.getCulture()?.data.name;
                    initial_mood = actor.data.mood;
                    initial_kills = actor.data.kills;
                    initial_is_group_leader = actor.is_group_leader;
                    initial_texture = UnitAvatarSaver.SaveInitialAvatar(actor);
                    was_initialized = true;
                    initial_characteristics.getList().AddRange(actor.stats.getList());
                    initial_era = World.world_era.id;
                    initial_time = World.world.getCurWorldTime();
                    initial_position = (actor.currentTile.pos.x, actor.currentTile.pos.y);
                }
            }
        }*/

        // Используется для инициализации
        void OnEnable()
        {
            Actor actor = gameObject.GetComponent<Actor>();
            if (actor != null)
            {
                if (!was_initialized)
                {
                    initial_name = actor.getName();
                    initial_traits.AddRange(actor.data.traits);

                    if (actor.equipment != null)
                    {
                        List<ItemData> dataForSave = actor.equipment.getDataForSave();
                        if (dataForSave.Count > 0)
                        {
                            foreach (var data in dataForSave)
                            {
                                ItemDataLogged data_logged = new ItemDataLogged(data);
                                initial_items.Add(data_logged);
                            }
                        }
                    }

                    initial_children = actor.data.children;
                    initial_profession = actor.data.profession;
                    initial_citizenship = actor.kingdom?.data.name;
                    initial_township = actor.city?.data.name;
                    initial_culturship = actor.getCulture()?.data.name;
                    initial_mood = actor.data.mood;
                    initial_kills = actor.data.kills;
                    initial_is_group_leader = actor.is_group_leader;
                    initial_texture = UnitAvatarSaver.SaveInitialAvatar(actor);
                    was_initialized = true;
                    initial_characteristics.getList().AddRange(actor.stats.getList());
                    initial_era = World.world_era.id;
                    initial_time = World.world.getCurWorldTime();
                    initial_position = (actor.currentTile.pos.x, actor.currentTile.pos.y);
                }
            }
        }

        // Удаляет все данные экземпляра класса, во избежание утечек памяти
        void OnDestroy()
        {
            // Освобождаем словарь
            if (received_names != null)
            {
                received_names.Clear();
                received_names = null;
            }

            // Освобождаем списки
            if (received_traits != null)
            {
                received_traits.Clear();
                received_traits = null;
            }
            if (lost_traits != null)
            {
                lost_traits.Clear();
                lost_traits = null;
            }
            if (received_items != null)
            {
                received_items.Clear();
                received_items = null;
            }
            if (lost_items != null)
            {
                lost_items.Clear();
                lost_items = null;
            }
            if (born_children != null)
            {
                born_children.Clear();
                born_children = null;
            }
            if (received_professions != null)
            {
                received_professions.Clear();
                received_professions = null;
            }
            if (received_citizenships != null)
            {
                received_citizenships.Clear();
                received_citizenships = null;
            }
            if (received_townships != null)
            {
                received_townships.Clear();
                received_townships = null;
            }
            if (received_culturships != null)
            {
                received_culturships.Clear();
                received_culturships = null;
            }
            if (received_clanships != null)
            {
                received_clanships.Clear();
                received_clanships = null;
            }
            if (received_moods != null)
            {
                received_moods.Clear();
                received_moods = null;
            }
            if (eaten_food != null)
            {
                eaten_food.Clear();
                eaten_food = null;
            }
            if (manufactured_items != null)
            {
                manufactured_items.Clear();
                manufactured_items = null;
            }
            if (killed_units != null)
            {
                killed_units.Clear();
                killed_units = null;
            }
            if (social_characteristics != null)
            {
                social_characteristics.Clear();
                social_characteristics = null;
            }
            if (received_resources != null)
            {
                received_resources.Clear();
                received_resources = null;
            }
            if (given_resources != null)
            {
                given_resources.Clear();
                given_resources = null;
            }
            if (changing_eras != null)
            {
                changing_eras.Clear();
                changing_eras = null;
            }
            if (citizen_job_starts != null)
            {
                citizen_job_starts.Clear();
                citizen_job_starts = null;
            }
            if (citizen_job_ends != null)
            {
                citizen_job_ends.Clear();
                citizen_job_ends = null;
            }

            // Удаляем объекты
            killer_actor = null;
            killer_building = null;

            // Обнуляем примитивные типы
            initial_name = null;
            initial_traits = null;
            initial_items = null;
            initial_children = 0;
            initial_profession = UnitProfession.Null;
            initial_citizenship = null;
            initial_township = null;
            initial_culturship = null;
            initial_clanship = null;
            initial_mood = null;
            initial_kills = 0;
            initial_is_group_leader = false;
            initial_texture = null;
            initial_characteristics = null;
            initial_era = null;
            initial_time = 0;
            was_initialized = false;

            // Обнуляем main_dict (если у него есть сеттер)
            main_dict = null;

            GC.Collect();
        }
    }
}
