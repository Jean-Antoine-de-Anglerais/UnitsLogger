using ai.behaviours;
using BepInEx;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

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
                    DeadLogger.SavingAll(__instance, DeadLogger.Prepare());
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
                if (!__instance.hasTrait(pTrait) && !(AssetManager.traits.get(pTrait) == null) && (pRemoveOpposites || !__instance.hasOppositeTrait(pTrait)))
                {
                    LifeLogger logger = __instance.gameObject.GetComponent<LifeLogger>();
                    logger?.received_traits.Add((World.world.getCurWorldTime(), __instance.GetActorPosition(), pTrait, DataType.ReceivedTraits));
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

                    logger?.received_traits.Add((World.world.getCurWorldTime(), actor.GetActorPosition(), pTrait, DataType.ReceivedTraits));
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

                    logger?.lost_traits.Add((World.world.getCurWorldTime(), __instance.GetActorPosition(), pTraitID, DataType.LostTraits));
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

                    logger?.lost_traits.Add((World.world.getCurWorldTime(), actor.GetActorPosition(), pTraitID, DataType.LostTraits));
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

                logger?.received_professions.Add((World.world.getCurWorldTime(), __instance.GetActorPosition(), pType, DataType.Professions));
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

                    logger?.received_citizenships.Add((World.world.getCurWorldTime(), __instance.GetActorPosition(), pKingdom.data.name, DataType.Сitizenships));
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
                    logger?.received_townships.Add((World.world.getCurWorldTime(), __instance.GetActorPosition(), pCity.data.name, DataType.Townships));
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

                logger?.received_culturships.Add((World.world.getCurWorldTime(), __instance.GetActorPosition(), pCulture.data.name, DataType.Culturships));
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
                        logger.received_names.Add(World.world.getCurWorldTime(), (pName, actor.GetActorPosition()));
                    }
                    else
                    {
                        logger.received_names.Remove(World.world.getCurWorldTime());
                        logger.received_names.Add(World.world.getCurWorldTime(), (pName, actor.GetActorPosition()));
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

                if (pMood != __instance.data.mood)
                {
                    logger?.received_moods.Add((World.world.getCurWorldTime(), __instance.GetActorPosition(), pMood, DataType.Moods));
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
                    logger?.killed_units.Add((World.world.getCurWorldTime(), __instance.GetActorPosition(), $"существо вида {pDeadUnit.asset.id.GetLocal()}, по имени {pDeadUnit.getName()}", DataType.KilledUnits));
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
        /*[HarmonyPrefix]
        [HarmonyPatch(typeof(CityBehProduceUnit), "produceNewCitizen")] // Что-то из этого метода вызывает ошибки (ошибки в блокноте)
        public static bool produceNewCitizen_Prefix(CityBehProduceUnit __instance, ref bool __result, Building pBuilding, City pCity)
        {
            Actor actor = __instance._possibleParents.Pop();
            if (actor == null)
            {
                __result = false;
                return false;
            }
            if (!Toolbox.randomChance(actor.stats[S.fertility]))
            {
                __result = false;
                return false;
            }
            Actor actor2 = null;
            if (__instance._possibleParents.Count > 0)
            {
                actor2 = __instance._possibleParents.Pop();
            }
            ResourceAsset foodItem = pCity.getFoodItem();
            pCity.eatFoodItem(foodItem.id);
            pCity.status.housingFree--;
            pCity.data.born++;
            if (pCity.kingdom != null)
            {
                pCity.kingdom.data.born++;
            }
            ActorAsset asset = actor.asset;
            ActorData actorData = new ActorData();
            actorData.created_time = MapBox.instance.getCreationTime();
            actorData.cityID = pCity.data.id;
            actorData.id = MapBox.instance.mapStats.getNextId("unit");
            actorData.asset_id = asset.id;
            ActorBase.generateCivUnit(actor.asset, actorData, actor.race);
            actorData.generateTraits(asset, actor.race);
            actorData.inheritTraits(actor.data.traits);
            actorData.hunger = asset.maxHunger / 2;
            actor.data.makeChild(MapBox.instance.getCurWorldTime());
            if (actor2 != null)
            {
                actorData.inheritTraits(actor2.data.traits);
                actor2.data.makeChild(MapBox.instance.getCurWorldTime());
            }
            Clan clan = StaticStuff.checkGreatClan(actor, actor2);
            actorData.skin = ActorTool.getBabyColor(actor, actor2);
            actorData.skin_set = actor.data.skin_set;
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
        }*/

        [HarmonyTranspiler]
        [HarmonyPatch(typeof(CityBehProduceUnit), nameof(CityBehProduceUnit.produceNewCitizen))]
        public static IEnumerable<CodeInstruction> produceNewCitizen_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);

            int index = codes.FindLastIndex(instruction => instruction.opcode == OpCodes.Ldc_I4_1);

            if (index == -1)
            {
                Console.WriteLine("produceNewCitizen_Transpiler: index not found");
                return codes.AsEnumerable();
            }

            index--;

            codes.Insert(index + 1, new CodeInstruction(OpCodes.Ldloc_0));
            codes.Insert(index + 2, new CodeInstruction(OpCodes.Ldloc_1));
            codes.Insert(index + 3, new CodeInstruction(OpCodes.Ldloc_S, 4));
            codes.Insert(index + 4, new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(TranspilersContainer), nameof(TranspilersContainer.produceNewCitizen_Transpiler))));

            return codes.AsEnumerable();
        }

        /*[HarmonyPrefix]
        [HarmonyPatch(typeof(BehMakeBaby), "makeBaby")]
        public static bool makeBaby_Prefix(Actor pActor1, Actor pActor2)
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
            Actor actor = MapBox.instance.units.createNewUnit(pStatsID, worldTile, 6f);
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

            pActor1.makeChild(MapBox.instance.getCurWorldTime(), pActor2, actor);

            pActor2.makeChild(MapBox.instance.getCurWorldTime(), pActor1, actor);

            MapBox.instance.gameStats.data.creaturesBorn++;
            return false;
        }*/

        [HarmonyTranspiler]
        [HarmonyPatch(typeof(BehMakeBaby), nameof(BehMakeBaby.makeBaby))]
        public static IEnumerable<CodeInstruction> makeBaby_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);

            int index = codes.FindLastIndex(instruction => instruction.opcode == OpCodes.Ret);

            if (index == -1)
            {
                Console.WriteLine("makeBaby_Transpiler: index not found");
                return codes.AsEnumerable();
            }

            index--;

            codes.Insert(index + 1, new CodeInstruction(OpCodes.Ldarg_1));
            codes.Insert(index + 2, new CodeInstruction(OpCodes.Ldarg_2));
            codes.Insert(index + 3, new CodeInstruction(OpCodes.Ldloc_2));
            codes.Insert(index + 4, new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(TranspilersContainer), nameof(TranspilersContainer.makeBaby_Transpiler))));

            return codes.AsEnumerable();
        }

        /*[HarmonyPrefix]
        [HarmonyPatch(typeof(BehTryBabymaking), "makeEgg")]
        public static bool makeEgg_Prefix(Actor pActor)
        {
            pActor.startBabymakingTimeout();
            pActor.data.children++;
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
                actor.data.skin_set = pActor.data.skin_set;
            }
            if (StaticStuff.GetIsTracked(pActor))
            {
                LifeLogger logger = pActor.gameObject.GetComponent<LifeLogger>();
        
                logger?.born_children.Add((World.world.getCurWorldTime(), actor.GetActorPosition(), actor.getName(), actor.data.gender, DataType.Children));
            }
            return false;
        }*/

        [HarmonyTranspiler]
        [HarmonyPatch(typeof(BehTryBabymaking), nameof(BehTryBabymaking.makeEgg))]
        public static IEnumerable<CodeInstruction> makeEgg_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);

            int index = codes.FindIndex(instruction => instruction.opcode == OpCodes.Stloc_1);

            if (index == -1)
            {
                Console.WriteLine("makeEgg_Transpiler: index not found");
                return codes.AsEnumerable();
            }

            codes.Insert(index + 1, new CodeInstruction(OpCodes.Ldarg_1));
            codes.Insert(index + 2, new CodeInstruction(OpCodes.Ldloc_1));
            codes.Insert(index + 3, new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(TranspilersContainer), nameof(TranspilersContainer.makeEgg_Transpiler))));

            return codes.AsEnumerable();
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

                logger?.eaten_food.Add((World.world.getCurWorldTime(), __instance.GetActorPosition(), pAsset.id, DataType.Food));
            }
        }
        #endregion

        #region Изменение социальных характеристик юнита
        /*[HarmonyPrefix]
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

                                logger?.social_characteristics.Add((World.world.getCurWorldTime(), actor.GetActorPosition(), "intelligence", DataType.SocialCharacteristics));
                            }
                            break;
                        case "diplomacy":
                            __instance.diplomacy++;
                            if (StaticStuff.GetIsTracked(__instance))
                            {
                                Actor actor = World.world.units.get(__instance.id);
                                LifeLogger logger = actor.gameObject.GetComponent<LifeLogger>();

                                logger?.social_characteristics.Add((World.world.getCurWorldTime(), actor.GetActorPosition(), "diplomacy", DataType.SocialCharacteristics));
                            }
                            break;
                        case "warfare":
                            __instance.warfare++;
                            if (StaticStuff.GetIsTracked(__instance))
                            {
                                Actor actor = World.world.units.get(__instance.id);
                                LifeLogger logger = actor.gameObject.GetComponent<LifeLogger>();

                                logger?.social_characteristics.Add((World.world.getCurWorldTime(), actor.GetActorPosition(), "warfare", DataType.SocialCharacteristics));
                            }
                            break;
                        case "stewardship":
                            __instance.stewardship++;
                            if (StaticStuff.GetIsTracked(__instance))
                            {
                                Actor actor = World.world.units.get(__instance.id);
                                LifeLogger logger = actor.gameObject.GetComponent<LifeLogger>();

                                logger?.social_characteristics.Add((World.world.getCurWorldTime(), actor.GetActorPosition(), "stewardship", DataType.SocialCharacteristics));
                            }
                            break;
                    }
                }
            }
            return false;
        }*/

        [HarmonyTranspiler]
        [HarmonyPatch(typeof(ActorData), nameof(ActorData.updateAttributes))]
        public static IEnumerable<CodeInstruction> updateAttributes_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);

            int index_intelligence = codes.FindIndex(instruction => instruction.opcode == OpCodes.Stfld && ((FieldInfo)instruction.operand).Name == "intelligence");
            int index_diplomacy = codes.FindIndex(instruction => instruction.opcode == OpCodes.Stfld && ((FieldInfo)instruction.operand).Name == "diplomacy");
            int index_warfare = codes.FindIndex(instruction => instruction.opcode == OpCodes.Stfld && ((FieldInfo)instruction.operand).Name == "warfare");
            int index_stewardship = codes.FindIndex(instruction => instruction.opcode == OpCodes.Stfld && ((FieldInfo)instruction.operand).Name == "stewardship");


            if (index_intelligence == -1)
            {
                Console.WriteLine("updateAttributes_Transpiler: index_intelligence not found");
                return codes.AsEnumerable();
            }

            if (index_diplomacy == -1)
            {
                Console.WriteLine("updateAttributes_Transpiler: index_diplomacy not found");
                return codes.AsEnumerable();
            }

            if (index_warfare == -1)
            {
                Console.WriteLine("updateAttributes_Transpiler: index_warfare not found");
                return codes.AsEnumerable();
            }

            if (index_stewardship == -1)
            {
                Console.WriteLine("updateAttributes_Transpiler: index_stewardship not found");
                return codes.AsEnumerable();
            }

            codes.Insert(index_intelligence + 1, new CodeInstruction(OpCodes.Ldarg_0));
            codes.Insert(index_intelligence + 2, new CodeInstruction(OpCodes.Ldstr, "intelligence"));
            codes.Insert(index_intelligence + 3, new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(TranspilersContainer), nameof(TranspilersContainer.updateAttributes_Transpiler))));

            codes.Insert(index_diplomacy + 1, new CodeInstruction(OpCodes.Ldarg_0));
            codes.Insert(index_diplomacy + 2, new CodeInstruction(OpCodes.Ldstr, "diplomacy"));
            codes.Insert(index_diplomacy + 3, new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(TranspilersContainer), nameof(TranspilersContainer.updateAttributes_Transpiler))));

            codes.Insert(index_warfare + 1, new CodeInstruction(OpCodes.Ldarg_0));
            codes.Insert(index_warfare + 2, new CodeInstruction(OpCodes.Ldstr, "warfare"));
            codes.Insert(index_warfare + 3, new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(TranspilersContainer), nameof(TranspilersContainer.updateAttributes_Transpiler))));

            codes.Insert(index_stewardship + 1, new CodeInstruction(OpCodes.Ldarg_0));
            codes.Insert(index_stewardship + 2, new CodeInstruction(OpCodes.Ldstr, "stewardship"));
            codes.Insert(index_stewardship + 3, new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(TranspilersContainer), nameof(TranspilersContainer.updateAttributes_Transpiler))));

            return codes.AsEnumerable();
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

                logger?.received_resources.Add((World.world.getCurWorldTime(), __instance.GetActorPosition(), pID, pAmount, DataType.GetResources));
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

                    logger?.given_resources.Add((World.world.getCurWorldTime(), __instance.GetActorPosition(), string.Join(", ", __instance.inventory.getResources().Values.Select(r => $"{r.id.GetLocal()} - {r.amount}")), DataType.GiveResources));
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

                    logger?.changing_eras.Add((World.world.getCurWorldTime(), actor.GetActorPosition(), pAsset.id, DataType.NewEra));
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

                logger?.citizen_job_starts.Add((World.world.getCurWorldTime(), __instance.GetActorPosition(), pJobAsset.id, DataType.CitizenJobStart));
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

                    logger?.citizen_job_ends.Add((World.world.getCurWorldTime(), __instance.GetActorPosition(), __instance.citizen_job.id, DataType.CitizenJobEnd));
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
                if (!pActor.beh_building_target.isUnderConstruction())
                {
                    LifeLogger logger = pActor.gameObject.GetComponent<LifeLogger>();

                    logger?.builded_construction.Add((World.world.getCurWorldTime(), pActor.GetActorPosition(), pActor.beh_building_target.asset.id, DataType.BuildedConstruction));
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
                LifeLogger logger = pActor.gameObject.GetComponent<LifeLogger>();

                logger?.cleaned_construction.Add((World.world.getCurWorldTime(), pActor.GetActorPosition(), pActor.beh_building_target.asset.id, DataType.CleanedConstruction));
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
                LifeLogger logger = pActor.gameObject.GetComponent<LifeLogger>();

                logger?.extract_resources.Add((World.world.getCurWorldTime(), pActor.GetActorPosition(), pActor.beh_building_target.asset.id, DataType.ExtractResources));
            }
        }
        #endregion

        #region Постройка дороги
        [HarmonyPrefix]
        [HarmonyPatch(typeof(BehCityCreateRoad), nameof(BehCityCreateRoad.execute))]
        public static void execute_BehCityCreateRoad_Prefix(Actor pActor)
        {
            if (StaticStuff.GetIsTracked(pActor))
            {
                LifeLogger logger = pActor.gameObject.GetComponent<LifeLogger>();

                logger?.create_road.Add((World.world.getCurWorldTime(), pActor.GetActorPosition(), $"top_type: {pActor.beh_tile_target.top_type.id}, main_type: {pActor.beh_tile_target.main_type.id}, cur_tile_type: {pActor.beh_tile_target.cur_tile_type.id}, Height: {pActor.beh_tile_target.Height}", DataType.CreateRoad));
            }
        }
        #endregion

        #region Работа фермера
        // Вспахивание поля
        [HarmonyPrefix]
        [HarmonyPatch(typeof(BehMakeFarm), nameof(BehMakeFarm.execute))]
        public static void execute_BehMakeFarm_Prefix(Actor pActor)
        {
            if (!pActor.beh_tile_target.Type.can_be_farm)
            {
                return;
            }
            if (pActor.beh_tile_target.building != null && !pActor.beh_tile_target.building.canRemoveForFarms())
            {
                return;
            }

            if (StaticStuff.GetIsTracked(pActor))
            {
                LifeLogger logger = pActor.gameObject.GetComponent<LifeLogger>();

                logger?.make_farm.Add((World.world.getCurWorldTime(), pActor.GetActorPosition(), $"top_type: {pActor.beh_tile_target.top_type.id}, main_type: {pActor.beh_tile_target.main_type.id}, cur_tile_type: {pActor.beh_tile_target.cur_tile_type.id}, Height: {pActor.beh_tile_target.Height}", DataType.MakeFarm));
            }
        }

        // Посадка пшеницы
        [HarmonyPrefix]
        [HarmonyPatch(typeof(BehPlantCrops), nameof(BehPlantCrops.execute))]
        public static void execute_BehPlantCrops_Prefix(Actor pActor)
        {
            if (pActor.beh_tile_target.Type == TopTileLibrary.field && pActor.beh_tile_target.building == null)
            {
                BehaviourActionBase<Actor>.world.buildings.addBuilding(SB.wheat_0, pActor.beh_tile_target, false, false, BuildPlacingType.New);
                MusicBox.playSound("event:/SFX/CIVILIZATIONS/PlantCrops", pActor.beh_tile_target, true, false);
            }
        }

        #endregion

        #region Производство предмета
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(City), nameof(City.produceItem))]
        public static IEnumerable<CodeInstruction> produceItem_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);

            int index = codes.FindLastIndex(instruction => instruction.opcode == OpCodes.Pop);

            if (index == -1)
            {
                Console.WriteLine("produceItem_Transpiler: index not found");
                return codes.AsEnumerable();
            }

            codes.Insert(index + 1, new CodeInstruction(OpCodes.Ldarg_1));
            codes.Insert(index + 2, new CodeInstruction(OpCodes.Ldloc_S, 6));
            codes.Insert(index + 3, new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(TranspilersContainer), nameof(TranspilersContainer.produceItem_Transpiler))));

            return codes.AsEnumerable();
        }
        #endregion

        #region Основание государства
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(BehCheckBuildCity), nameof(BehCheckBuildCity.execute))]
        public static IEnumerable<CodeInstruction> execute_BehCheckBuildCity_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);

            int index = codes.FindLastIndex(instruction => instruction.opcode == OpCodes.Ldc_I4_0 || instruction.Is(OpCodes.Ldc_I4, 0));

            index--;

            if (index == -1)
            {
                Console.WriteLine("execute_BehCheckBuildCity_Transpiler: index not found");
                return codes.AsEnumerable();
            }

            codes.Insert(index + 1, new CodeInstruction(OpCodes.Ldarg_1));
            codes.Insert(index + 2, new CodeInstruction(OpCodes.Ldloc_0));
            codes.Insert(index + 3, new CodeInstruction(OpCodes.Ldloc_1));
            codes.Insert(index + 4, new CodeInstruction(OpCodes.Ldloc_2));
            codes.Insert(index + 5, new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(TranspilersContainer), nameof(TranspilersContainer.execute_BehCheckBuildCity_Transpiler))));

            return codes.AsEnumerable();
        }
        #endregion

        #region Поедание построек
        [HarmonyPrefix]
        [HarmonyPatch(typeof(BehConsumeTargetBuilding), nameof(BehConsumeTargetBuilding.execute))]
        public static void execute_BehConsumeTargetBuilding_Prefix(Actor pActor)
        {
            string type = pActor.beh_building_target.asset.type;
            if (type == SB.type_fruits)
            {
                if (pActor.beh_building_target.hasResources)
                {
                    if (StaticStuff.GetIsTracked(pActor))
                    {
                        LifeLogger logger = pActor.gameObject.GetComponent<LifeLogger>();

                        logger?.eaten_buildings.Add((World.world.getCurWorldTime(), pActor.GetActorPosition(), pActor.beh_building_target.asset.id, DataType.EatenBuildings));
                    }
                }
            }
            else if ((type == SB.type_crops || type == SB.type_flower || type == SB.type_vegetation) && pActor.beh_building_target.isAlive())
            {
                if (StaticStuff.GetIsTracked(pActor))
                {
                    LifeLogger logger = pActor.gameObject.GetComponent<LifeLogger>();

                    logger?.eaten_buildings.Add((World.world.getCurWorldTime(), pActor.GetActorPosition(), pActor.beh_building_target.asset.id, DataType.EatenBuildings));
                }
            }
        }
        #endregion

        #region Добыча ресурсов в шахте
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(BehGetResourcesFromMine), nameof(BehGetResourcesFromMine.execute))]
        public static IEnumerable<CodeInstruction> execute_BehGetResourcesFromMine_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);

            int index = codes.FindLastIndex(instruction => instruction.opcode == OpCodes.Ldc_I4_0 || instruction.Is(OpCodes.Ldc_I4, 0));

            index--;

            if (index == -1)
            {
                Console.WriteLine("execute_BehGetResourcesFromMine_Transpiler: index not found");
                return codes.AsEnumerable();
            }

            codes.Insert(index + 1, new CodeInstruction(OpCodes.Ldarg_1));
            codes.Insert(index + 2, new CodeInstruction(OpCodes.Ldloc_0));
            codes.Insert(index + 3, new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(TranspilersContainer), nameof(TranspilersContainer.execute_BehGetResourcesFromMine_Transpiler))));

            return codes.AsEnumerable();
        }
        #endregion

        #region Полное пополнение голода (для крабов и существ, кормящихся на воде)
        [HarmonyPrefix]
        [HarmonyPatch(typeof(BehReplenishHunger), nameof(BehReplenishHunger.execute))]
        public static void execute_BehReplenishHunger_Prefix(Actor pActor)
        {
            if (StaticStuff.GetIsTracked(pActor))
            {
                LifeLogger logger = pActor.gameObject.GetComponent<LifeLogger>();

                logger?.replenish_hunger.Add((World.world.getCurWorldTime(), pActor.GetActorPosition(), "", DataType.ReplenishHunger));
            }
        }
        #endregion

        #region Каст заклинания спавна скелетов
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(BehMagicMakeSkeleton), nameof(BehMagicMakeSkeleton.execute))]
        public static IEnumerable<CodeInstruction> execute_BehMagicMakeSkeleton_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);

            int index = codes.FindLastIndex(instruction => instruction.opcode == OpCodes.Ldc_I4_0 || instruction.Is(OpCodes.Ldc_I4, 0));
            index--;
            if (index == -1)
            {
                Console.WriteLine("execute_BehMagicMakeSkeleton_Transpiler: index not found");
                return codes.AsEnumerable();
            }

            var containerType = typeof(BehMagicMakeSkeleton).GetNestedType("<>c__DisplayClass0_0", BindingFlags.NonPublic);
            var field = containerType.GetField("tTile", BindingFlags.Instance | BindingFlags.Public);

            codes.Insert(index + 1, new CodeInstruction(OpCodes.Ldloc_0));
            codes.Insert(index + 2, new CodeInstruction(OpCodes.Ldfld, field));
            codes.Insert(index + 3, new CodeInstruction(OpCodes.Ldarg_1));
            codes.Insert(index + 4, new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(TranspilersContainer), nameof(TranspilersContainer.execute_BehMagicMakeSkeleton_Transpiler))));

            return codes.AsEnumerable();
        }
        #endregion

        #region Зарывание краба в песок
        //[HarmonyTranspiler]
        //[HarmonyPatch(typeof(BehCrabBurrow), nameof(BehCrabBurrow.execute))]
        public static IEnumerable<CodeInstruction> execute_BehCrabBurrow_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);

            int index_Stop_1 = codes.FindIndex(instruction => instruction.opcode == OpCodes.Ldc_I4_1 || instruction.Is(OpCodes.Ldc_I4, 1));
            int index_Stop_2 = codes.FindLastIndex(instruction => instruction.opcode == OpCodes.Ldc_I4_1 || instruction.Is(OpCodes.Ldc_I4, 1));
            int index_RepeatStep = codes.FindLastIndex(instruction => instruction.opcode == OpCodes.Ldc_I4_2 || instruction.Is(OpCodes.Ldc_I4, 2));

            index_Stop_1--;
            if (index_Stop_1 == -1)
            {
                Console.WriteLine("updateAttributes_Transpiler: index_Stop_1 not found");
                return codes.AsEnumerable();
            }

            index_Stop_2--;
            if (index_Stop_2 == -1)
            {
                Console.WriteLine("updateAttributes_Transpiler: index_Stop_2 not found");
                return codes.AsEnumerable();
            }

            index_RepeatStep--;
            if (index_RepeatStep == -1)
            {
                Console.WriteLine("updateAttributes_Transpiler: index_RepeatStep not found");
                return codes.AsEnumerable();
            }

            codes.Insert(index_Stop_1 + 1, new CodeInstruction(OpCodes.Ldarg_1));
            codes.Insert(index_Stop_1 + 2, new CodeInstruction(OpCodes.Ldstr, "type_hunger"));
            codes.Insert(index_Stop_1 + 3, new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(TranspilersContainer), nameof(TranspilersContainer.execute_BehCrabBurrow_Transpiler))));

            codes.Insert(index_Stop_2 + 1, new CodeInstruction(OpCodes.Ldarg_1));
            codes.Insert(index_Stop_2 + 2, new CodeInstruction(OpCodes.Ldstr, "type_danger"));
            codes.Insert(index_Stop_2 + 3, new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(TranspilersContainer), nameof(TranspilersContainer.execute_BehCrabBurrow_Transpiler))));

            codes.Insert(index_RepeatStep + 1, new CodeInstruction(OpCodes.Ldarg_1));
            codes.Insert(index_RepeatStep + 2, new CodeInstruction(OpCodes.Ldstr, "type_repeat"));
            codes.Insert(index_RepeatStep + 3, new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(TranspilersContainer), nameof(TranspilersContainer.execute_BehCrabBurrow_Transpiler))));

            return codes.AsEnumerable();
        }
        #endregion

        #region Тушение пожара
        //[HarmonyPrefix]
        //[HarmonyPatch(typeof(BehCityRemoveFire), nameof(BehCityRemoveFire.execute))]
        public static void execute_BehCityRemoveFire_Prefix(Actor pActor)
        {
            TileZone zone = pActor.currentTile.zone;
            if (WorldBehaviourActionFire.countFires(zone) > 0)
            {
                for (int i = 0; i < zone.tiles.Count; i++)
                {
                    WorldTile worldTile = zone.tiles[i];
                    if (worldTile.isOnFire())
                    {
                        worldTile.stopFire();
                    }
                    if (worldTile.building != null)
                    {
                        worldTile.building.stopFire();
                    }
                }
            }
            return;
        }

        #endregion

        #region Телепорт
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(ActionLibrary), nameof(ActionLibrary.teleportRandom))]
        public static IEnumerable<CodeInstruction> teleportRandom_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);

            int index = codes.FindLastIndex(instruction => instruction.opcode == OpCodes.Pop);

            if (index == -1)
            {
                Console.WriteLine("teleportRandom_Transpiler: index not found");
                return codes.AsEnumerable();
            }

            codes.Insert(index + 1, new CodeInstruction(OpCodes.Ldarg_1));
            codes.Insert(index + 2, new CodeInstruction(OpCodes.Ldloc_0));
            codes.Insert(index + 3, new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(TranspilersContainer), nameof(TranspilersContainer.teleportRandom_Transpiler))));

            return codes.AsEnumerable();
        }
        #endregion

        #region Каст комбатных заклинаний
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(CombatActionLibrary), nameof(CombatActionLibrary.tryToCastSpell))]
        public static IEnumerable<CodeInstruction> tryToCastSpell_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);

            int index = codes.FindLastIndex(instruction => instruction.opcode == OpCodes.Callvirt && ((MethodInfo)instruction.operand).Name == "doCastAnimation");

            index--;
            index--;

            if (index == -1)
            {
                Console.WriteLine("tryToCastSpell_Transpiler: index not found");
                return codes.AsEnumerable();
            }

            codes.Insert(index + 1, new CodeInstruction(OpCodes.Ldloc_0));
            codes.Insert(index + 2, new CodeInstruction(OpCodes.Ldloc_1));
            codes.Insert(index + 3, new CodeInstruction(OpCodes.Ldloc_2));
            codes.Insert(index + 4, new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(TranspilersContainer), nameof(TranspilersContainer.tryToCastSpell_Transpiler))));

            return codes.AsEnumerable();
        }
        #endregion

        #region Каст огня для сжигания опухоли
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(BehBurnTumorTiles), nameof(BehBurnTumorTiles.execute))]
        public static IEnumerable<CodeInstruction> execute_BehBurnTumorTiles_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);

            int index = codes.FindLastIndex(instruction => instruction.opcode == OpCodes.Callvirt && ((MethodInfo)instruction.operand).Name == "doCastAnimation");

            if (index == -1)
            {
                Console.WriteLine("execute_BehBurnTumorTiles_Transpiler: index not found");
                return codes.AsEnumerable();
            }

            codes.Insert(index + 1, new CodeInstruction(OpCodes.Ldarg_1));
            codes.Insert(index + 2, new CodeInstruction(OpCodes.Ldloc_0));
            codes.Insert(index + 3, new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(TranspilersContainer), nameof(TranspilersContainer.execute_BehBurnTumorTiles_Transpiler))));

            return codes.AsEnumerable();
        }
        #endregion

        #region Каст удобрения для деревьев
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(BehSpawnTreeFertilizer), nameof(BehSpawnTreeFertilizer.execute))]
        public static IEnumerable<CodeInstruction> execute_BehSpawnTreeFertilizer_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            var codes = new List<CodeInstruction>(instructions);
            LocalBuilder builder = generator.DeclareLocal(typeof(WorldTile));

            int index_new_field = codes.FindLastIndex(instruction => instruction.opcode == OpCodes.Call && ((MethodInfo)instruction.operand).Name == "GetRandom");
            int index = codes.FindLastIndex(instruction => instruction.opcode == OpCodes.Callvirt && ((MethodInfo)instruction.operand).Name == "Invoke");

            if (index == -1)
            {
                Console.WriteLine("execute_BehSpawnTreeFertilizer_Transpiler: index not found");
                return codes.AsEnumerable();
            }


            if (index_new_field == -1)
            {
                Console.WriteLine("execute_BehSpawnTreeFertilizer_Transpiler: index_new_field not found");
                return codes.AsEnumerable();
            }

            codes.Insert(index_new_field + 1, new CodeInstruction(OpCodes.Stloc_S, builder.LocalIndex));
            codes.Insert(index_new_field + 2, new CodeInstruction(OpCodes.Ldloc_S, builder.LocalIndex));

            codes.Insert(index + 1, new CodeInstruction(OpCodes.Ldarg_1));
            codes.Insert(index + 2, new CodeInstruction(OpCodes.Ldloc_S, builder.LocalIndex));
            codes.Insert(index + 3, new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(TranspilersContainer), nameof(TranspilersContainer.execute_BehSpawnTreeFertilizer_Transpiler))));

            return codes.AsEnumerable();
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
                if (!__instance.currentTile.Type.liquid && __instance.isAlive() && !__instance.isInMagnet())
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
