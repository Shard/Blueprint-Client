using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Blueprint
{
    class ItemPackage
    {

        public ItemType[] ItemTypes;
        public Texture2D ItemTexture;

        public void Initialize(Texture2D itemTexture)
        {

            ItemTexture = itemTexture;

        }

        public void mock(BlockType[] blockTypes, Weapon[] weapons)
        {
            
            ItemTypes = new ItemType[10];

            ItemTypes[0] = new ItemType("Sword", new Rectangle(0, 0, 40, 40), weapons[0]);
            ItemTypes[1] = new ItemType("Food", new Rectangle(40, 0, 40, 40));
            ItemTypes[2] = new ItemType("Stone", new Rectangle(80, 0, 40, 40), blockTypes[0]);
            ItemTypes[3] = new ItemType("Shirt", new Rectangle(120, 0, 40, 40));
            ItemTypes[4] = new ItemType("Health Potion", new Rectangle(160, 0, 40, 40));
            ItemTypes[5] = new ItemType("Mana Potion", new Rectangle(200, 0, 40, 40));
            ItemTypes[6] = new ItemType("Pants", new Rectangle(240, 0, 40, 40));
            ItemTypes[7] = new ItemType("Gun", new Rectangle(280, 0, 40, 40));
            ItemTypes[8] = new ItemType("Copper Ore", new Rectangle(320, 0, 40, 40));
            ItemTypes[9] = new ItemType("Bow", new Rectangle(360, 0, 40, 40));

        }

        public void Draw(SpriteBatch spriteBatch)
        {

        }

    }
}
