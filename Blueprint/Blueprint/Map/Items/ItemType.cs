using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace Blueprint
{
    class ItemType
    {

        // Meta Informaiton
        public string Name;
        public string Description;
        public int Stacksize;
        public Rectangle Location;

        // Item Uses
        public string Use;


        public ItemType(string name, Rectangle location)
        {

            Name = name;
            Description = "A default description";
            Stacksize = 255;
            Location = location;

            Use = "block:1";
        }

        public ItemType(string name, Rectangle location, BlockType placeableBlock)
        {
            Name = name;
            Description = "A default description";
            Stacksize = 255;
            Location = location;

            Use = "block:" + placeableBlock.Id;
        }

        public ItemType(string name, Rectangle location, Weapon weapon)
        {
            Name = name;
            Description = "A default description";
            Stacksize = 255;
            Location = location;

            Use = "weapon:" + weapon.Id;
        }

        public ItemType(string name, Rectangle location, string raw_use)
        {
            Name = name;
            Description = "A default description";
            Stacksize = 255;
            Location = location;
            Use = raw_use;
        }
    }
}
