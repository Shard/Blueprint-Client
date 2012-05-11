using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Blueprint.Events;

namespace Blueprint
{
    class EntityType
    {

        /// <summary>
        /// The public id of the entity
        /// </summary>
        public int Id;

        /// <summary>
        /// The name of the entity
        /// </summary>
        public string Name;

        /// <summary>
        /// The description of the entity
        /// </summary>
        public string Description;

        /// <summary>
        /// The sprite of the entity
        /// </summary>
        public Texture2D Sprite; // cause im lazy, deal with it

        /// <summary>
        /// Width of the entity in block
        /// </summary>
        public int Width;

        /// <summary>
        /// Height of the entity in block
        /// </summary>
        public int Height;

        /// <summary>
        /// Events
        /// </summary>
        public List<Event> Events;

        /// <summary>
        /// Maximun health
        /// </summary>
        public int Health;

        /// <summary>
        /// The default phase of the entity
        /// </summary>
        public bool Solid;

        public enum EntityFunction
        {
            Door,
            Torch,
            Custom
        }

        public EntityFunction Function;

        /// <summary>
        /// Constuctor for entity types
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="sprite"></param>
        /// <param name="solid"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public EntityType(int id, string name, Texture2D sprite, int width, int height, bool solid, EntityFunction function = EntityFunction.Custom)
        {
            Id = id;
            Name = name;
            Description = "Stub";
            Sprite = sprite;
            //AltSprite = new Rectangle(sprite.X + sprite.Width, sprite.Y, sprite.Width, sprite.Height);
            Solid = solid;
            Width = width;
            Height = height;
            Health = 100;
            Function = function;
        }

    }
}
