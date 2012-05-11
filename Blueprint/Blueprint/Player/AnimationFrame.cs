using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Blueprint
{

    /// <summary>
    /// Describe a single frame inside of an animation
    /// </summary>
    class AnimationFrame
    {

        public short Width;
        public short Height;
        public short HitboxWidth;
        public short HitboxHeight;
        public short X;
        public short Y;
        public short Time;

        public AnimationFrame(short width, short height, short x, short y, short time)
        {
            Width = width;
            Height = height;
            X = x;
            Y = y;
            Time = time;
            HitboxHeight = height;
            HitboxWidth = width;
        }

        public Rectangle ToRectangle()
        {
            return new Rectangle(X,Y,Width,Height);
        }

    }
}
