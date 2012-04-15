using System;
using System.Collections.Generic;
using System.Xml;

namespace Blueprint
{

    /// <summary>
    /// Used to describe animations which are managed by the Animations class
    /// </summary>
    class Animation
    {

        public string Name;
        public Int16 Time;
        public Int16 Width;
        public Int16 Height;
        public Int16 HitboxHeight;
        public Int16 HitboxWidth;
        public Int16 OffsetX;
        public Int16 OffsetY;
        public AnimationFrame[] Frames;

        public Animation(string name)
        {
            Name = name;
            Time = 1;
        }

    }
}
