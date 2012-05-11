using System;
using System.Collections.Generic;

namespace Blueprint.Fluid
{
    class FluidWater : Fluid
    {

        public FluidWater()
        {
            MaxSkip = 2;
            MaxMove = 4;
            MaxEmptyMove = 6;
            MaxDownMove = 24;
            CurrentSkip = 0;
        }

    }
}
