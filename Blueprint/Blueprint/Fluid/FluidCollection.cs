using System;
using System.Collections.Generic;

namespace Blueprint.Fluid
{
    class FluidCollection
    {

        public Fluid Water;
        
        public FluidCollection()
        {
            Water = new FluidWater();
        }

        public void Initialize(int width, int height)
        {
            Water.Initialize(width, height);
        }

        public void Update()
        {

        }

        public void Draw()
        {

        }

    }
}
