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
        public int Health;
        public Rectangle Location;


        public BlockType( string name, Rectangle location, int health )
        {
            Name = name;
            Location = location;
            Health = health;
        }

    }
}
