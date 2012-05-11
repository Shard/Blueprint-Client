using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Blueprint
{
    class Flora
    {

        public FloraType Type;

        /// <summary>
        /// The current count the flora is up to
        /// </summary>
        public int GrowUpto;

        public Flora(FloraType type)
        {
            Type = type;
            GrowUpto = 0;
        }

    }
}
