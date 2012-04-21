using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Blueprint
{

    class Item
    {

        public int Stack;
        public bool IsUnlimited;
        public ItemType Type;

        public Item(ItemType itemType, int stack, bool isUnlimited)
        {
            Type = itemType;
            Stack = stack;
            IsUnlimited = isUnlimited;
        }

        public Item(ItemType itemType, int stack)
        {
            Type = itemType;
            Stack = stack;
            IsUnlimited = false;
        }
        public Item(ItemType itemType)
        {
            Type = itemType;
            Stack = 1;
            IsUnlimited = false;
        }
    }


}
