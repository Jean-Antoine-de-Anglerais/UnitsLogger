﻿using static UnityEngine.GraphicsBuffer;

namespace UnitsLogger_BepInEx
{
    public class TranspilersContainer
    {
        public static void produceItem_Transpiler(Actor actor, ItemData item)
        {
            if (StaticStuff.GetIsTracked(actor))
            {
                LifeLogger logger = actor.gameObject.GetComponent<LifeLogger>();
                ItemDataLogged data_logged = new ItemDataLogged(item);

                logger?.manufactured_items.Add((World.world.getCurWorldTime(), actor.GetActorPosition(), data_logged, DataType.ManufacturedItem));
            }
        }

        public static void makeEgg_Transpiler(Actor parent, Actor baby)
        {
            if (StaticStuff.GetIsTracked(parent))
            {
                LifeLogger logger = parent.gameObject.GetComponent<LifeLogger>();

                logger?.born_children.Add((World.world.getCurWorldTime(), parent.GetActorPosition(), baby.getName(), baby.data.gender, DataType.Children));
            }
        }

        public static void makeBaby_Transpiler(Actor parent1, Actor parent2, Actor baby)
        {
            parent1.makeChild(MapBox.instance.getCurWorldTime(), parent2, baby);

            parent2.makeChild(MapBox.instance.getCurWorldTime(), parent1, baby);
        }

        public static void produceNewCitizen_Transpiler(Actor parent1, Actor parent2, ActorData baby)
        {
            parent1.makeChild(MapBox.instance.getCurWorldTime(), parent2, baby);

            if (parent2 != null)
            {
                parent2.makeChild(MapBox.instance.getCurWorldTime(), parent1, baby);
            }
        }

        public static void updateAttributes_Transpiler(ActorData data, string attribute)
        {
            if (StaticStuff.GetIsTracked(data))
            {
                Actor actor = World.world.units.get(data.id);
                LifeLogger logger = actor.gameObject.GetComponent<LifeLogger>();

                logger?.social_characteristics.Add((World.world.getCurWorldTime(), actor.GetActorPosition(), attribute, DataType.SocialCharacteristics));
            }
        }

        public static void execute_BehCheckBuildCity_Transpiler(Actor actor, City city, TileZone zone, Kingdom kingdom)
        {
            if (StaticStuff.GetIsTracked(actor))
            {
                LifeLogger logger = actor.gameObject.GetComponent<LifeLogger>();

                if (kingdom == null || kingdom.countCities() == 1)
                {
                    logger?.founded_cities.Add((World.world.getCurWorldTime(), actor.GetActorPosition(), true, city.name, actor.kingdom.name, (zone.x, zone.y), DataType.FoundedCities));
                }
                else
                {
                    logger?.founded_cities.Add((World.world.getCurWorldTime(), actor.GetActorPosition(), false, city.name, "", (zone.x, zone.y), DataType.FoundedCities));
                }
            }
        }

        public static void execute_BehGetResourcesFromMine_Transpiler(Actor actor, BuildingAsset building_asset)
        {
            if (building_asset != null)
            {
                if (StaticStuff.GetIsTracked(actor))
                {
                    LifeLogger logger = actor.gameObject.GetComponent<LifeLogger>();

                    logger?.mine_resources.Add((World.world.getCurWorldTime(), actor.GetActorPosition(), building_asset.id, DataType.MineResources));
                }
            }
        }

        public static void execute_BehMagicMakeSkeleton_Transpiler(WorldTile tile, Actor actor)
        {
            if (StaticStuff.GetIsTracked(actor))
            {
                LifeLogger logger = actor.gameObject.GetComponent<LifeLogger>();

                logger?.maked_skeletons.Add((World.world.getCurWorldTime(), actor.GetActorPosition(), (tile.x, tile.y), DataType.MakeSkeleton));
            }
        }

        public static void execute_BehCrabBurrow_Transpiler(Actor actor, string behaviour_type)
        {
            if (StaticStuff.GetIsTracked(actor))
            {
                LifeLogger logger = actor.gameObject.GetComponent<LifeLogger>();

                if (behaviour_type == "type_repeat")
                {
                    logger?.crab_burrow.Add((World.world.getCurWorldTime(), actor.GetActorPosition(), "type_repeat", actor.timer_action, DataType.CrabBurrow));
                }
                else if (behaviour_type == "type_hunger")
                {
                    logger?.crab_burrow.Add((World.world.getCurWorldTime(), actor.GetActorPosition(), "type_hunger", 0, DataType.CrabBurrow));
                }
                else if (behaviour_type == "type_danger")
                {
                    logger?.crab_burrow.Add((World.world.getCurWorldTime(), actor.GetActorPosition(), "type_danger", 0, DataType.CrabBurrow));
                }
            }
        }

        public static void teleportRandom_Transpiler(BaseSimObject actor, WorldTile tile)
        {
            if (StaticStuff.GetIsTracked(actor))
            {
                LifeLogger logger = actor.gameObject.GetComponent<LifeLogger>();

                logger?.teleport_random.Add((World.world.getCurWorldTime(), actor.GetActorPosition(), (tile.x, tile.y), DataType.TeleportRandom));
            }
        }

        public static void tryToCastSpell_Transpiler(Actor initiator, BaseSimObject target, Spell spell)
        {
            if (StaticStuff.GetIsTracked(initiator))
            {
                LifeLogger logger = initiator.gameObject.GetComponent<LifeLogger>();

                logger?.cast_spell.Add((World.world.getCurWorldTime(), initiator.GetActorPosition(), spell.id, target.GetActorPosition(), DataType.CastSpell));
            }
        }
    }
}
