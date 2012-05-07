using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Blueprint
{
    class DroppedItem
    {

        public Vector2 Location;
        public Item Item;
        public Movement Movement;

        public DroppedItem(Vector2 location, Item item)
        {
            Location = location;
            Item = item;
            Movement = new Movement(location, 16, 16);
        }

        public bool AttemptPickup(Player player)
        {
            if (Rectangle.Intersect(Movement.Area, player.Movement.Area) != Rectangle.Empty)
            {
                // Pickup Item
                return player.Inventory.Pickup(Item);
            }
            return false;
        }

    }
}
