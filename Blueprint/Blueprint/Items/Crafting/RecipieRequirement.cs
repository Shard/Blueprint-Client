using System;
using System.Collections.Generic;

namespace Blueprint
{
    class RecipieRequirement
    {

        public ItemType Type;
        public int Qty;

        public RecipieRequirement(ItemType type, int qty)
        {
            Type = type;
            Qty = qty;
        }

    }
}
