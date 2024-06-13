using ai;
using ai.behaviours;
using BepInEx;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;

namespace UnitsLogger_BepInEx
{
    public static class Patches
    {
        #region Переключить отслеживаемость юнита
        [HarmonyPostfix]
        [HarmonyPatch(typeof(RaceClick), nameof(RaceClick.click))]
        public static void click_Postfix()
        {
            if (Config.selectedUnit != null)
            {
                if (StaticStuff.GetIsTracked(Config.selectedUnit))
                {
                    StaticStuff.SetIsTracked(Config.selectedUnit, false);

                    WorldTip.showNow("actor_set_untracked", true, "top", 1f);

                    // Уничтожение компонента LifeLogger
                    LifeLogger logger = Config.selectedUnit.gameObject.GetComponent<LifeLogger>();
                    if (logger != null)
                    {
                        UnityEngine.Object.Destroy(logger);
                    }
                }

                else
                {
                    StaticStuff.SetIsTracked(Config.selectedUnit, true);
                    WorldTip.showNow("actor_set_tracked", true, "top", 1f);
                    Config.selectedUnit.gameObject.AddComponent<LifeLogger>();
                }
            }
        }
        #endregion

        #region Запуск сохранения
        [HarmonyPrefix]
        [HarmonyPatch(typeof(Actor), nameof(Actor.killHimself))]
        public static void killHimself_Prefix(Actor __instance, bool pDestroy = false, AttackType pType = AttackType.Other, bool pCountDeath = true, bool pLaunchCallbacks = true, bool pLogFavorite = true)
        {
            if (StaticStuff.GetIsTracked(__instance))
            {
                if (__instance.isAlive())
                {
                    DeadLogger.SavingDead(__instance, DeadLogger.Prepare());
                    StaticStuff.SetIsTracked(__instance, false);

                    // Уничтожение компонента LifeLogger
                    LifeLogger logger = __instance.gameObject.GetComponent<LifeLogger>();
                    if (logger != null)
                    {
                        UnityEngine.Object.Destroy(logger);
                    }
                }
            }
        }
        #endregion

        #region Черты, которые юнит получил
        [HarmonyPrefix]
        [HarmonyPatch(typeof(ActorBase), nameof(ActorBase.addTrait))]
        public static void addTrait_ActorBase_Prefix(ActorBase __instance, string pTrait, bool pRemoveOpposites = false)
        {
            if (StaticStuff.GetIsTracked(__instance))
            {
                if (!__instance.hasTrait(pTrait) && !(AssetManager.traits.get(pTrait) == null) && (pRemoveOpposites || !(bool)Reflection.CallMethod(__instance, "hasOppositeTrait", pTrait)))
                {
                    LifeLogger logger = __instance.gameObject.GetComponent<LifeLogger>();
                    logger?.received_traits.Add((World.world.getCurWorldTime(), pTrait, DataType.ReceivedTraits));
                }
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(ActorData), nameof(ActorData.addTrait))]
        public static void addTrait_ActorData_Prefix(ActorData __instance, string pTrait)
        {
            if (StaticStuff.GetIsTracked(__instance))
            {
                if (!__instance.traits.Contains(pTrait))
                {
                    Actor actor = World.world.units.get(__instance.id);

                    LifeLogger logger = actor.gameObject.GetComponent<LifeLogger>();

                    logger?.received_traits.Add((World.world.getCurWorldTime(), pTrait, DataType.ReceivedTraits));
                }
            }
        }
        #endregion

