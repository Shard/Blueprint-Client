using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Blueprint
{
    class FurnitureType
    {

        public int Id;
        public string Name;
        public string Description;
        public Rectangle Sprite;
        public int Width;
        public int Height;

        public int Health;
        public bool Solid;

        public FurnitureType(int id, string name, Rectangle sprite, bool solid, int width, int height)
        {
            Id = id;
            Name = name;
            Description = "Stub";
            Sprite = sprite;
            Solid = solid;
            Width = width;
            Height = height;
        }

    }
}
