using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Blueprint
{
    class Inventory
    {

        public Texture2D InventoryTexture;

        // Player owned item management
        public Item[] Bag; // Items in the players bag
        public Quickbar Quickbar; // Just one quickbar
        public Item[] Bank; // Items in the players bank

        // Money
        public int Money;
        public int BankedMoney;

        public Inventory()
        {

            Bag = new Item[40];
            Quickbar = new Quickbar(10);
            Bank = new Item[80];
            Money = 99999;
            BankedMoney = 12345;
        }

        public Inventory(int bagSize, int quickbarSize, int bankSize, int money, int bankedMoney)
        {

            Bag = new Item[bagSize];
            Quickbar = new Quickbar(quickbarSize);
            Bank = new Item[bankSize];
            Money = money;
            BankedMoney = bankedMoney;
        }

        public void Initialize(Texture2D inventoryTexture, ItemPackage itemPackage)
        {
            InventoryTexture = inventoryTexture;
            Quickbar.Items.Items[0] = new Item(itemPackage.ItemTypes[0], 1, true);
            Quickbar.Items.Items[1] = new Item(itemPackage.ItemTypes[2], 255, false); 
        }


        public void Update(Control control)
        {

            Quickbar.Update(control);

            

        }

        public void Draw(SpriteBatch spriteBatch, Control control, SpriteFont font, ItemPackage itemPackage)
        {

            Quickbar.Draw(spriteBatch, InventoryTexture, itemPackage, font);

            // Dragging Item
            if(Quickbar.Dragging > -1)
            {
                spriteBatch.Draw(itemPackage.ItemTexture, new Rectangle(control.currentMouse.X-30,control.currentMouse.Y-30,30,30), Quickbar.Items.Items[Quickbar.Dragging].Type.Location, Color.Orange);
            }

        }

        /// <summary>
        /// The player is attempting to pickup an item, this function finds a place to put the item
        /// </summary>
        /// <param name="item"></param>
        public bool Pickup(Item item)
        {
            if (Quickbar.Items.AddItem(item)) { return true; }

            return false;
        }

    }
}
