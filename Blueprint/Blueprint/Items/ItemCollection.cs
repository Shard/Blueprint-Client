using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Blueprint
{
    class ItemCollection
    {

        public int Size;
        public Item[] Items;

        public ItemCollection(int size)
        {
            Size = size;
            Items = new Item[size];
        }

        /// <summary>
        /// Attempts to add an item to the collection, will attempt to stack
        /// </summary>
        /// <returns>If Item addded successfully the will return, otherwise false will return</returns>
        public bool AddItem(Item item)
        {

            // Try and stack first
            for (int i = 0; i < Items.Length; i++)
            {
                if (Items[i] != null && Items[i].Type == item.Type && Items[i].Stack < Items[i].Type.Stacksize)
                    { Items[i].Stack += 1; return true; }
            }

            // No stack matches, find an empty spot
            for (int i = 0; i < Items.Length; i++)
            {
                if (Items[i] == null) { Items[i] = item; return true; }
            }


            return false;

        }

    }
}
