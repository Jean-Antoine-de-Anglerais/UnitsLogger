using System.Collections.Generic;

namespace UnitsLogger_BepInEx
{
    public class Test : ActorBase
    {
        public static List<long> ms = new List<long>();

        /*public void giveInventoryResourcesToCityFinally()
        {
            if (this.inventory.hasResources() && this.city != null && this.city.isAlive())
            {
                StringBuilder builder = new StringBuilder();

                Dictionary<string, ResourceContainer>.ValueCollection.Enumerator enumerator = this.inventory.getResources().Values.GetEnumerator();
                try
                {
                    while (enumerator.MoveNext())
                    {
                        ResourceContainer value = enumerator.Current;
                        this.city.data.storage.change(value.id, value.amount);

                        builder.Append($"{value.id.GetLocal()} - {value.amount}, ");
                    }
                }
                finally
                {
                    enumerator.Dispose();
                }

                if (StaticStuff.GetIsTracked(this))
                {
                    TranspilersContainer.giveInventoryResourcesToCity_Transpiler(this, ref builder);
                }

                builder = null;
            }

            this.inventory.empty();
            if (!this.asset.use_items)
                return;
            this.dirty_sprite_item = true;
        }

        public void giveInventoryResourcesToCityLowWithChecks()
        {
            if (this.inventory.hasResources() && this.city != null && this.city.isAlive())
            {
                StringBuilder builder = new StringBuilder();         

                Dictionary<string, ResourceContainer>.ValueCollection.Enumerator enumerator = this.inventory.getResources().Values.GetEnumerator();
                try
                {
                    if (this.GetIsTracked())
                    {
                        while (enumerator.MoveNext())
                        {
                            ResourceContainer value = enumerator.Current;
                            this.city.data.storage.change(value.id, value.amount);

                            builder.Append($"{value.id.GetLocal()} - {value.amount}, ");
                        }
                    }

                    else
                    {
                        while (enumerator.MoveNext())
                        {
                            ResourceContainer value = enumerator.Current;
                            this.city.data.storage.change(value.id, value.amount);

                            builder.Append($"{value.id.GetLocal()} - {value.amount}, ");
                        }
                    }
                }
                finally
                {
                    enumerator.Dispose();
                }

                TranspilersContainer.giveInventoryResourcesToCity_Transpiler(this, ref builder);
            }
            this.inventory.empty();
            if (!this.asset.use_items)
                return;
            this.dirty_sprite_item = true;
        }

        public void giveInventoryResourcesToCityLow()
        {
            if (this.inventory.hasResources() && this.city != null && this.city.isAlive())
            {
                if (this.GetIsTracked())
                {
                    StringBuilder builder = new StringBuilder();

                    Dictionary<string, ResourceContainer>.ValueCollection.Enumerator enumerator = this.inventory.getResources().Values.GetEnumerator();
                    try
                    {
                        while (enumerator.MoveNext())
                        {
                            ResourceContainer value = enumerator.Current;
                            this.city.data.storage.change(value.id, value.amount);

                            builder.Append($"{value.id.GetLocal()} - {value.amount}, ");
                        }
                    }
                    finally
                    {
                        enumerator.Dispose();
                    }

                    TranspilersContainer.giveInventoryResourcesToCity_Transpiler(this, ref builder);
                }

                else
                {
                    Dictionary<string, ResourceContainer>.ValueCollection.Enumerator enumerator = this.inventory.getResources().Values.GetEnumerator();
                    try
                    {
                        while (enumerator.MoveNext())
                        {
                            ResourceContainer value = enumerator.Current;
                            this.city.data.storage.change(value.id, value.amount);
                        }
                    }
                    finally
                    {
                        enumerator.Dispose();
                    }
                }
            }
            this.inventory.empty();
            if (!this.asset.use_items)
                return;
            this.dirty_sprite_item = true;
        }

        public void giveInventoryResourcesToCityHigh()
        {
            if (this.GetIsTracked())
            {
                if (this.inventory.hasResources() && this.city != null && this.city.isAlive())
                {
                    StringBuilder builder = new StringBuilder();

                    foreach (ResourceContainer value in this.inventory.getResources().Values)
                    {
                        this.city.data.storage.change(value.id, value.amount);

                        builder.Append($"{value.id.GetLocal()} - {value.amount}, ");
                    }

                    TranspilersContainer.giveInventoryResourcesToCity_Transpiler(this, ref builder);
                }
            }

            else
            {
                if (this.inventory.hasResources() && this.city != null && this.city.isAlive())
                {
                    foreach (ResourceContainer value in this.inventory.getResources().Values)
                    {
                        this.city.data.storage.change(value.id, value.amount);
                    }
                }
            }

            this.inventory.empty();
            if (this.asset.use_items)
            {
                this.dirty_sprite_item = true;
            }
        }*/
    }
}
