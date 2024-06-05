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

                CustomDictionary.SetLocal("item_mat_base", "Base");
                CustomDictionary.SetLocal("item_mat_base", "Базовый");

                CustomDictionary.SetLocal("Генерал", "General");
                CustomDictionary.SetLocal("Генерал", "Генерал");

                CustomDictionary.SetLocal("Гражданская принадлежность", "Civil affiliation");
                CustomDictionary.SetLocal("Гражданская принадлежность", "Гражданская принадлежность");

                CustomDictionary.SetLocal("Профессия", "Profession");
                CustomDictionary.SetLocal("Профессия", "Профессия");

                CustomDictionary.SetLocal("Политические характеристики", "Political characteristics");
                CustomDictionary.SetLocal("Политические характеристики", "Политические характеристики");

                CustomDictionary.SetLocal("Тип", "Type");
                CustomDictionary.SetLocal("Тип", "Тип");

                CustomDictionary.SetLocal("Год создания", "Year of creation");
                CustomDictionary.SetLocal("Год создания", "Год создания");

                CustomDictionary.SetLocal("Имя", "Name");
                CustomDictionary.SetLocal("Имя", "Имя");

                CustomDictionary.SetLocal("Создатель", "Creator");
                CustomDictionary.SetLocal("Создатель", "Создатель");

                CustomDictionary.SetLocal("Создан в государстве", "Was created in");
                CustomDictionary.SetLocal("Создан в государстве", "Создан в государстве");

                CustomDictionary.SetLocal("Модификаторы", "Modifiers");
                CustomDictionary.SetLocal("Модификаторы", "Модификаторы");

                CustomDictionary.SetLocal("Биологический вид", "Biological species");
                CustomDictionary.SetLocal("Биологический вид", "Биологический вид");

                CustomDictionary.SetLocal("Место смерти", "Place of death");
                CustomDictionary.SetLocal("Место смерти", "Место смерти");

                CustomDictionary.SetLocal("Сводка о смерти юнита", "Unit death report");
                CustomDictionary.SetLocal("Сводка о смерти юнита", "Сводка о смерти юнита");

                CustomDictionary.SetLocal("Пол", "Gender");
                CustomDictionary.SetLocal("Пол", "Пол");

                foreach (var profession in UnitProfession.GetValues(typeof(UnitProfession)))
                {
                    CustomDictionary.SetLocal("profession_" + profession.ToString(), profession.ProfessionsLocalizationEn());
                    CustomDictionary.SetLocal("profession_" + profession.ToString(), profession.ProfessionsLocalizationRu());
                }

                foreach (var gender in ActorGender.GetValues(typeof(ActorGender)))
                {
                    CustomDictionary.SetLocal("gender_" + gender.ToString(), gender.GendersLocalizationEn());
                    CustomDictionary.SetLocal("gender_" + gender.ToString(), gender.GendersLocalizationRu());
                }

                foreach (var quality in ItemQuality.GetValues(typeof(ItemQuality)))
                {
                    CustomDictionary.SetLocal("quality_" + quality.ToString(), quality.QualityLocalizationEn());
                    CustomDictionary.SetLocal("quality_" + quality.ToString(), quality.QualityLocalizationRu());
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

                harmony.Patch(AccessTools.Method(typeof(ActorData), "updateAttributes"),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(Patches), "updateAttributes_Prefix")));
                #endregion

                _initialized = true;
            }
        }
    }
}
