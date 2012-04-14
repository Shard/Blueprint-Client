using System;
using System.Collections.Generic;

namespace Blueprint.Fluid
{
    class FluidWater : Fluid
    {

        public FluidWater()
        {
            MaxSkip = 5;
            CurrentSkip = 0;
        }

    }
}
