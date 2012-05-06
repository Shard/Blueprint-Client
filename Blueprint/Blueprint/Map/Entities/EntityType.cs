using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Blueprint.Events;

namespace Blueprint
{
    class EntityType
    {

        public int Id;
        public string Name;
        public string Description;
        public Texture2D Sprite; // cause im lazy, deal with it
        public int AltSprite;
        public int Width;
        public int Height;
        public List<Event> Events;

        public int Health;
        public bool Solid;

        /// <summary>
        /// Constuctor for entity types
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="sprite"></param>
        /// <param name="solid"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public EntityType(int id, string name, Texture2D sprite, int width, int height, bool solid)
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
        }

    }
}
