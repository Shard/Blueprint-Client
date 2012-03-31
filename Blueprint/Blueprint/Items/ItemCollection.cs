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

    }
}
