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
        public BlockType PlaceableBlock;
        public string PlaceableItem;
        public Weapon Weapon;


        public ItemType(string name, Rectangle location)
        {

            Name = name;
            Description = "A default description";
            Stacksize = 255;
            Location = location;

            PlaceableBlock = null;
            PlaceableItem = null;
            Weapon = null;
        }

        public ItemType(string name, Rectangle location, BlockType placeableBlock)
        {
            Name = name;
            Description = "A default description";
            Stacksize = 255;
            Location = location;

            PlaceableBlock = placeableBlock;
            PlaceableItem = null;
            Weapon = null;
        }

        public ItemType(string name, Rectangle location, Weapon weapon)
        {
            Name = name;
            Description = "A default description";
            Stacksize = 255;
            Location = location;

            PlaceableBlock = null;
            PlaceableItem = null;
            Weapon = weapon;
        }
    }
}
