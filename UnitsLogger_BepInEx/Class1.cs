using ai.behaviours;
using BepInEx;
using HarmonyLib;
using ReflectionUtility;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static ConstantNamespace.ConstantClass;

namespace UnitsLogger_BepInEx
{
    [BepInPlugin(pluginGuid, pluginName, pluginVersion)]
    public class UnitsLoggerClass : BaseUnityPlugin
    {
        public static Harmony harmony = new Harmony(pluginName);
        private bool _initialized = false;

        public void Awake()
        {
            Logger.LogMessage("ХООООООООЙ");
        }

        public void Start()
        {
            if (global::Config.gameLoaded)
            {
                Logger.LogMessage("Пропатчено");
            }
        }

        // Метод, запускающийся каждый кадр (в моём случае он зависим от загрузки игры)
        public void Update()
        {
            if (global::Config.gameLoaded)
            {
            }

            if (global::Config.gameLoaded && !_initialized)
            {
                Localizer.Localization("en", "actor_set_tracked", "Trait Editor now removes traits from a creature");
                Localizer.Localization("ru", "actor_set_tracked", "Теперь вы отслеживаете жизненный путь этого юнита!");

                Localizer.Localization("en", "actor_set_untracked", "Trait Editor now adds traits to the creature");
                Localizer.Localization("ru", "actor_set_untracked", "Теперь вы не отслеживаете жизненный путь этого юнита!");

                harmony.Patch(AccessTools.Method(typeof(RaceClick), "click"),
                postfix: new HarmonyMethod(AccessTools.Method(typeof(Patches), "click_Prefix")));

                harmony.Patch(AccessTools.Method(typeof(Actor), "killHimself"),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(Patches), "killHimself_Prefix")));

