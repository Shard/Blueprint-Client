using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Blueprint.Events;
using Krypton.Lights;

namespace Blueprint
{
    class Entity
    {

        /// <summary>
        /// Current health of the entity
        /// </summary>
        public int Health;

        /// <summary>
        /// Determines wether the furniture is a solid collidable object.
        /// </summary>
        public bool Solid;

        /// <summary>
        /// Current Area that the entitiy occupies
        /// </summary>
        public Rectangle Area;

        /// <summary>
        /// The id of the entity type
        /// </summary>
        public int Type;

        /// <summary>
        /// The events binded to the entitiy
        /// </summary>
        public List<Event> Events;

        public Light2D Light;

        public bool AltSprite;


        public Entity(EntityType type, int x, int y)
        {
            Type = type.Id;
            Health = 100;
            Area = new Rectangle(x * 24, y * 24 - ((type.Height - 1) * 24), type.Width * 24, type.Height * 24);
            Solid = type.Solid;
            Events = new List<Event>();
            AltSprite = false;
        }

    }
}
