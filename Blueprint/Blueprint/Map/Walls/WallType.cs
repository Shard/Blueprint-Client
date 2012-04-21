using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Blueprint
{
    class WallType
    {

        /// <summary>
        /// Unique intiger that links up with the blueprint database
        /// </summary>
        public int Id;

        /// <summary>
        /// Location on the spritesheet
        /// </summary>
        public Rectangle Sprite;

        public string Name;

        public WallType(int id, string name, Rectangle sprite)
        {
            Id = id;
            Name = name;
            Sprite = sprite;
        }

    }
}