                harmony.Patch(AccessTools.Method(typeof(Actor), "setProfession"),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(Patches), "setProfession_Prefix")));

                _initialized = true;
            }
        }
    }

    class MyClass<T>
    {

    }

    public class ActorLogged<T> : Actor where T : ActorData
    {
        public void ClearAll()
        {
            killer_actor = new Actor();
            killer_building = new Building();
            received_items = new List<(double, ItemData)>();
            born_children = new List<(double, Actor)>();
            received_professions = new List<(double, UnitProfession)>();
            received_citizenships = new List<(double, Kingdom)>();
            received_townships = new List<(double, City)>();
            received_culturships = new List<(double, Culture)>();
            received_clanships = new List<(double, Clan)>();
            received_moods = new List<(double, string)>();
            eaten_food = new List<(double, string)>();

            received_names = new List<(double, string)>();
            received_traits = new List<(double, string)>();
            lost_traits = new List<(double, string)>();
        }

        // Случаи, когда юнит получал или менял имя
        public List<(double, string)> received_names = new List<(double, string)>();

        //Все черты, которые юнит получил за свою жизнь (значения - ID черты и точное время получения черты)
        public List<(double, string)> received_traits = new List<(double, string)>();

        //Все черты, которые юнит потерял за свою жизнь
        public List<(double, string)> lost_traits = new List<(double, string)>();

        // Существо, убившее юнита (если есть)
        public Actor killer_actor = new Actor();

        // Здание, убившее юнита (если есть)
        public Building killer_building = new Building();

        //Все предметы, которые юнит получил за свою жизнь
        public List<(double, ItemData)> received_items = new List<(double, ItemData)>();

        //Все предметы, которые юнит потерял за свою жизнь
        public List<(double, ItemData)> lost_items = new List<(double, ItemData)>();

        // Дети, которых юнит родил
        public List<(double, Actor)> born_children = new List<(double, Actor)>();

        // Случаи, когда юнит получал или менял профессию
        public List<(double, UnitProfession)> received_professions = new List<(double, UnitProfession)>();

        // Случаи, когда юнит получал или менял гражданство
        public List<(double, Kingdom)> received_citizenships = new List<(double, Kingdom)>();

        // Случаи, когда юнит получал или менял принадлежность к городу
        public List<(double, City)> received_townships = new List<(double, City)>();

        // Случаи, когда юнит получал или менял культуру
        public List<(double, Culture)> received_culturships = new List<(double, Culture)>();

        // Случаи, когда юнит получал или менял клан
        // ПОКА НЕ НУЖНО, ТАК КАК ЮНИТ НЕ МОЖЕТ МЕНЯТЬ ПРИНАДЛЕЖНОСТЬ К КЛАНУ
        public List<(double, Clan)> received_clanships = new List<(double, Clan)>();

        // Случаи, когда юнит получал или менял настроение
        public List<(double, string)> received_moods = new List<(double, string)>();

        // Случаи, когда юнит что-то ел
        public List<(double, string)> eaten_food = new List<(double, string)>();
    }

    public static class StaticStuff
    {
        #region Связь между ActorData и ActorLogged
        public static Dictionary<ActorData, ActorLogged<ActorData>> main_connection = new Dictionary<ActorData, ActorLogged<ActorData>>();

        public static void AddConnectionData(this ActorData data)
        {
            ActorDataLogged actor_logged = new ActorDataLogged();
            main_connection.Add(data, actor_logged);
        }

        public static void AddConnectionActor(ActorData data)
        {
            if (!main_connection.ContainsKey(data))
            {
                ActorDataLogged actor_logged = new ActorDataLogged();
                main_connection.Add(data, actor_logged);
            }
        }

        public static ActorDataLogged GetConnectionData(this ActorData data)
        {
            if (main_connection.ContainsKey(data))
            {
                return main_connection[data];
            }

            else
            {
                return null;
            }
        }

        #endregion

        public static double current_time = World.world.getCurWorldTime();

        public static string current_date = World.world.mapStats.getDate(current_time);

        #region GetActorData
        public static ActorData GetActorData(this Actor actor) => (ActorData)Reflection.GetField(actor.GetType(), actor, "data");

        public static ActorData GetActorData(this ActorBase actor) => (ActorData)Reflection.GetField(actor.GetType(), actor, "data");
        #endregion

        public static string CurrentDate(double time)
        {
            return World.world.mapStats.getDate(time);
        }

        #region SetIsTracked
        public static void SetIsTracked(BaseSystemData data, bool is_tracked)
        {
            data.set("tracked", is_tracked);
        }

        public static void SetIsTracked(BaseObjectData data, bool is_tracked)
        {
            data.set("tracked", is_tracked);
        }

        public static void SetIsTracked(ActorData data, bool is_tracked)
        {
            data.set("tracked", is_tracked);
        }

        public static void SetIsTracked(ActorDataLogged data, bool is_tracked)
        {
            data.set("tracked", is_tracked);
        }

        public static void SetIsTracked(BaseSimObject actor, bool is_tracked)
        {
            actor.base_data.set("tracked", is_tracked);
        }

        public static void SetIsTracked(Actor actor, bool is_tracked)
        {
            actor.base_data.set("tracked", is_tracked);
        }

        public static void SetIsTracked(ActorBase actor, bool is_tracked)
        {
            actor.base_data.set("tracked", is_tracked);
        }
        #endregion

        #region GetIsTracked
        public static bool GetIsTracked(BaseSystemData data)
        {
            data.get("tracked", out bool result);
            return result;
        }

        public static bool GetIsTracked(BaseObjectData data)
        {
            data.get("tracked", out bool result);
            return result;
        }

        public static bool GetIsTracked(ActorData data)
        {
            data.get("tracked", out bool result);
            return result;
        }

        public static bool GetIsTracked(ActorDataLogged data)
        {
            data.get("tracked", out bool result);
            return result;
        }

        public static bool GetIsTracked(Actor actor)
        {
            actor.base_data.get("tracked", out bool result);
            return result;
        }

        public static bool GetIsTracked(BaseSimObject actor)
        {
            actor.base_data.get("tracked", out bool result);
            return result;
        }

        public static bool GetIsTracked(ActorBase actor)
        {
            actor.base_data.get("tracked", out bool result);
            return result;
        }
        #endregion
    }

    public class Patches
    {
        public static void killHimself_Prefix(Actor __instance, bool pDestroy = false, AttackType pType = AttackType.Other, bool pCountDeath = true, bool pLaunchCallbacks = true, bool pLogFavorite = true)
        {
            if (StaticStuff.GetIsTracked(__instance))
            {
                if (!__instance.isAlive() && !pDestroy)
                {
                    return;
                }

                Logger.Saving(__instance, Logger.Prepare());

                actor_logged.ClearAll();
                actor_logged.gameObject.SetActive(false);
            }
        }

        #region Переключить отслеживаемость юнита
        public static void click_Prefix()
        {
            if (Config.selectedUnit != null)
            {
                if (StaticStuff.GetIsTracked(Config.selectedUnit))
                {
                    StaticStuff.SetIsTracked(Config.selectedUnit, false);

                    WorldTip.showNow("actor_set_untracked", true, "top", 1f);

                }

                else
                {
                    StaticStuff.SetIsTracked(Config.selectedUnit, true);
                    WorldTip.showNow("actor_set_tracked", true, "top", 1f);
                }
            }
        }
        #endregion

        #region Черты, которые юнит получил
        public static void addTrait_ActorBase_Prefix(ActorBase __instance, string pTrait, bool pRemoveOpposites = false)
        {
            if (StaticStuff.GetIsTracked(__instance))
            {
                if (!__instance.hasTrait(pTrait) && !(AssetManager.traits.get(pTrait) == null) && (pRemoveOpposites || !(bool)Reflection.CallMethod(__instance, "hasOppositeTrait", pTrait)))
                {
                    StaticStuff.AddConnectionActor(__instance.GetActorData());


                    actor.data_logged.received_traits.Add((StaticStuff.current_time, pTrait));
                }
            }
        }

        public void addTrait_ActorData_Prefix(ActorData __instance, string pTrait)
        {
            if (StaticStuff.GetIsTracked(__instance))
            {
                if (!__instance.traits.Contains(pTrait))
                {
                    ActorDataLogged data = (ActorDataLogged)__instance;

                    data.received_traits.Add((StaticStuff.current_time, pTrait));
                }
            }
        }
        #endregion

        #region Черты, которые юнит потерял
        public static void removeTrait_ActorBase_Prefix(ActorBase __instance, string pTraitID)
        {
            if (StaticStuff.GetIsTracked(__instance))
            {
                if (__instance.hasTrait(pTraitID))
                {
                    ActorLogged actor = (ActorLogged)__instance;

                    actor.data_logged.lost_traits.Add((StaticStuff.current_time, pTraitID));
                }
            }
        }

        public static void removeTrait_ActorData_Prefix(ActorData __instance, string pTraitID)
        {
            if (StaticStuff.GetIsTracked(__instance))
            {
                if (__instance.hasTrait(pTraitID))
                {
                    ActorDataLogged data = (ActorDataLogged)__instance;

                    data.lost_traits.Add((StaticStuff.current_time, pTraitID));
                }
            }
        }
        #endregion

        #region Смена профессии
        public static void setProfession_Prefix(Actor __instance, UnitProfession pType, bool pCancelBeh = true)
        {
            if (StaticStuff.GetIsTracked(__instance))
            {
                ActorLogged actor = (ActorLogged)__instance;

                actor.received_professions.Add((StaticStuff.current_time, pType));
            }
        }
        #endregion

        #region Смена королевства
        public static void setKingdom_Prefix(ActorBase __instance, Kingdom pKingdom)
        {
            if (StaticStuff.GetIsTracked(__instance))
            {
                if (__instance.kingdom != pKingdom)
                {
                    ActorLogged actor = (ActorLogged)__instance;

                    actor.received_citizenships.Add((StaticStuff.current_time, pKingdom));
                }
            }
        }
        #endregion

        #region Смена города
        public static void setCity_Prefix(ActorBase __instance, City pCity)
        {
            if (StaticStuff.GetIsTracked(__instance))
            {
                ActorLogged actor = (ActorLogged)__instance;

                actor.received_townships.Add((StaticStuff.current_time, pCity));
            }
        }
        #endregion

        #region Смена культуры
        public static void setCulture_Prefix(Actor __instance, Culture pCulture)
        {
            if (StaticStuff.GetIsTracked(__instance))
            {
                ActorLogged actor = (ActorLogged)__instance;

                actor.received_culturships.Add((StaticStuff.current_time, pCulture));
            }
        }
        #endregion

        #region Смена имени
        public static void setName_Prefix(ActorData __instance, string pName)
        {
            if (StaticStuff.GetIsTracked(__instance))
            {
                ActorDataLogged data = (ActorDataLogged)__instance;

                data.received_names.Add((StaticStuff.current_time, pName));
            }
        }
        #endregion

    }

    public class Logger
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

        public static void Saving(Actor actor, string folder_path)
        {
            ActorLogged actor_logged = (ActorLogged)actor;

            string war_file_path = Path.Combine(folder_path, actor_logged.data_logged.id + $", с {StaticStuff.CurrentDate(actor_logged.data.created_time)} и по {StaticStuff.current_date}" + ".txt");

            string unit_statistic = "";

            foreach (var profession in actor_logged.received_professions)
            {
                unit_statistic += $"\rЮнит взял себе профессию {profession.Item2.ToString()} {StaticStuff.CurrentDate(profession.Item1)}";
            }
        }
    }

    public static class Localizer
    {
        public static void Localization(string planguage, string id, string name)
        {
            string language = Reflection.GetField(LocalizedTextManager.instance.GetType(), LocalizedTextManager.instance, "language") as string;
            string templanguage;

            templanguage = language;

            if (templanguage != "ru" && templanguage != "en")
            {
                templanguage = "en";
            }

            if (planguage == templanguage)
            {
                Dictionary<string, string> localizedText = Reflection.GetField(LocalizedTextManager.instance.GetType(), LocalizedTextManager.instance, "localizedText") as Dictionary<string, string>;
                if (!localizedText.ContainsKey(id))
                {
                    localizedText.Add(id, name);
                }
                else if (localizedText.ContainsKey(id))
                {
                    localizedText.Remove(id);
                    localizedText.Add(id, name);
                }
            }
        }
    }

    public class Patches_НеДоделано
    {
        #region
        // ActionLibrary
        public static void removeUnit_Prefix(Actor pActor)
        {
            if (pActor.base_data.custom_data_flags.Count > 0)
            {
                var data = (ActorData)Reflection.GetField(pActor.GetType(), pActor, "data");

                if (data.favorite)
                {
                    pActor.base_data.custom_data_flags.Clear();
                    pActor.base_data.addFlag("dead_reason_transformation");
                }
            }
        }

        public static void checkDeathOutsideMap_Prefix(Actor __instance, Actor pActor)
        {
            if (!__instance.inMapBorder())
            {
                if (__instance.base_data.custom_data_flags.Count > 0)
                {
                    var data = (ActorData)Reflection.GetField(__instance.GetType(), __instance, "data");

                    if (data.favorite)
                    {
                        __instance.base_data.custom_data_flags.Clear();
                        __instance.base_data.addFlag("dead_reason_abroad");
                    }
                }
            }
        }

        public static void checkDieOnGround_Prefix(Actor __instance)
        {
            if (!__instance.currentTile.Type.liquid && __instance.isAlive() && !(bool)Reflection.CallMethod(__instance, "isInMagnet"))
            {
                if (__instance.base_data.custom_data_flags.Count > 0)
                {
                    var data = (ActorData)Reflection.GetField(__instance.GetType(), __instance, "data");

                    if (data.favorite)
                    {
                        __instance.base_data.custom_data_flags.Clear();
                        __instance.base_data.addFlag("dead_reason_die_on_ground");
                    }
                }
            }
        }

        public static bool updateAge_Prefix(Actor __instance, ref ActorData ___data, ref Race ___race, ref BaseStats ___stats)
        {

            if (!(bool)Reflection.CallMethod(___data, "updateAge", ___race, __instance.asset, ___stats[S.max_age]) && !__instance.hasTrait("immortal"))
            {
                if (__instance.base_data.custom_data_flags.Count > 0)
                {
                    var data = (ActorData)Reflection.GetField(__instance.GetType(), __instance, "data");

                    if (data.favorite)
                    {
                        __instance.base_data.custom_data_flags.Clear();
                        __instance.base_data.addFlag("dead_reason_age");
                    }
                }
                __instance.killHimself(false, AttackType.Age, true, true, true);
                return true;
            }
            if (__instance.city != null)
            {
                if (__instance.isKing())
                {
                    Reflection.CallMethod(__instance, "addExperience", 20);
                }
                if (__instance.isCityLeader())
                {
                    Reflection.CallMethod(__instance, "addExperience", 10);
                }
            }
            float num = (float)__instance.getAge();
            if (__instance.asset.unit && num > 300f && __instance.hasTrait("immortal") && Toolbox.randomBool())
            {
                __instance.addTrait("evil", false);
            }
            if (num > 40f && Toolbox.randomChance(0.3f))
            {
                __instance.addTrait("wise", false);
            }
            return false;
        }

        // BehBeeCheckHome
        public static bool BehBeeCheckHome_execute_Prefix(ref BehResult __result, Actor pActor)
        {
            if (pActor.isHomeBuildingUsable())
            {
                __result = BehResult.Continue;
                return true;
            }
            BiomeAsset biome_asset = pActor.currentTile.Type.biome_asset;
            if (((biome_asset != null) ? biome_asset.grow_type_selector_plants : null) != null)
            {
                BuildingActions.tryGrowVegetationRandom(pActor.currentTile, VegetationType.Plants, false, true);
            }

            if (pActor.base_data.custom_data_flags.Count > 0)
            {
                var data = (ActorData)Reflection.GetField(pActor.GetType(), pActor, "data");

                if (data.favorite)
                {
                    pActor.base_data.custom_data_flags.Clear();
                    pActor.base_data.addFlag("dead_reason_bee_no_home");
                }
            }

            pActor.killHimself(true, AttackType.None, false, true, true);
            __result = BehResult.Continue;
            return false;
        }

        public static void BehKillHimself_execute_Prefix(BehKillHimself __instance, Actor pActor)
        {
            if (pActor.base_data.custom_data_flags.Count > 0)
            {
                var data = (ActorData)Reflection.GetField(pActor.GetType(), pActor, "data");

                if (data.favorite)
                {
                    pActor.base_data.custom_data_flags.Clear();
                    pActor.base_data.addFlag("dead_reason_beh_kill_himself");
                }
            }
        }

        public static void getHit_Prefix(Actor __instance, ref Dictionary<string, StatusEffectData> ___activeStatus_dict, ref bool ___has_attack_target, ref float ___timer_action, ref BaseStats ___stats, ref ActorData ___data, ref BaseSimObject ___attackedBy, ref bool ___shake_active, float pDamage, bool pFlash = true, AttackType pAttackType = AttackType.Other, BaseSimObject pAttacker = null, bool pSkipIfShake = true, bool pMetallicWeapon = false)
        {
            ___attackedBy = null;

            if (pSkipIfShake && ___shake_active)
            {
                return;
            }

            if ((bool)Reflection.CallMethod(__instance, "hasStatus", "invincible"))
            {
                return;
            }

            if (pAttacker != __instance)
            {
                ___attackedBy = pAttacker;
            }


            if (___data.health <= 0)
            {
                if (pAttacker != null && pAttacker != __instance && (bool)Reflection.CallMethod(pAttacker, "isActor") && pAttacker.isAlive())
                {
                    Actor a = (Actor)Reflection.GetField(pAttacker.GetType(), pAttacker, "a");

                    var actor = (ActorLogged)__instance;
                    actor.killer_actor = a;

                    if (__instance.base_data.custom_data_flags.Count > 0)
                    {
                        var data = (ActorData)Reflection.GetField(__instance.GetType(), __instance, "data");

                        if (data.favorite)
                        {
                            __instance.base_data.custom_data_flags.Clear();
                            switch (pAttackType)
                            {
                                case AttackType.Acid:
                                    __instance.base_data.addFlag("dead_reason_was_killed_by_actor_asid");
                                    break;

                                case AttackType.Age:
                                    __instance.base_data.addFlag("dead_reason_was_killed_by_actor_age");
                                    break;

                                case AttackType.AshFever:
                                    __instance.base_data.addFlag("dead_reason_was_killed_by_actor_ash_fever");
                                    break;

                                case AttackType.Block:
                                    __instance.base_data.addFlag("dead_reason_was_killed_by_actor_block");
                                    break;

                                case AttackType.Eaten:
                                    __instance.base_data.addFlag("dead_reason_was_killed_by_actor_eaten");
                                    break;

                                case AttackType.Fire:
                                    __instance.base_data.addFlag("dead_reason_was_killed_by_actor_fire");
                                    break;

                                case AttackType.Hunger:
                                    __instance.base_data.addFlag("dead_reason_was_killed_by_actor_hunger");
                                    break;

                                case AttackType.Infection:
                                    __instance.base_data.addFlag("dead_reason_was_killed_by_actor_infection");
                                    break;

                                case AttackType.None:
                                    __instance.base_data.addFlag("dead_reason_was_killed_by_actor_none");
                                    break;

                                case AttackType.Other:
                                    __instance.base_data.addFlag("dead_reason_was_killed_by_actor_other");
                                    break;

                                case AttackType.Plague:
                                    __instance.base_data.addFlag("dead_reason_was_killed_by_actor_plague");
                                    break;

                                case AttackType.Poison:
                                    __instance.base_data.addFlag("dead_reason_was_killed_by_actor_poison");
                                    break;

                                case AttackType.Transformation:
                                    __instance.base_data.addFlag("dead_reason_was_killed_by_actor_transformation");
                                    break;

                                case AttackType.Tumor:
                                    __instance.base_data.addFlag("dead_reason_was_killed_by_actor_tumor");
                                    break;

                                case AttackType.Weapon:
                                    __instance.base_data.addFlag("dead_reason_was_killed_by_actor_weapon");
                                    break;

                                default:
                                    __instance.base_data.addFlag("dead_reason_was_killed_by_actor");
                                    break;
                            }
                        }
                    }
                }

                else if (pAttacker != null && pAttacker != __instance && (bool)Reflection.CallMethod(pAttacker, "isBuilding") && pAttacker.isAlive())
                {
                    Building b = (Building)Reflection.GetField(pAttacker.GetType(), pAttacker, "b");

                    var actor = (ActorLogged)__instance;
                    actor.killer_building = b;

                    if (__instance.base_data.custom_data_flags.Count > 0)
                    {
                        var data = (ActorData)Reflection.GetField(__instance.GetType(), __instance, "data");

                        if (data.favorite)
                        {
                            __instance.base_data.custom_data_flags.Clear();
                            switch (pAttackType)
                            {
                                case AttackType.Acid:
                                    __instance.base_data.addFlag("dead_reason_was_killed_by_building_asid");
                                    break;

                                case AttackType.Age:
                                    __instance.base_data.addFlag("dead_reason_was_killed_by_building_age");
                                    break;

                                case AttackType.AshFever:
                                    __instance.base_data.addFlag("dead_reason_was_killed_by_building_ash_fever");
                                    break;

                                case AttackType.Block:
                                    __instance.base_data.addFlag("dead_reason_was_killed_by_building_block");
                                    break;

                                case AttackType.Eaten:
                                    __instance.base_data.addFlag("dead_reason_was_killed_by_building_eaten");
                                    break;

                                case AttackType.Fire:
                                    __instance.base_data.addFlag("dead_reason_was_killed_by_building_fire");
                                    break;

                                case AttackType.Hunger:
                                    __instance.base_data.addFlag("dead_reason_was_killed_by_building_hunger");
                                    break;

                                case AttackType.Infection:
                                    __instance.base_data.addFlag("dead_reason_was_killed_by_building_infection");
                                    break;

                                case AttackType.None:
                                    __instance.base_data.addFlag("dead_reason_was_killed_by_building_none");
                                    break;

                                case AttackType.Other:
                                    __instance.base_data.addFlag("dead_reason_was_killed_by_building_other");
                                    break;

                                case AttackType.Plague:
                                    __instance.base_data.addFlag("dead_reason_was_killed_by_building_plague");
                                    break;

                                case AttackType.Poison:
                                    __instance.base_data.addFlag("dead_reason_was_killed_by_building_poison");
                                    break;

                                case AttackType.Transformation:
                                    __instance.base_data.addFlag("dead_reason_was_killed_by_building_transformation");
                                    break;

                                case AttackType.Tumor:
                                    __instance.base_data.addFlag("dead_reason_was_killed_by_building_tumor");
                                    break;

                                case AttackType.Weapon:
                                    __instance.base_data.addFlag("dead_reason_was_killed_by_building_weapon");
                                    break;

                                default:
                                    __instance.base_data.addFlag("dead_reason_was_killed_by_building");
                                    break;
                            }
                        }
                    }
                }

                else if (pAttacker == __instance)
                {
                    if (__instance.base_data.custom_data_flags.Count > 0)
                    {
                        var data = (ActorData)Reflection.GetField(__instance.GetType(), __instance, "data");

                        if (data.favorite)
                        {
                            __instance.base_data.custom_data_flags.Clear();
                            __instance.base_data.addFlag("dead_reason_kill_himself");
                        }
                    }
                }

                else if (true)
                {

                }
            }
        }
        #endregion
    }
}
