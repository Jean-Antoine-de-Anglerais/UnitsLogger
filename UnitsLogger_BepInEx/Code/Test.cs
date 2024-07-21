namespace UnitsLogger_BepInEx
{
    internal class Test //: BehaviourActionCity
    {
        /*private List<Actor> _possibleParents = new List<Actor>();

        private bool unitProduced;

        public override BehResult execute(City pCity)
        {
            if (!BehaviourActionBase<City>.world.worldLaws.world_law_civ_babies.boolVal)
            {
                return BehResult.Stop;
            }
            if (!pCity.hasAnyFood())
            {
                return BehResult.Stop;
            }
            if (!isCityCanProduceUnits(pCity))
            {
                return BehResult.Stop;
            }
            unitProduced = false;
            findPossibleParents(pCity);
            if (_possibleParents.Count == 0)
            {
                return BehResult.Stop;
            }
            int pMaxExclusive = pCity.status.population / 7 + 1;
            int num = Toolbox.randomInt(1, pMaxExclusive);
            if (DebugConfig.isOn(DebugOption.CityFastPopGrowth) && num < 100)
            {
                num = 100;
            }
            for (int i = 0; i < num; i++)
            {
                if (_possibleParents.Count == 0)
                {
                    break;
                }
                if (!isCityCanProduceUnits(pCity))
                {
                    break;
                }
                tryToProduceUnit(pCity);
            }
            _possibleParents.Clear();
            if (unitProduced)
            {
                return BehResult.Continue;
            }
            return BehResult.Stop;
        }

        private bool isCityCanProduceUnits(City pCity)
        {
            if (pCity.status.housingFree == 0)
            {
                return false;
            }
            if (BehaviourActionBase<City>.world.worldLaws.world_law_civ_limit_population_100.boolVal && pCity.getPopulationTotal() >= 100)
            {
                return false;
            }
            return true;
        }

        private void findPossibleParents(City pCity)
        {
            _possibleParents.Clear();
            List<Actor> simpleList = pCity.units.getSimpleList();
            double num = BehaviourActionBase<City>.world.getCurWorldTime() / 5.0;
            for (int i = 0; i < simpleList.Count; i++)
            {
                Actor actor = simpleList[i];
                if (actor.isAlive() && !(actor.stats[S.fertility] <= 0f) && !(num - actor.data.had_child_timeout / 5.0 < 8.0))
                {
                    _possibleParents.Add(actor);
                }
            }
            _possibleParents.Shuffle();
        }

        private void tryToProduceUnit(City pCity)
        {
            if (pCity.getFoodItem() != null)
            {
                Building buildingType = pCity.getBuildingType(SB.type_house);
                if ((object)buildingType != null && produceNewCitizen(buildingType, pCity))
                {
                    unitProduced = true;
                }
            }
        }

        private bool produceNewCitizen(Building pBuilding, City pCity)
        {
            Actor actor = _possibleParents.Pop();
            if (actor == null)
            {
                return false;
            }
            if (!Toolbox.randomChance(actor.stats[S.fertility]))
            {
                return false;
            }
            Actor actor2 = null;
            if (_possibleParents.Count > 0)
            {
                actor2 = _possibleParents.Pop();
            }
            if (actor2 == null)
            { 
                return false; 
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
            actorData.created_time = BehaviourActionBase<City>.world.getCreationTime();
            actorData.cityID = pCity.data.id;
            actorData.id = BehaviourActionBase<City>.world.mapStats.getNextId("unit");
            actorData.asset_id = asset.id;
            ActorBase.generateCivUnit(actor.asset, actorData, actor.race);
            actorData.generateTraits(asset, actor.race);
            actorData.inheritTraits(actor.data.traits);
            actorData.hunger = asset.maxHunger / 2;
            actor.data.makeChild(BehaviourActionBase<City>.world.getCurWorldTime());
            if (actor2 != null)
            {
                actorData.inheritTraits(actor2.data.traits);
                actor2.data.makeChild(BehaviourActionBase<City>.world.getCurWorldTime());
            }
            Clan clan = checkGreatClan(actor, actor2);
            actorData.skin = ActorTool.getBabyColor(actor, actor2);
            actorData.skin_set = actor.data.skin_set;
            Culture babyCulture = getBabyCulture(actor, actor2);
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
            return true;
        }

        private static Clan checkGreatClan(Actor pParent1, Actor pParent2)
        {
            string text = string.Empty;
            if (string.IsNullOrEmpty(text))
            {
                if (pParent1.isKing())
                {
                    text = pParent1.data.clan;
                }
                else if (pParent2 != null && pParent2.isKing())
                {
                    text = pParent2.data.clan;
                }
            }
            if (string.IsNullOrEmpty(text))
            {
                if (pParent1.isCityLeader() && pParent2 != null && pParent2.isCityLeader())
                {
                    text = ((!Toolbox.randomBool()) ? pParent2.data.clan : pParent1.data.clan);
                }
                else if (pParent1 != null && pParent1.isCityLeader())
                {
                    text = pParent1.data.clan;
                }
                else if (pParent2 != null && pParent2.isCityLeader())
                {
                    text = pParent2.data.clan;
                }
            }
            Clan result = null;
            if (!string.IsNullOrEmpty(text))
            {
                result = BehaviourActionBase<City>.world.clans.get(text);
            }
            return result;
        }

        private static Culture getBabyCulture(Actor pActor1, Actor pActor2)
        {
            string text = pActor1.data.culture;
            string text2 = text;
            if (pActor2 != null)
            {
                text2 = pActor2.data.culture;
            }
            if (string.IsNullOrEmpty(text))
            {
                text = pActor1.city?.data.culture;
            }
            if (string.IsNullOrEmpty(text2) && pActor2 != null)
            {
                text2 = pActor2.city?.data.culture;
            }
            Culture culture = pActor1.currentTile.zone.culture;
            if (culture != null && culture.data.race == pActor1.race.id && Toolbox.randomChance(culture.stats.culture_spread_convert_chance.value))
            {
                text = culture.data.id;
            }
            if (Toolbox.randomBool())
            {
                return BehaviourActionBase<City>.world.cultures.get(text);
            }
            return BehaviourActionBase<City>.world.cultures.get(text2);
        }*/
    }
}
