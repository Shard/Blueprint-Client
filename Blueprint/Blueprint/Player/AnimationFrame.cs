using System;
using System.Collections.Generic;

namespace Blueprint
{

    /// <summary>
    /// Describe a single frame inside of an animation
    /// </summary>
    class AnimationFrame
    {

        public Int16 Width;
        public Int16 Height;
        public Int16 HitboxWidth;
        public Int16 HitboxHeight;
        public Int16 X;
        public Int16 Y;
        public Int16 Time;

        public AnimationFrame(Int16 width, Int16 height, Int16 x, Int16 y, Int16 time)
        {
            Width = width;
            Height = height;
            X = x;
            Y = y;
            Time = time;
            HitboxHeight = height;
            HitboxWidth = width;
        }

    }
}