        #region Черты, которые юнит потерял
        [HarmonyPrefix]
        [HarmonyPatch(typeof(ActorBase), nameof(ActorBase.removeTrait))]
        public static void removeTrait_ActorBase_Prefix(ActorBase __instance, string pTraitID)
        {
            if (StaticStuff.GetIsTracked(__instance))
            {
                if (__instance.hasTrait(pTraitID))
                {
                    LifeLogger logger = __instance.gameObject.GetComponent<LifeLogger>();

                    logger?.lost_traits.Add((World.world.getCurWorldTime(), pTraitID, DataType.LostTraits));
                }
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(ActorData), nameof(ActorData.removeTrait))]
        public static void removeTrait_ActorData_Prefix(ActorData __instance, string pTraitID)
        {
            if (StaticStuff.GetIsTracked(__instance))
            {
                if (__instance.hasTrait(pTraitID))
                {
                    Actor actor = World.world.units.get(__instance.id);

                    LifeLogger logger = actor.gameObject.GetComponent<LifeLogger>();

                    logger?.lost_traits.Add((World.world.getCurWorldTime(), pTraitID, DataType.LostTraits));
                }
            }
        }
        #endregion

        #region Смена профессии
        [HarmonyPrefix]
        [HarmonyPatch(typeof(Actor), "setProfession")]
        public static void setProfession_Prefix(Actor __instance, UnitProfession pType, bool pCancelBeh = true)
        {
            if (StaticStuff.GetIsTracked(__instance))
            {
                LifeLogger logger = __instance.gameObject.GetComponent<LifeLogger>();

                logger?.received_professions.Add((World.world.getCurWorldTime(), pType, DataType.Professions));
            }
        }
        #endregion

        #region Смена королевства
        [HarmonyPrefix]
        [HarmonyPatch(typeof(ActorBase), "setKingdom")]
        public static void setKingdom_Prefix(ActorBase __instance, Kingdom pKingdom)
        {
            if (StaticStuff.GetIsTracked(__instance))
            {
                if (__instance.kingdom != pKingdom)
                {
                    LifeLogger logger = __instance.gameObject.GetComponent<LifeLogger>();

                    logger?.received_citizenships.Add((World.world.getCurWorldTime(), pKingdom.data.name, DataType.Сitizenships));
                }
            }
        }
        #endregion

        #region Смена города
        [HarmonyPrefix]
        [HarmonyPatch(typeof(Actor), "setCity")]
        public static void setCity_Prefix(Actor __instance, City pCity)
        {
            if (StaticStuff.GetIsTracked(__instance))
            {
                LifeLogger logger = __instance.gameObject.GetComponent<LifeLogger>();

                if (pCity != null && logger != null)
                {
                    logger?.received_townships.Add((World.world.getCurWorldTime(), pCity.data.name, DataType.Townships));
                }
            }
        }
        #endregion

        #region Смена культуры
        [HarmonyPrefix]
        [HarmonyPatch(typeof(Actor), "setCulture")]
        public static void setCulture_Prefix(Actor __instance, Culture pCulture)
        {
            if (StaticStuff.GetIsTracked(__instance))
            {
                LifeLogger logger = __instance.gameObject.GetComponent<LifeLogger>();

                logger?.received_culturships.Add((World.world.getCurWorldTime(), pCulture.data.name, DataType.Culturships));
            }
        }
        #endregion

        #region Смена имени
        [HarmonyPrefix]
        [HarmonyPatch(typeof(ActorData), nameof(ActorData.setName))]
        public static void setName_Prefix(ActorData __instance, string pName)
        {
            if (StaticStuff.GetIsTracked(__instance))
            {
                Actor actor = World.world.units.get(__instance.id);

                LifeLogger logger = actor.gameObject.GetComponent<LifeLogger>();

                if (logger != null)
                {
                    if (logger.initial_name.IsNullOrWhiteSpace())
                    {
                        logger.initial_name = __instance.name;
                    }

                    if (!logger.received_names.ContainsKey(World.world.getCurWorldTime()))
                    {
                        logger.received_names.Add(World.world.getCurWorldTime(), pName);
                    }
                    else
                    {
                        logger.received_names.Remove(World.world.getCurWorldTime());
                        logger.received_names.Add(World.world.getCurWorldTime(), pName);
                    }
                }
            }
        }
        #endregion

