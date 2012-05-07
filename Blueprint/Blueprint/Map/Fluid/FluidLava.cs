using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blueprint.Fluid
{
    class FluidLava : Fluid
    {

        public FluidLava()
        {
            MaxSkip = 6;
            MaxMove = 1;
            MaxDownMove = 5;
            CurrentSkip = 0;
        }

    }
}
