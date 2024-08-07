﻿using BepInEx;
using HarmonyLib;
using HarmonyLib.Tools;
using static ConstantNamespace.ConstantClass;
using static UnityEngine.UI.CanvasScaler;

namespace UnitsLogger_BepInEx
{
    [BepInPlugin(pluginGuid, pluginName, pluginVersion)]
    public class Main : BaseUnityPlugin
    {
        public static Harmony harmony = new Harmony(pluginName);
        private bool _initialized = false;

        public void Awake()
        {
            HarmonyFileLog.Enabled = true;
        }

        // Метод, запускающийся каждый кадр (в моём случае он зависим от загрузки игры)
        public void Update()
        {
            if (global::Config.gameLoaded)
            {
                // TODO: выяснить, что производительнее в данном случае

                //  foreach (Actor unit in World.world.units)
                //  {
                //      if (!unit.data.favorite)
                //      {
                //          unit?.SetIsTracked(true);
                //          unit?.gameObject?.AddComponent<LifeLogger>();
                //          unit.data.favorite = true;
                //      }
                //  }

                var unitList = World.world.units.getSimpleList();

                for (int i = 0; i < unitList.Count; i++)
                {                    
                    var unit = unitList[i];

                    if (!unit.data.favorite)
                    {
                        unit?.SetIsTracked(true);
                        unit?.gameObject?.AddComponent<LifeLogger>();
                        unit.data.favorite = true;
                    }
                }
            }

            if (global::Config.gameLoaded && !_initialized)
            {
                //foreach (var item in AssetManager.spells.list)
                //{
                //    Logger.LogMessage("  " + '"' + item.id + '"' + ": " + '"' + '"' + ',');
                //    Logger.LogMessage(item.id);
                //}
                //
                //Logger.LogMessage("=================================================================================================");
                //Logger.LogMessage("ТОП ТАЙЛЫ");
                //Logger.LogMessage("=================================================================================================");
                //
                //foreach (var item in AssetManager.topTiles.list)
                //{
                //    Logger.LogMessage("  " + '"' + item.id + '"' + ": " + '"' + '"' + ',');
                //}

                #region Локализация
                Localizer.SetLocalization("en", "actor_set_tracked", "Trait Editor now removes traits from a creature");
                Localizer.SetLocalization("ru", "actor_set_tracked", "Теперь вы отслеживаете жизненный путь этого юнита!");

                Localizer.SetLocalization("en", "actor_set_untracked", "Trait Editor now adds traits to the creature");
                Localizer.SetLocalization("ru", "actor_set_untracked", "Теперь вы не отслеживаете жизненный путь этого юнита!");

                CustomDictionary.SetLocal("item_mat_base", "Base");
                CustomDictionary.SetLocal("item_mat_base", "Базовый", "ru");

                CustomDictionary.SetLocal("Генерал", "General");
                CustomDictionary.SetLocal("Генерал", "Генерал", "ru");

                CustomDictionary.SetLocal("Гражданская принадлежность", "Civil affiliation");
                CustomDictionary.SetLocal("Гражданская принадлежность", "Гражданская принадлежность", "ru");

                CustomDictionary.SetLocal("Профессия", "Profession");
                CustomDictionary.SetLocal("Профессия", "Профессия", "ru");

                CustomDictionary.SetLocal("Политические характеристики", "Political characteristics");
                CustomDictionary.SetLocal("Политические характеристики", "Политические характеристики", "ru");

                CustomDictionary.SetLocal("Тип", "Type");
                CustomDictionary.SetLocal("Тип", "Тип", "ru");

                CustomDictionary.SetLocal("Год создания", "Year of creation");
                CustomDictionary.SetLocal("Год создания", "Год создания", "ru");

                CustomDictionary.SetLocal("Имя", "Name");
                CustomDictionary.SetLocal("Имя", "Имя", "ru");

                CustomDictionary.SetLocal("Создатель", "Creator");
                CustomDictionary.SetLocal("Создатель", "Создатель", "ru");

                CustomDictionary.SetLocal("Создан в государстве", "Was created in");
                CustomDictionary.SetLocal("Создан в государстве", "Создан в государстве", "ru");

                CustomDictionary.SetLocal("Модификаторы", "Modifiers");
                CustomDictionary.SetLocal("Модификаторы", "Модификаторы", "ru");

                CustomDictionary.SetLocal("Биологический вид", "Biological species");
                CustomDictionary.SetLocal("Биологический вид", "Биологический вид", "ru");

                CustomDictionary.SetLocal("Место смерти", "Place of death");
                CustomDictionary.SetLocal("Место смерти", "Место смерти", "ru");

                CustomDictionary.SetLocal("Сводка о смерти юнита", "Unit death report");
                CustomDictionary.SetLocal("Сводка о смерти юнита", "Сводка о смерти юнита", "ru");

                CustomDictionary.SetLocal("Пол", "Gender");
                CustomDictionary.SetLocal("Пол", "Пол", "ru");

                //foreach (var profession in UnitProfession.GetValues(typeof(UnitProfession)))
                //{
                //    CustomDictionary.SetLocal("profession_" + profession.ToString(), profession.ProfessionsLocalizationEn());
                //    CustomDictionary.SetLocal("profession_" + profession.ToString(), profession.ProfessionsLocalizationRu());
                //}

                //foreach (var gender in ActorGender.GetValues(typeof(ActorGender)))
                //{
                //    CustomDictionary.SetLocal("gender_" + gender.ToString(), gender.GendersLocalizationEn());
                //    CustomDictionary.SetLocal("gender_" + gender.ToString(), gender.GendersLocalizationRu());
                //}

                //foreach (var quality in ItemQuality.GetValues(typeof(ItemQuality)))
                //{
                //    CustomDictionary.SetLocal("quality_" + quality.ToString(), quality.QualityLocalizationEn());
                //    CustomDictionary.SetLocal("quality_" + quality.ToString(), quality.QualityLocalizationRu());
                //}

                //foreach (var job in AssetManager.citizen_job_library.list)
                //{
                //    CustomDictionary.SetLocal("job_" + job.id, job.JobsLocalizationEn());
                //    CustomDictionary.SetLocal("job_" + job.id, job.JobsLocalizationRu());
                //}
                #endregion

                #region Патчинг
                harmony.PatchAll(typeof(Patches));

                /*harmony.Patch(AccessTools.Method(typeof(RaceClick), "click"),
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

                  harmony.Patch(AccessTools.Method(typeof(Actor), "checkDieOnGround"),
                  prefix: new HarmonyMethod(AccessTools.Method(typeof(Patches), "checkDieOnGround_Prefix")));

                  harmony.Patch(AccessTools.Method(typeof(Actor), "checkDeathOutsideMap"),
                  prefix: new HarmonyMethod(AccessTools.Method(typeof(Patches), "checkDeathOutsideMap_Prefix")));

                  harmony.Patch(AccessTools.Method(typeof(ActionLibrary), "removeUnit"),
                  prefix: new HarmonyMethod(AccessTools.Method(typeof(Patches), "removeUnit_Prefix")));

                  harmony.Patch(AccessTools.Method(typeof(Actor), "addToInventory"),
                  prefix: new HarmonyMethod(AccessTools.Method(typeof(Patches), "addToInventory_Prefix")));*/
                #endregion

                _initialized = true;
            }
        }
    }
}
