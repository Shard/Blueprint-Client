using System;
using System.Collections.Generic;

namespace Blueprint
{
    class ActiveNpc
    {

        public Npc Npc;
        public int Health;
        public string Name;

        public ActiveNpc(Npc npc)
        {
            Npc = npc;
            Name = Npc.Name;
            Health = Npc.Health;
        }

    }
}
