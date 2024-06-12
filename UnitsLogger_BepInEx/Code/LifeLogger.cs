using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnitsLogger_BepInEx
{
    public class LifeLogger : MonoBehaviour
    {
        // Случаи, когда юнит получал или менял имя
        public Dictionary<double, string> received_names = new Dictionary<double, string>();
        //Все черты, которые юнит получил за свою жизнь (значения - ID черты и точное время получения черты)
        public List<(double, string, DataType)> received_traits = new List<(double, string, DataType)>();
        //Все черты, которые юнит потерял за свою жизнь
        public List<(double, string, DataType)> lost_traits = new List<(double, string, DataType)>();
        // Существо, убившее юнита (если есть)
        public Actor killer_actor = new Actor();
        // Здание, убившее юнита (если есть)
        public Building killer_building = new Building();
        //Все предметы, которые юнит получил за свою жизнь
        public List<(double, ItemData, DataType)> received_items = new List<(double, ItemData, DataType)>();
        //Все предметы, которые юнит потерял за свою жизнь
        public List<(double, ItemData, DataType)> lost_items = new List<(double, ItemData, DataType)>();
        // Дети, которых юнит родил
        public List<(double, string, DataType)> born_children = new List<(double, string, DataType)>();
        // Случаи, когда юнит получал или менял профессию
        public List<(double, UnitProfession, DataType)> received_professions = new List<(double, UnitProfession, DataType)>();
        // Случаи, когда юнит получал или менял гражданство
        public List<(double, string, DataType)> received_citizenships = new List<(double, string, DataType)>();
        // Случаи, когда юнит получал или менял принадлежность к городу
        public List<(double, string, DataType)> received_townships = new List<(double, string, DataType)>();
        // Случаи, когда юнит получал или менял культуру
        public List<(double, string, DataType)> received_culturships = new List<(double, string, DataType)>();
        // Случаи, когда юнит получал или менял клан
        // ПОКА НЕ НУЖНО, ТАК КАК ЮНИТ НЕ МОЖЕТ МЕНЯТЬ ПРИНАДЛЕЖНОСТЬ К КЛАНУ
        public List<(double, string)> received_clanships = new List<(double, string)>();
        // Случаи, когда юнит получал или менял настроение
        public List<(double, string, DataType)> received_moods = new List<(double, string, DataType)>();
        // Случаи, когда юнит что-то ел
        public List<(double, string, DataType)> eaten_food = new List<(double, string, DataType)>();
        // Случаи производства предметов
        public List<(double, ItemData)> manufactured_items = new List<(double, ItemData)>();
        // Случаи убийства кого-либо
        public List<(double, string, DataType)> killed_units = new List<(double, string, DataType)>();
        // Случаи изменение социальных характеристик, не связанные с чертами и чем-то таким
        public List<(double, string, DataType)> social_characteristics = new List<(double, string, DataType)>();
        // Причина смерти
        public DeadReason dead_reason = DeadReason.Null;
        // Полученные ресурсы
        public List<(double, string, int, DataType)> received_resources = new List<(double, string, int, DataType)>();
        // Отданные ресурсы
        public List<(double, string, DataType)> given_resources = new List<(double, string, DataType)>();
        // Смена эпох
        public List<(double, string, DataType)> changing_eras = new List<(double, string, DataType)>();
        // Начала городской работы
        public List<(double, string, DataType)> citizen_job_starts = new List<(double, string, DataType)>();
        // Окончания городской работы
        public List<(double, string, DataType)> citizen_job_ends = new List<(double, string, DataType)>();
        // Построенные строения
        public List<(double, string, DataType)> builded_construction = new List<(double, string, DataType)>();
        // Очищенные руины
        public List<(double, string, DataType)> cleaned_construction = new List<(double, string, DataType)>();

        // Имя, бывшее у юнита изначально
        public string initial_name = "";
        // Черты, бывшие у юнита изначально 
        public List<string> initial_traits = new List<string>();
        // Предметы, бывшие у юнита изначально
        public List<ItemData> initial_items = new List<ItemData>();
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

        // Были ли исходные данные уже добавлены
        public bool was_initialized = false;

        // Список, который объединяет все остальные списки и словари в один, для дальнейшей сортировки
        public List<(double, string, DataType)> main_dict
        {
            get
            {
                List<(double, string, DataType)> temp_dict = new List<(double, string, DataType)>();

                List<(double, string, DataType)> received_name_list = new List<(double, string, DataType)>();
                foreach (var name in received_names)
                {
                    received_name_list.Add((name.Key, name.Value, DataType.Names));
                }

                List<(double, string, DataType)> received_professions_list = new List<(double, string, DataType)>();
                foreach (var profession in received_professions)
                {
                    received_professions_list.Add((profession.Item1, (profession.Item2.ToString()).GetLocal(), profession.Item3));
                }

                List<(double, string, DataType)> received_resources_list = new List<(double, string, DataType)>();
                foreach (var resource in received_resources)
                {
                    received_resources_list.Add((resource.Item1, (resource.Item3 + " " + resource.Item2.GetLocal()), resource.Item4));
                }

                temp_dict.AddRange(received_traits);
                temp_dict.AddRange(received_name_list);
                temp_dict.AddRange(received_professions_list);
                temp_dict.AddRange(received_resources_list);
                temp_dict.AddRange(lost_traits);
                temp_dict.AddRange(born_children);
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

                return temp_dict;
            }

            set
            {
                // Реализация setter, если нужно
            }
        }


        // Используется для инициализации
        void Start()
        {
            Actor actor = gameObject.GetComponent<Actor>();
            if (actor != null)
            {
                if (!was_initialized)
                {
                    initial_name = actor.GetActorData().name;
                    initial_traits.AddRange(actor.GetActorData().traits);
                    if (actor.GetActorData().items != null)
                    {
                        initial_items.AddRange(actor.GetActorData().items);
                    }
                    initial_children = actor.GetActorData().children;
                    initial_profession = actor.GetActorData().profession;
                    initial_citizenship = actor.kingdom?.data.name;
                    initial_township = actor.city?.data.name;
                    initial_culturship = actor.getCulture()?.data.name;
                    initial_mood = actor.GetActorData().mood;
                    initial_kills = actor.GetActorData().kills;
                    initial_is_group_leader = actor.is_group_leader;
                    initial_texture = UnitAvatarSaver.SaveInitialAvatar(actor);
                    was_initialized = true;
                    initial_characteristics.getList().AddRange(actor.GetBaseStats().getList());
                    initial_era = World.world_era.id;
                    initial_time = World.world.getCurWorldTime();
                }
            }
        }

        // Используется для инициализации
        void OnEnable()
        {
            Actor actor = gameObject.GetComponent<Actor>();
            if (actor != null)
            {
                if (!was_initialized)
                {
                    initial_name = actor.GetActorData().name;
                    initial_traits.AddRange(actor.GetActorData().traits);
                    if (actor.GetActorData().items != null)
                    {
                        initial_items.AddRange(actor.GetActorData().items);
                    }
                    initial_children = actor.GetActorData().children;
                    initial_profession = actor.GetActorData().profession;
                    initial_citizenship = actor.kingdom?.data.name;
                    initial_township = actor.city?.data.name;
                    initial_culturship = actor.getCulture()?.data.name;
                    initial_mood = actor.GetActorData().mood;
                    initial_kills = actor.GetActorData().kills;
                    initial_is_group_leader = actor.is_group_leader;
                    initial_texture = UnitAvatarSaver.SaveInitialAvatar(actor);
                    was_initialized = true;
                    initial_characteristics.getList().AddRange(actor.GetBaseStats().getList());
                    initial_era = World.world_era.id;
                    initial_time = World.world.getCurWorldTime();
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
