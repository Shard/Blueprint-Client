using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Blueprint
{

    class ItemCollection
    {

        public ItemType[] ItemTypes;
        public Texture2D ItemTexture;

        public void Initialize(Texture2D itemTexture)
        {

            ItemTexture = itemTexture;

        }

        public void mock()
        {

            ItemTypes = new ItemType[10];

            ItemTypes[0] = new ItemType("Sword", new Rectangle(0,0,40,40));
            ItemTypes[1] = new ItemType("Food", new Rectangle(40, 0, 40, 40));
            ItemTypes[2] = new ItemType("Stone", new Rectangle(80, 0, 40, 40));
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

    class Item
    {

        public int Stack;
        public bool IsUnlimited;
        public ItemType Type;

        public Item(ItemType itemType, int stack, bool isUnlimited)
        {
            Type = itemType;
            Stack = stack;
            IsUnlimited = isUnlimited;
        }

        public Item(ItemType itemType, int stack)
        {
            Type = itemType;
            Stack = stack;
            IsUnlimited = false;
        }
        public Item(ItemType itemType)
        {
            Type = itemType;
            Stack = 1;
            IsUnlimited = false;
        }
    }

    class ItemType
    {

        // Meta Informaiton
        public string Name;
        public string Description;
        public int Stacksize;
        public Rectangle Location;

        // Placeable Uses
        public string PlaceableBlock;
        public string PlaceableItem;
        
        // Weapon Uses
        public bool IsSword;
        public bool IsBow;
        public bool IsGun;


        public ItemType(string name, Rectangle location)
        {

            Name = name;
            Description = "A default description";
            Stacksize = 255;
            Location = location;

            PlaceableBlock = null;
            PlaceableItem = null;

            IsSword = false;
            IsBow = false;
            IsGun = false;
             
        }
    }
}
