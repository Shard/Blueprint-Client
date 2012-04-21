using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blueprint
{
    class Wall
    {
        public byte Health;
        public WallType Type;

        public Wall(WallType type)
        {
            Health = 100;
            Type = type;
        }
    }
}
