using System;
using System.Collections.Generic;

namespace Blueprint.Fluid
{
    class FluidWater : Fluid
    {

        public FluidWater()
        {
            MaxSkip = 3;
            MaxMove = 1;
            MaxDownMove = 3;
            CurrentSkip = 0;
        }

    }
}