        #region Смена настроения
        [HarmonyPrefix]
        [HarmonyPatch(typeof(Actor), "changeMood")]
        public static void changeMood_Prefix(Actor __instance, string pMood)
        {
            if (StaticStuff.GetIsTracked(__instance))
            {
                LifeLogger logger = __instance.gameObject.GetComponent<LifeLogger>();

                if (pMood != __instance.GetActorData().mood)
                {
                    logger?.received_moods.Add((World.world.getCurWorldTime(), pMood, DataType.Moods));
                }
            }
        }
        #endregion

        #region Убийство юнита
        [HarmonyPrefix]
        [HarmonyPatch(typeof(Actor), "newKillAction")]
        public static void newKillAction_Prefix(Actor __instance, Actor pDeadUnit, Kingdom pPrevKingdom)
        {
            if (StaticStuff.GetIsTracked(__instance))
            {
                LifeLogger logger = __instance.gameObject.GetComponent<LifeLogger>();

                if (pDeadUnit != null)
                {
                    logger?.killed_units.Add((World.world.getCurWorldTime(), $"существо вида {pDeadUnit.asset.nameLocale.GetLocal()}, по имени {pDeadUnit.getName()}", DataType.KilledUnits));
                }
            }

            if (StaticStuff.GetIsTracked(pDeadUnit))
            {
                LifeLogger logger = pDeadUnit.gameObject.GetComponent<LifeLogger>();

                if (__instance != null)
                {
                    logger.killer_actor = __instance;
                }
            }
        }
        #endregion

