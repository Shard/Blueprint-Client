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
        public Rectangle Sprite;
        public Rectangle AltSprite;
        public int Width;
        public int Height;
        public List<Event> Events;

        public int Health;
        public bool Solid;

        public EntityType(int id, string name, Rectangle sprite, bool solid, int width, int height)
        {
            Id = id;
            Name = name;
            Description = "Stub";
            Sprite = sprite;
            AltSprite = new Rectangle(sprite.X + sprite.Width, sprite.Y, sprite.Width, sprite.Height);
            Solid = solid;
            Width = width;
            Height = height;
            Health = 100;
        }

    }
}
