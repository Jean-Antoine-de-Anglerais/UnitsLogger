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
    }
}
