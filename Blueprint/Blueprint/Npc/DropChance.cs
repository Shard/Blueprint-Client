using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blueprint
{
    class DropChance
    {

        public Item Item;
        public float Chance; // Chance out of 100 of dropping the item
        public int MaxDrop; // The amount of items that can be dropped

        public DropChance(Item item)
        {
            Item = item;
            Chance = 100f;
            MaxDrop = 1;
        }

        public DropChance(Item item, float chance)
        {
            Item = item;
            Chance = chance;
            MaxDrop = 1;
        }

        public DropChance(Item item, float chance, int maxDrop)
        {
            Item = item;
            Chance = chance;
            MaxDrop = maxDrop;
        }

    }
}