        #region Рождение юнита
        [HarmonyPrefix]
        [HarmonyPatch(typeof(CityBehProduceUnit), "produceNewCitizen")] // Что-то из этого метода вызывает ошибки (ошибки в блокноте)
        public static bool produceNewCitizen_Prefix(CityBehProduceUnit __instance, ref bool __result, ref List<Actor> ____possibleParents, Building pBuilding, City pCity)
        {
            Actor actor = ____possibleParents.Pop();
            if (actor == null)
            {
                __result = false;
                return false;
            }
            if (!Toolbox.randomChance(actor.GetBaseStats()[S.fertility]))
            {
                __result = false;
                return false;
            }
            Actor actor2 = null;
            if (____possibleParents.Count > 0)
            {
                actor2 = ____possibleParents.Pop();
            }
            ResourceAsset foodItem = (ResourceAsset)pCity.CallMethod("getFoodItem", "");
            pCity.CallMethod("eatFoodItem", foodItem.id);
            pCity.status.housingFree--;
            pCity.data.born++;
            if (pCity.GetCityKingdom() != null)
            {
                pCity.GetCityKingdom().data.born++;
            }
            ActorAsset asset = actor.asset;
            ActorData actorData = new ActorData();
            actorData.created_time = MapBox.instance.getCreationTime();
            actorData.cityID = pCity.data.id;
            actorData.id = MapBox.instance.mapStats.getNextId("unit");
            actorData.asset_id = asset.id;
            ActorBase.generateCivUnit(actor.asset, actorData, actor.GetActorRace());
            actorData.generateTraits(asset, actor.GetActorRace());
            actorData.CallMethod("inheritTraits", actor.GetActorData().traits);
            actorData.hunger = asset.maxHunger / 2;
            actor.GetActorData().makeChild(MapBox.instance.getCurWorldTime());
            if (actor2 != null)
            {
                actorData.CallMethod("inheritTraits", actor2.GetActorData().traits);
                actor2.GetActorData().makeChild(MapBox.instance.getCurWorldTime());
            }
            Clan clan = StaticStuff.checkGreatClan(actor, actor2);
            actorData.skin = ActorTool.getBabyColor(actor, actor2);
            actorData.skin_set = actor.GetActorData().skin_set;
            Culture babyCulture = StaticStuff.getBabyCulture(actor, actor2);
            if (babyCulture != null)
            {
                actorData.culture = babyCulture.data.id;
                actorData.level = babyCulture.getBornLevel();
            }
            if (clan != null)
            {
                Actor pActor = pCity.spawnPopPoint(actorData, actor.currentTile);
                clan.addUnit(pActor);
            }
            else
            {
                pCity.addPopPoint(actorData);
            }

            actor.makeChild(MapBox.instance.getCurWorldTime(), actor2, actorData);

            if (actor2 != null)
            {
                actor2.makeChild(MapBox.instance.getCurWorldTime(), actor, actorData);
            }

            __result = true;
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(BehMakeBaby), "makeBaby")]
        public static bool makeBaby_Prefix(Actor pActor1, Actor pActor2)
        {
            pActor1.startBabymakingTimeout();
            pActor2.startBabymakingTimeout();
            pActor1.GetActorData().children++;
            pActor2.GetActorData().children++;
            string pStatsID;
            if (Toolbox.randomBool())
            {
                pStatsID = pActor1.asset.id;
            }
            else
            {
                pStatsID = pActor2.asset.id;
            }
            WorldTile worldTile = null;
            foreach (WorldTile worldTile2 in pActor1.currentTile.neighbours)
            {
                if (worldTile2 != pActor1.currentTile && worldTile2 != pActor1.currentTile && !worldTile2.Type.liquid)
                {
                    worldTile = worldTile2;
                    break;
                }
            }
            if (worldTile == null)
            {
                worldTile = pActor1.currentTile;
            }
            Actor actor = MapBox.instance.units.createNewUnit(pStatsID, worldTile, 6f);
            actor.CallMethod("justBorn");
            actor.GetActorData().hunger = 79;
            if (actor.asset.useSkinColors)
            {
                if (Toolbox.randomBool())
                {
                    actor.GetActorData().skin_set = pActor1.GetActorData().skin_set;
                }
                else
                {
                    actor.GetActorData().skin_set = pActor2.GetActorData().skin_set;
                }
                actor.GetActorData().skin = ActorTool.getBabyColor(pActor1, pActor2);
            }

            pActor1.makeChild(MapBox.instance.getCurWorldTime(), pActor2, actor);

            pActor2.makeChild(MapBox.instance.getCurWorldTime(), pActor1, actor);

            ((GameStatsData)Reflection.GetField(MapBox.instance.gameStats.GetType(), MapBox.instance.gameStats, "data")).creaturesBorn++;
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(BehTryBabymaking), "makeEgg")]
        public static bool makeEgg_Prefix(Actor pActor)
        {
            pActor.startBabymakingTimeout();
            pActor.GetActorData().children++;
            WorldTile worldTile = null;
            foreach (WorldTile worldTile2 in pActor.currentTile.neighbours)
            {
                if (worldTile2 != pActor.currentTile && worldTile2 != pActor.currentTile && !worldTile2.Type.liquid)
                {
                    worldTile = worldTile2;
                    break;
                }
            }
            if (worldTile == null)
            {
                worldTile = pActor.currentTile;
            }
            Actor actor = MapBox.instance.units.createNewUnit(pActor.asset.eggStatsID, worldTile, 6f);
            if (pActor.asset.useSkinColors)
            {
                actor.GetActorData().skin_set = pActor.GetActorData().skin_set;
            }
            if (StaticStuff.GetIsTracked(pActor))
            {
                LifeLogger logger = pActor.gameObject.GetComponent<LifeLogger>();

                logger?.born_children.Add((World.world.getCurWorldTime(), actor.getName(), actor.GetActorData().gender, DataType.Children));
            }
            return false;
        }
        #endregion

        #region Поедание еды
        [HarmonyPrefix]
        [HarmonyPatch(typeof(Actor), nameof(Actor.consumeCityFoodItem))]
        public static void consumeCityFoodItem_Prefix(Actor __instance, ResourceAsset pAsset)
        {
            if (StaticStuff.GetIsTracked(__instance))
            {
                LifeLogger logger = __instance.gameObject.GetComponent<LifeLogger>();

                logger?.eaten_food.Add((World.world.getCurWorldTime(), pAsset.id, DataType.Food));
            }
        }
        #endregion

        #region Изменение социальных характеристик юнита
        [HarmonyPrefix]
        [HarmonyPatch(typeof(ActorData), "updateAttributes")]
        public static bool updateAttributes_Prefix(ActorData __instance, ActorAsset pAsset, Race pRace, bool pForce = false)
        {
            if (pAsset.unit)
            {
                float num = __instance.getAge();
                if ((num % 3f == 0f && num <= 100f) || pForce)
                {
                    switch (pRace.preferred_attribute.GetRandom())
                    {
                        case "intelligence":
                            __instance.intelligence++;
                            if (StaticStuff.GetIsTracked(__instance))
                            {
                                Actor actor = World.world.units.get(__instance.id);
                                LifeLogger logger = actor.gameObject.GetComponent<LifeLogger>();

                                logger?.social_characteristics.Add((World.world.getCurWorldTime(), "intelligence", DataType.SocialCharacteristics));
                            }
                            break;
                        case "diplomacy":
                            __instance.diplomacy++;
                            if (StaticStuff.GetIsTracked(__instance))
                            {
                                Actor actor = World.world.units.get(__instance.id);
                                LifeLogger logger = actor.gameObject.GetComponent<LifeLogger>();

                                logger?.social_characteristics.Add((World.world.getCurWorldTime(), "diplomacy", DataType.SocialCharacteristics));
                            }
                            break;
                        case "warfare":
                            __instance.warfare++;
                            if (StaticStuff.GetIsTracked(__instance))
                            {
                                Actor actor = World.world.units.get(__instance.id);
                                LifeLogger logger = actor.gameObject.GetComponent<LifeLogger>();

                                logger?.social_characteristics.Add((World.world.getCurWorldTime(), "warfare", DataType.SocialCharacteristics));
                            }
                            break;
                        case "stewardship":
                            __instance.stewardship++;
                            if (StaticStuff.GetIsTracked(__instance))
                            {
                                Actor actor = World.world.units.get(__instance.id);
                                LifeLogger logger = actor.gameObject.GetComponent<LifeLogger>();

                                logger?.social_characteristics.Add((World.world.getCurWorldTime(), "stewardship", DataType.SocialCharacteristics));
                            }
                            break;
                    }
                }
            }
            return false;
        }
        #endregion

        #region Добыча ресурсов
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Actor), nameof(Actor.addToInventory))]
        public static void addToInventory_Postfix(Actor __instance, string pID, int pAmount)
        {
            if (StaticStuff.GetIsTracked(__instance))
            {
                LifeLogger logger = __instance.gameObject.GetComponent<LifeLogger>();

                logger?.received_resources.Add((World.world.getCurWorldTime(), pID, pAmount, DataType.GetResources));
            }
        }
        #endregion

        #region Передача ресурсов городу
        [HarmonyPrefix]
        [HarmonyPatch(typeof(ActorBase), nameof(ActorBase.giveInventoryResourcesToCity))]
        public static void giveInventoryResourcesToCity_Prefix(ActorBase __instance)
        {
            if (__instance.inventory.hasResources() && __instance.city != null && __instance.city.isAlive())
            {
                if (StaticStuff.GetIsTracked(__instance))
                {
                    LifeLogger logger = __instance.gameObject.GetComponent<LifeLogger>();

                    logger?.given_resources.Add((World.world.getCurWorldTime(), string.Join(", ", __instance.inventory.getResources().Values.Select(r => $"{r.id.GetLocal()} - {r.amount}")), DataType.GiveResources));
                }
            }
        }
        #endregion

        #region Смена эпохи
        [HarmonyPrefix]
        [HarmonyPatch(typeof(EraManager), "setEra", new System.Type[] { typeof(EraAsset), typeof(bool) })]
        public static void setEra_Prefix(EraAsset pAsset, bool pOverrideTime = true)
        {
            foreach (var actor in World.world.units.getSimpleList())
            {
                if (StaticStuff.GetIsTracked(actor))
                {
                    LifeLogger logger = actor.gameObject.GetComponent<LifeLogger>();

                    logger?.changing_eras.Add((World.world.getCurWorldTime(), pAsset.id, DataType.NewEra));
                }
            }
        }
        #endregion

        #region Городская работа
        // Начало городской работы
        [HarmonyPrefix]
        [HarmonyPatch(typeof(ActorBase), nameof(ActorBase.setCitizenJob))]
        public static void setCitizenJob_Prefix(ActorBase __instance, CitizenJobAsset pJobAsset)
        {
            if (StaticStuff.GetIsTracked(__instance))
            {
                LifeLogger logger = __instance.gameObject.GetComponent<LifeLogger>();

                logger?.citizen_job_starts.Add((World.world.getCurWorldTime(), pJobAsset.id, DataType.CitizenJobStart));
            }
        }

        // Окончание городской работы
        [HarmonyPrefix]
        [HarmonyPatch(typeof(ActorBase), nameof(ActorBase.endJob))]
        public static void endJob_Prefix(ActorBase __instance)
        {
            if (__instance.citizen_job != null)
            {
                if (StaticStuff.GetIsTracked(__instance))
                {
                    LifeLogger logger = __instance.gameObject.GetComponent<LifeLogger>();

                    logger?.citizen_job_ends.Add((World.world.getCurWorldTime(), __instance.citizen_job.id, DataType.CitizenJobEnd));
                }
            }
        }
        #endregion

        #region Постройка зданий
        [HarmonyPrefix]
        [HarmonyPatch(typeof(BehBuildTargetProgress), nameof(BehBuildTargetProgress.execute))]
        public static void execute_BehBuildTargetProgress_Prefix(Actor pActor)
        {
            if (StaticStuff.GetIsTracked(pActor))
            {
                var beh_building_target = (Building)Reflection.GetField(typeof(ActorBase), pActor, "beh_building_target");

                if (!beh_building_target.isUnderConstruction())
                {
                    LifeLogger logger = pActor.gameObject.GetComponent<LifeLogger>();

                    logger?.builded_construction.Add((World.world.getCurWorldTime(), ((BuildingAsset)Reflection.GetField(typeof(Building), beh_building_target, "asset")).id, DataType.BuildedConstruction));
                }
            }
        }
        #endregion

        #region Уборка руин
        [HarmonyPostfix]
        [HarmonyPatch(typeof(BehRemoveRuins), nameof(BehRemoveRuins.execute))]
        public static void execute_BehRemoveRuins_Postfix(Actor pActor)
        {
            if (StaticStuff.GetIsTracked(pActor))
            {
                var beh_building_target = (Building)Reflection.GetField(typeof(ActorBase), pActor, "beh_building_target");

                LifeLogger logger = pActor.gameObject.GetComponent<LifeLogger>();

                logger?.cleaned_construction.Add((World.world.getCurWorldTime(), ((BuildingAsset)Reflection.GetField(typeof(Building), beh_building_target, "asset")).id, DataType.CleanedConstruction));
            }
        }
        #endregion

        #region Добыча ресурсов из строения
        [HarmonyPrefix]
        [HarmonyPatch(typeof(BehExtractResourcesFromBuilding), nameof(BehExtractResourcesFromBuilding.execute))]
        public static void execute_BehExtractResourcesFromBuilding_Prefix(Actor pActor)
        {
            if (StaticStuff.GetIsTracked(pActor))
            {
                var beh_building_target = (Building)Reflection.GetField(typeof(ActorBase), pActor, "beh_building_target");

                LifeLogger logger = pActor.gameObject.GetComponent<LifeLogger>();

                logger?.extract_resources.Add((World.world.getCurWorldTime(), ((BuildingAsset)Reflection.GetField(typeof(Building), beh_building_target, "asset")).id, DataType.ExtractResources));
            }
        }
        #endregion

        #region Постройка дороги
        [HarmonyPostfix]
        [HarmonyPatch(typeof(BehCityCreateRoad), nameof(BehCityCreateRoad.execute))]
        public static void execute_Postfix(Actor pActor)
        {
            if (StaticStuff.GetIsTracked(pActor))
            {
                WorldTile beh_tile_target = (WorldTile)Reflection.GetField(typeof(ActorBase), pActor, "beh_tile_target");

                LifeLogger logger = pActor.gameObject.GetComponent<LifeLogger>();

                logger?.create_road.Add((World.world.getCurWorldTime(), $"X: {beh_tile_target.x}, Y: {beh_tile_target.y}", DataType.CreateRoad));
            }
        }
        #endregion

        #region Выпадение за границу мира
        [HarmonyPrefix]
        [HarmonyPatch(typeof(Actor), "checkDeathOutsideMap")]
        public static void checkDeathOutsideMap_Prefix(Actor __instance, Actor pActor)
        {
            if (StaticStuff.GetIsTracked(__instance))
            {
                if (!__instance.inMapBorder())
                {
                    LifeLogger logger = __instance.gameObject.GetComponent<LifeLogger>();

                    if (logger?.dead_reason == DeadReason.Null)
                    {
                        logger.dead_reason = DeadReason.WorldBorder;
                    }
                }
            }
        }
        #endregion

        #region Смерть от земли
        [HarmonyPrefix]
        [HarmonyPatch(typeof(Actor), nameof(Actor.checkDieOnGround))]
        public static void checkDieOnGround_Prefix(Actor __instance)
        {
            if (StaticStuff.GetIsTracked(__instance))
            {
                if (!__instance.currentTile.Type.liquid && __instance.isAlive() && !(bool)__instance.CallMethod("isInMagnet"))
                {
                    LifeLogger logger = __instance.gameObject.GetComponent<LifeLogger>();

                    if (logger?.dead_reason == DeadReason.Null)
                    {
                        logger.dead_reason = DeadReason.Ground;
                    }
                }
            }
        }
        #endregion

        #region Смерть от старости
        // Вызывает ошибку, так как не может найти поле rase
        /*[HarmonyPrefix]
          [HarmonyPatch(typeof(Actor), "updateAge")]
          public static bool updateAge_Prefix(Actor __instance, ref ActorData ___data, ref BaseStats ___stats)
          {
              if (!(bool)___data.CallMethod("updateAge", (Race)Reflection.GetField(typeof(ActorBase), __instance, "rase"), __instance.asset, ___stats[S.max_age]) && !__instance.hasTrait("immortal"))
              {
                  if (StaticStuff.GetIsTracked(__instance))
                  {
                      LifeLogger logger = __instance.gameObject.GetComponent<LifeLogger>();

                      if (logger?.dead_reason == DeadReason.Null)
                      {
                          logger.dead_reason = DeadReason.OldAge;
                      }
                  }
                  __instance.killHimself(false, AttackType.Age, true, true, true);
                  return true;
              }
              if (__instance.city != null)
              {
                  if (__instance.isKing())
                  {
                      __instance.CallMethod("addExperience", 20);
                  }
                  if (__instance.isCityLeader())
                  {
                      __instance.CallMethod("addExperience", 10);
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
          }*/
        #endregion

        #region Смерть от трансформации
        [HarmonyPrefix]
        [HarmonyPatch(typeof(ActionLibrary), nameof(ActionLibrary.removeUnit))]
        public static void removeUnit_Prefix(Actor pActor)
        {
            if (StaticStuff.GetIsTracked(pActor))
            {
                LifeLogger logger = pActor.gameObject.GetComponent<LifeLogger>();

                if (logger?.dead_reason == DeadReason.Null)
                {
                    logger.dead_reason = DeadReason.Transformation;
                }
            }
        }
        #endregion
    }
}
