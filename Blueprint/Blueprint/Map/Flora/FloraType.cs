using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Blueprint
{
    class FloraType
    {

        /// <summary>
        /// Name of the flora
        /// </summary>
        public string Name;

        /// <summary>
        /// The dropped item ascioated
        /// </summary>
        public DroppedItem DroppedItem;

        /// <summary>
        /// The location of the sprite
        /// </summary>
        public Rectangle Sprite;

        /// <summary>
        /// At how many steps of being alive should the flora attempt to evolve
        /// </summary>
        public int GrowAt;

        /// <summary>
        /// The index of the flora that it should evolve to when it reaches its grow.
        /// </summary>
        public int GrowInto;

        public FloraType(string name, Rectangle sprite, DroppedItem droppedItem = null, int growAt = 0, int growTo = 0)
        {
            Name = name;
            Sprite = sprite;
            DroppedItem = droppedItem;
            GrowAt = growAt;
            GrowInto = growTo;
        }

    }
}
