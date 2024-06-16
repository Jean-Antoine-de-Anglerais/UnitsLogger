using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
