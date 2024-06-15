using System.Collections.Generic;

namespace UnitsLogger_BepInEx
{
    public class ActorLogged
    {
        /* Уникальный ID
         * Имя
         * Годы жизни
         * Причина смерти
         * Черты при смерти
         * Профессия при смерти
         * Место смерти
         * Предметы при смерти
         * Ресурсы при смерти
         * Любимая еда
         * Настроение
         * Вид юнита
         * Пол
         * Культура
         * Клан
         * Количество убийств
         * Количество детей
         * Сытость
         * Уровень
         * Опыт
         * Все характеристики
         * Причина смерти
         */

        public void ClearAll()
        {
            id = string.Empty;
            name = string.Empty;
            born_in = string.Empty;
            dead_in = string.Empty;
            traits = new List<string>();
            profession = string.Empty;
            place_of_death = (0, 0);
            resources = new Dictionary<string, int>();
            favorite_food = string.Empty;
            mood = string.Empty;
            gender = string.Empty;
            kills = 0;
            items = new List<ItemData>();
            species = string.Empty;
            kingdom = new Kingdom();
            city = new City();
            culture = new Culture();
            clan = new Clan();
            influence = 0;
            level = 0;
            experience = 0;
            hunger = 0;
        }

        public ActorLogged(Actor actor)
        {
            id = actor.base_data.id;

            ActorData data = actor.data;
            if (data.items != null)
            {
                items = data.items;
            }

            if (actor.inventory != null)
            {
                foreach (var resourc in actor.inventory.getResources())
                {
                    resources.Add(resourc.Value.id.GetLocal(), resourc.Value.amount);
                }
            }
            profession = data.profession.ToString().GetLocal();

            name = actor.getName();
            born_in = actor.base_data.created_time.GetDateFromTime();
            dead_in = (World.world.getCurWorldTime()).GetDateFromTime();
            favorite_food = data.favoriteFood.GetLocal();
            mood = ("mood_" + data.mood).GetLocal();

            influence = actor.getInfluence();
            hunger = (int)((float)data.hunger / (float)actor.asset.maxHunger * 100f);
            level = data.level;
            experience = data.experience;

            kingdom = actor.kingdom;
            city = actor.city;
            culture = actor.getCulture();
            clan = actor.getClan();

            foreach (var trait in data.traits)
            {
                traits.Add(("trait_" + trait).GetLocal());
            }

            place_of_death = (data.x, data.y);

            gender = data.gender.ToString().GetLocal();
            kills = data.kills;

            species = actor.asset.id.GetLocal();

            if (actor.s_personality != null)
            {
                personality = ("personality_" + actor.s_personality.id).GetLocal();
            }

            children = data.children;

            diplomacy = data.diplomacy;
            intelligence = data.intelligence;
            stewardship = data.stewardship;
            warfare = data.warfare;
        }

        // Уникальный ID
        public string id = "";

        // Имя
        public string name = "";

        // Дата рождения
        public string born_in = "";

        // Дата смерти
        public string dead_in = "";

        // Черты при смерти
        public List<string> traits = new List<string>();

        // Профессия при смерти
        public string profession = "";

        // Место смерти
        public (int, int) place_of_death = (0, 0);

        // Ресурсы
        public Dictionary<string, int> resources = new Dictionary<string, int>();

        // Любимая еда
        public string favorite_food = "";

        // Настроение
        public string mood = "";

        // Пол
        public string gender = "";

        // Убийства
        public int kills = 0;

        // Предметы
        public List<ItemData> items = new List<ItemData>();

        // Биологический вид
        public string species = "";

        // Королевство юнита
        public Kingdom kingdom = new Kingdom();

        // Город юнита
        public City city = new City();

        // Культура юнита
        public Culture culture = new Culture();

        // Клан юнита
        public Clan clan = new Clan();

        // Влияние
        public int influence = 0;

        // Уровень
        public int level = 0;

        // Опыт
        public int experience = 0;

        // Голод
        public int hunger = 0;

        // Дипломатия
        public int diplomacy = 0;

        // Интеллект
        public int intelligence = 0;

        // Управление
        public int stewardship = 0;

        // Военное дело
        public int warfare = 0;

        // Личность
        public string personality = "";

        // Количество детей
        public int children = 0;
    }
}
