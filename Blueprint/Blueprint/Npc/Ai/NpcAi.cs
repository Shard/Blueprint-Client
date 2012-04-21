using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blueprint
{
    abstract class NpcAi
    {
        public string Name;

        public abstract void Update(Movement movement, Player player);
    }
}
