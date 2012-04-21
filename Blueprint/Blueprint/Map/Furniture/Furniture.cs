using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Blueprint
{
    class Furniture
    {

        public int Health;
        public Rectangle Area;
        public FurnitureType Type;

        public Furniture(FurnitureType type, int x, int y)
        {
            Type = type;
            Health = 100;
            Area = new Rectangle(x * 24, y * 24, Type.Width * 24, Type.Height * 24);
        }

    }
}
