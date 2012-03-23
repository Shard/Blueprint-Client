using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Blueprint
{
    class DroppedItem
    {

        public bool Falling;
        public Vector2 Location;
        public Item Item;

        public DroppedItem(Vector2 location, Item item)
        {
            Falling = true;
            Location = location;
            Item = item;
        }

    }
}
