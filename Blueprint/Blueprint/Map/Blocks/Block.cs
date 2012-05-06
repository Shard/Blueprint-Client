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
        public Rectangle[] Slices;

        public BlockType( string name, int id, int health )
        {
            Name = name;
            Id = id;
            Health = health;
            Slices = new Rectangle[8];
        }

    }

    class BlockFrame
    {
        public byte Index;
        public float Rotate;

        public BlockFrame(byte index, float rotate)
        {
            Index = index;
            Rotate = (float)(rotate * (Math.PI / 180));
        }
    }
}
