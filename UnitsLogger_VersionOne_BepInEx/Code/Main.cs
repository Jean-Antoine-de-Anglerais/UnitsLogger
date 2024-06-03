using BepInEx;
using HarmonyLib;
using static ConstantNamespace.ConstantClass;

namespace UnitsLogger_BepInEx
{
    [BepInPlugin(pluginGuid, pluginName, pluginVersion)]
    public class Main : BaseUnityPlugin
    {
        public static Harmony harmony = new Harmony(pluginName);
        private bool _initialized = false;

        // Метод, запускающийся каждый кадр (в моём случае он зависим от загрузки игры)
        public void Update()
        {
            /*if (global::Config.gameLoaded)
            {
                foreach (var unit in World.world.units)
                {
                    if (unit.asset.canBeCitizen && !StaticStuff.GetIsTracked(unit) && unit.hasTrait("immortal"))
                    {
                        unit.SetIsTracked(true);
                        unit.GetActorData().favorite = true;
                    }
                }
            }*/

            if (global::Config.gameLoaded && !_initialized)
            {
                #region Локализация
                Localizer.SetLocalization("en", "actor_set_tracked", "Trait Editor now removes traits from a creature");
                Localizer.SetLocalization("ru", "actor_set_tracked", "Теперь вы отслеживаете жизненный путь этого юнита!");

                Localizer.SetLocalization("en", "actor_set_untracked", "Trait Editor now adds traits to the creature");
                Localizer.SetLocalization("ru", "actor_set_untracked", "Теперь вы не отслеживаете жизненный путь этого юнита!");

                Localizer.SetLocalization("en", "item_mat_base", "Base");
                Localizer.SetLocalization("ru", "item_mat_base", "Базовый");

                Localizer.SetLocalization("en", "Генерал", "General");
                Localizer.SetLocalization("ru", "Генерал", "Генерал");

                Localizer.SetLocalization("en", "Гражданская принадлежность", "Civil affiliation");
                Localizer.SetLocalization("ru", "Гражданская принадлежность", "Гражданская принадлежность");

                Localizer.SetLocalization("en", "Профессия", "Profession");
                Localizer.SetLocalization("ru", "Профессия", "Профессия");

                Localizer.SetLocalization("en", "Политические характеристики", "Political characteristics");
                Localizer.SetLocalization("ru", "Политические характеристики", "Политические характеристики");

                Localizer.SetLocalization("en", "Тип", "Type");
                Localizer.SetLocalization("ru", "Тип", "Тип");

                Localizer.SetLocalization("en", "Год создания", "Year of creation");
                Localizer.SetLocalization("ru", "Год создания", "Год создания");

                Localizer.SetLocalization("en", "Имя", "Name");
                Localizer.SetLocalization("ru", "Имя", "Имя");

                Localizer.SetLocalization("en", "Создатель", "Creator");
                Localizer.SetLocalization("ru", "Создатель", "Создатель");

                Localizer.SetLocalization("en", "Создан в государстве", "Was created in");
                Localizer.SetLocalization("ru", "Создан в государстве", "Создан в государстве");

                Localizer.SetLocalization("en", "Модификаторы", "Modifiers");
                Localizer.SetLocalization("ru", "Модификаторы", "Модификаторы");

                Localizer.SetLocalization("en", "Биологический вид", "Biological species");
                Localizer.SetLocalization("ru", "Биологический вид", "Биологический вид");

                Localizer.SetLocalization("en", "Место смерти", "Place of death");
                Localizer.SetLocalization("ru", "Место смерти", "Место смерти");

                Localizer.SetLocalization("en", "Сводка о смерти юнита", "Unit death report");
                Localizer.SetLocalization("ru", "Сводка о смерти юнита", "Сводка о смерти юнита");

                Localizer.SetLocalization("en", "Пол", "Gender");
                Localizer.SetLocalization("ru", "Пол", "Пол");

                foreach (var profession in UnitProfession.GetValues(typeof(UnitProfession)))
                {
                    Localizer.SetLocalization("en", "profession_" + profession.ToString(), profession.ProfessionsLocalizationEn());
                    Localizer.SetLocalization("ru", "profession_" + profession.ToString(), profession.ProfessionsLocalizationRu());
                }

                foreach (var gender in ActorGender.GetValues(typeof(ActorGender)))
                {
                    Localizer.SetLocalization("en", "gender_" + gender.ToString(), gender.GendersLocalizationEn());
                    Localizer.SetLocalization("ru", "gender_" + gender.ToString(), gender.GendersLocalizationRu());
                }

                foreach (var quality in ItemQuality.GetValues(typeof(ItemQuality)))
                {
                    Localizer.SetLocalization("en", "quality_" + quality.ToString(), quality.QualityLocalizationEn());
                    Localizer.SetLocalization("ru", "quality_" + quality.ToString(), quality.QualityLocalizationRu());
                }
                #endregion

                #region Патчинг
                harmony.Patch(AccessTools.Method(typeof(RaceClick), "click"),
                postfix: new HarmonyMethod(AccessTools.Method(typeof(Patches), "click_Prefix")));

                harmony.Patch(AccessTools.Method(typeof(Actor), "killHimself"),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(Patches), "killHimself_Prefix")));

                harmony.Patch(AccessTools.Method(typeof(ActorBase), "addTrait"),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(Patches), "addTrait_ActorBase_Prefix")));

                harmony.Patch(AccessTools.Method(typeof(ActorData), "addTrait"),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(Patches), "addTrait_ActorData_Prefix")));

                harmony.Patch(AccessTools.Method(typeof(ActorBase), "removeTrait"),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(Patches), "removeTrait_ActorBase_Prefix")));

                harmony.Patch(AccessTools.Method(typeof(ActorData), "removeTrait"),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(Patches), "removeTrait_ActorData_Prefix")));

                harmony.Patch(AccessTools.Method(typeof(Actor), "setProfession"),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(Patches), "setProfession_Prefix")));

                harmony.Patch(AccessTools.Method(typeof(ActorBase), "setKingdom"),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(Patches), "setKingdom_Prefix")));

                harmony.Patch(AccessTools.Method(typeof(Actor), "setCulture"),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(Patches), "setCulture_Prefix")));

                harmony.Patch(AccessTools.Method(typeof(ActorData), "setName"),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(Patches), "setName_Prefix")));

                harmony.Patch(AccessTools.Method(typeof(Actor), "setCity"),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(Patches), "setCity_Prefix")));

                harmony.Patch(AccessTools.Method(typeof(Actor), "changeMood"),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(Patches), "changeMood_Prefix")));

                harmony.Patch(AccessTools.Method(typeof(Actor), "newKillAction"),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(Patches), "newKillAction_Prefix")));

                harmony.Patch(AccessTools.Method(typeof(Actor), "consumeCityFoodItem"),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(Patches), "consumeCityFoodItem_Prefix")));
                #endregion

                _initialized = true;
            }
        }
    }
}
