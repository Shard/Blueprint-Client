using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Blueprint
{
    class ActiveNpc
    {

        public Npc Npc;
        public NpcAi Ai;
        public int Health;
        public string Name;
        public Movement Movement;
        public float Invunerable;

        public Stack<Point> CurrentPath;
        public Point CurrentDestination;

        public ActiveNpc(Npc npc, Vector2 location )
        {
            Npc = npc;
            Ai = npc.Ai;
            Name = Npc.Name;
            Health = Npc.Health;
            Movement = new Movement(location, npc.Race.DefaultSprite.Width, npc.Race.DefaultSprite.Height);
            Invunerable = 0;
            CurrentPath = null;
        }

    }
}
