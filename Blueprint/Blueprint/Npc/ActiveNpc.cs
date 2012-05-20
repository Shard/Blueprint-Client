using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Blueprint
{

    /// <summary>
    /// Represents a npc that exists in the world, contains all subclasses neccesary for operation
    /// </summary>
    class ActiveNpc
    {

        /// <summary>
        /// The name of the npc
        /// </summary>
        public string Name;

        /// <summary>
        /// The type of the npc
        /// </summary>
        public NpcType Type;

        /// <summary>
        /// The current ai of the npc
        /// </summary>
        public NpcAi Ai;

        /// <summary>
        /// The current health of the npc (replace me with hitbox)
        /// </summary>
        public int Health;

        /// <summary>
        /// The movement object of the npc
        /// </summary>
        public Movement Movement;

        /// <summary>
        /// old
        /// </summary>
        public float Invunerable;

        #region Pathfinding

        /// <summary>
        /// The stack that represents all of the points the npc must follow to reach its destination
        /// </summary>
        public Stack<Point> CurrentPath;

        /// <summary>
        /// The current destiniation of the npc
        /// </summary>
        public Point CurrentDestination;

        #endregion

        public ActiveNpc(NpcType npc, Vector2 location )
        {
            Type = npc;
            Ai = npc.DefaultAi;
            Name = Type.Name;
            Health = npc.BaseStats.Str * 10;
            Movement = new Movement(location, npc.Race.Animation.CurrentLocation.Width, npc.Race.Animation.CurrentLocation.Height);
            Invunerable = 0;
            CurrentPath = null;
        }

    }
}
