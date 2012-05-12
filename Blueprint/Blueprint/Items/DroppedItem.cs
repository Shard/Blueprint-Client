using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Blueprint
{
    class DroppedItem
    {
        /// <summary>
        /// The item that will be picked up
        /// </summary>
        public Item Item;

        /// <summary>
        /// Handles movement
        /// </summary>
        public Movement Movement;

        /// <summary>
        /// When this counter reaches 0, movement update will not be called
        /// </summary>
        public byte SettleCounter;

        public DroppedItem(Vector2 location, Item item)
        {
            Item = item;
            Movement = new Movement(location, 16, 16, .15f);
            SettleCounter = 30;
        }

    }
}
