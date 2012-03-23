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
        public Item[] Quickbar; // Items on the players quickbar
        public Item[] Bank; // Items in the players bank
        public Keys[] Hotkeys;

        // States
        public int QuickbarSelected; // Which item is selected on the quickbar
        public int QuickbarMouseOver; // What quickbar slot is currently being hovered
        public int QuickbarMouseDown; // What quickbar slot has been mouse
        public int QuickbarDragging; // Which Quickbar slot is being dragged

        // Money
        public int Money;
        public int BankedMoney;

        public Inventory()
        {

            Bag = new Item[40];
            Quickbar = new Item[10];
            Bank = new Item[80];
            Money = 99999;
            BankedMoney = 12345;
            Hotkeys = ApplyHotkeys();
            QuickbarSelected = 0;
        }

        public Inventory(int bagSize, int quickbarSize, int bankSize, int money, int bankedMoney)
        {

            Bag = new Item[bagSize];
            Quickbar = new Item[quickbarSize];
            Bank = new Item[bankSize];
            Money = money;
            BankedMoney = bankedMoney;
            Hotkeys = ApplyHotkeys();
            QuickbarSelected = 0;
            QuickbarMouseOver = -1;
        }

        public Keys[] ApplyHotkeys()
        {
            Keys[] keys = new Keys[Quickbar.Length];
            for (int i = 0; i < Quickbar.Length; i++)
            {
                keys[i] = Keys.D1;
            }
            return keys;
        }

        public void Initialize(Texture2D inventoryTexture, ItemCollection itemCollection)
        {
            InventoryTexture = inventoryTexture;
            Quickbar[0] = new Item(itemCollection.ItemTypes[0], 1, true);
            Quickbar[1] = new Item(itemCollection.ItemTypes[1], 255, false); 
            QuickbarDragging = -1;
            QuickbarMouseDown = -1;
            QuickbarMouseOver = -1;
        }


        public void Update(KeyboardState currentKeyboardState, KeyboardState previousKeyboardState, MouseState currentMouseState, MouseState previousMouseState)
        {

            if (currentMouseState.ScrollWheelValue > previousMouseState.ScrollWheelValue)
            {
                QuickbarSelected++;
                if (QuickbarSelected > Quickbar.Length - 1) { QuickbarSelected = 0; }
            }
            if (currentMouseState.ScrollWheelValue < previousMouseState.ScrollWheelValue)
            {
                QuickbarSelected--;
                if (QuickbarSelected < 0) { QuickbarSelected = Quickbar.Length - 1; }
            }
            


            if (currentMouseState.X >= 20 && currentMouseState.X <= 60)
            {
                int currentY = currentMouseState.Y - 20;
                QuickbarMouseOver = currentY / 45;
                // add more checks here
            }
            else
            {
                QuickbarMouseOver = -1;
            }

            // Mousedown on button
            if( QuickbarMouseOver > -1 && currentMouseState.LeftButton == ButtonState.Pressed && previousMouseState.LeftButton == ButtonState.Released)
            {
                QuickbarMouseDown = QuickbarMouseOver;
            }

            // While hovering
            if(QuickbarMouseDown > -1){
                if(QuickbarMouseDown != QuickbarMouseOver && Quickbar[QuickbarMouseDown] != null)
                {
                    QuickbarDragging = QuickbarMouseDown;
                }
            }

            // Mouseup
            if( QuickbarMouseDown > -1 && currentMouseState.LeftButton == ButtonState.Released && previousMouseState.LeftButton == ButtonState.Pressed )
            {
                if(QuickbarDragging > -1 && QuickbarMouseOver > -1) // Drop on quickbar, moving its position
                {

                    Item tmpItem = Quickbar[QuickbarMouseOver];

                    Quickbar[QuickbarMouseOver] = Quickbar[QuickbarDragging];

                    if (tmpItem != null) // Commit swap
                    {
                        Quickbar[QuickbarDragging] = tmpItem;
                    }

                    Quickbar[QuickbarDragging] = null;
                    QuickbarDragging = -1;
                    
                } else if(QuickbarDragging == -1) {
                    QuickbarSelected = QuickbarMouseOver;
                }

                QuickbarMouseDown = -1;
            }

        }

        public void Draw(SpriteBatch spriteBatch, MouseState currentMouseState, SpriteFont font, ItemCollection itemCollection)
        {

            // Draw Quickbar
            for (int i = 0; i < Quickbar.Length; i++)
            {
                //if (Quickbar[i] == null) { continue; }
                spriteBatch.Draw(InventoryTexture, new Rectangle(20, 20 + (45 * i), 40, 40), new Rectangle(0, 0, 40, 40), Color.White);

                // Draw item
                if(Quickbar[i] != null)
                {
                    spriteBatch.Draw(itemCollection.ItemTexture, new Rectangle(20, 20 + (45 * i), 40, 40), Quickbar[i].Type.Location, Color.White);
                }

                if (Hotkeys[i].ToString() != "")
                {
                    spriteBatch.DrawString(font, Hotkeys[i].ToString(), new Vector2(27, 23 + (45 * i)), Color.White, 0, Vector2.Zero, 0.5f, SpriteEffects.None, 0);
                }
                if (Quickbar[i] != null && Quickbar[i].Type.Stacksize > 0)
                {
                    spriteBatch.DrawString(font, Quickbar[i].Stack.ToString(), new Vector2(27, 40 + (45 * i)), Color.White, 0, Vector2.Zero, 0.7f, SpriteEffects.None, 0);
                }
            }

            

            // States
            spriteBatch.Draw(InventoryTexture, new Rectangle(20,20 + (45 * QuickbarSelected), 40,40),new Rectangle(80,0,40,40), Color.White);
            
            if(QuickbarMouseOver > -1){
                spriteBatch.Draw(InventoryTexture, new Rectangle(20, 20 + (45 * QuickbarMouseOver),40,40), new Rectangle(40,0,40,40), Color.White);
            }

            // Dragging Item
            if(QuickbarDragging > -1)
            {
                spriteBatch.Draw(itemCollection.ItemTexture, new Rectangle(currentMouseState.X-30,currentMouseState.Y-30,30,30), Quickbar[QuickbarDragging].Type.Location, Color.Orange);
            }

        }

    }
}
