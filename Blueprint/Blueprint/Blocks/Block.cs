using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Blueprint
{
    class Block
    {

        public BlockType Type;
        public int Health;

        public Block( BlockType type )
        {
            Type = type;
            Health = type.Health;
        }

    }

    class BlockType
    {

        public string Name;
        public int Id; // The unique ID used on the website
        public int Health;
        public Rectangle Location;

        public BlockType( string name, int id, Rectangle location, int health )
        {
            Name = name;
            Id = id;
            Location = location;
            Health = health;
        }

    }
}
