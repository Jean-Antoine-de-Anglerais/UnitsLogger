using BepInEx;

namespace UnitsLogger_BepInEx
{
    public class Patches
    {
        #region Переключить отслеживаемость юнита
        public static void click_Prefix()
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

        #region Черты, которые юнит получил
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
        // Token: 0x06000DEA RID: 3562 RVA: 0x0008D334 File Offset: 0x0008B534
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
        /*public static bool produceNewCitizen(CityBehProduceUnit __instance, ref bool __result, ref List<Actor> ____possibleParents, Building pBuilding, City pCity)
          {
              Actor actor = ____possibleParents.Pop<Actor>();
              if (actor == null)
              {
                  __result = false;
                  return true;
              }
              if (!Toolbox.randomChance(((BaseStats)Reflection.GetField(actor.GetType(), actor, "stats"))[S.fertility]))
              {
                  __result = false;
                  return true;
              }
              Actor actor2 = null;
              if (____possibleParents.Count > 0)
              {
                  actor2 = ____possibleParents.Pop<Actor>();
              }
              ResourceAsset foodItem = (ResourceAsset)pCity.CallMethod("getFoodItem", null);
              pCity.CallMethod("eatFoodItem", foodItem.id);
              pCity.status.housingFree--;
              pCity.data.born++;
              Kingdom kingdom = (Kingdom)Reflection.GetField(pCity.GetType(), pCity, "kingdom");
              if (kingdom != null)
              {
                  kingdom.data.born++;
              }
              ActorAsset asset = actor.asset;
              ActorData actorData = new ActorData();
              MapBox world = MapBox.instance;
              actorData.created_time = world.getCreationTime();
              actorData.cityID = pCity.data.id;
              actorData.id = world.mapStats.getNextId("unit");
              actorData.asset_id = asset.id;
              Race race = (Race)Reflection.GetField(actor.GetType(), actor, "race");
              ActorBase.generateCivUnit(actor.asset, actorData, race);
              actorData.generateTraits(asset, race);
              ActorData data1 = (ActorData)Reflection.GetField(actor.GetType(), actor, "data");
              actorData.CallMethod("inheritTraits", data1.traits);
              actorData.hunger = asset.maxHunger / 2;
              data1.makeChild(world.getCurWorldTime());
              ActorData data2 = (ActorData)Reflection.GetField(actor2.GetType(), actor2, "data");
              if (actor2 != null)
              {
                  actorData.CallMethod("inheritTraits", data2.traits);
                  data2.makeChild(world.getCurWorldTime());
              }
              Clan clan = (Clan)Reflection.CallStaticMethod(typeof(CityBehProduceUnit), "checkGreatClan", actor, actor2);
              actorData.skin = ActorTool.getBabyColor(actor, actor2);
              actorData.skin_set = data1.skin_set;
              Culture babyCulture = (Culture)Reflection.CallStaticMethod(typeof(CityBehProduceUnit), "getBabyCulture", actor, actor2);
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
              __result = true;
              return false;
          }

          public static void makeBaby(Actor pActor1, Actor pActor2)
          {
              pActor1.startBabymakingTimeout();
              pActor2.startBabymakingTimeout();
              pActor1.data.children++;
              pActor2.data.children++;
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
              Actor actor = BehaviourActionBase<Actor>.world.units.createNewUnit(pStatsID, worldTile, 6f);
              actor.justBorn();
              actor.data.hunger = 79;
              if (actor.asset.useSkinColors)
              {
                  if (Toolbox.randomBool())
                  {
                      actor.data.skin_set = pActor1.data.skin_set;
                  }
                  else
                  {
                      actor.data.skin_set = pActor2.data.skin_set;
                  }
                  actor.data.skin = ActorTool.getBabyColor(pActor1, pActor2);
              }
              BehaviourActionBase<Actor>.world.gameStats.data.creaturesBorn++;
          }

          public static void makeEgg_Prefix(Actor pActor)
          {
              if (StaticStuff.GetIsTracked(pActor))
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
                  LifeLogger logger = pActor.gameObject.GetComponent<LifeLogger>();

                  logger?.born_children.Add((World.world.getCurWorldTime(), actor.getName(), DataType.Children));
                  return;
              }
          }*/
        #endregion

        #region Поедание еды
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

        #region Выпадение за границу мира
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
        public static bool updateAge_Prefix(Actor __instance, ref ActorData ___data, ref Race ___rase, ref BaseStats ___stats)
        {
            if (!(bool)___data.CallMethod("updateAge", ___rase, __instance.asset, ___stats[S.max_age]) && !__instance.hasTrait("immortal"))
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
        }
        #endregion

        #region Смерть от трансформации
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
