﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Blueprint
{
    class Quickbar
    {

        public ItemCollection Items; // Collection of all the items the quickbar contains
        public int Size; // Size of the quickbar

        // Positioning and layout
        public string Anchor; // top,right,bottom,left
        public int Offset; // How many action bars offset from the side
        public float Scale; // How large the quickbar should be
        public string Align; // If vertical: top,center,bottom. If horizontal: left,center,right.

        // States
        public int Selected; // Which item is selected on the quickbar
        public int MouseOver; // What quickbar slot is currently being hovered
        public int MouseDown; // What quickbar slot has been mouse
        public bool Locked; // If locked, dragging must be done with shift

        // Using Items
        public ItemUse UsingItem;
        public bool IsUsingItem;

        public Quickbar( int size  )
        {

            Size = size;
            Items = new ItemCollection(size);
            Locked = false;
            UsingItem = new ItemUse();

            // Default Positioning
            Anchor = "left";
            Offset = 0;
            Scale = 1f;
            Align = "center";

            // Default States
            Selected = 0;
            MouseOver = -1;
            MouseDown = -1;
        }

        public void Update(Control control, ref Item held_item)
        {


            // Scroll
            if (control.currentMouse.ScrollWheelValue > control.previousMouse.ScrollWheelValue)
            { Selected++; if (Selected > Size - 1) { Selected = 0; } }
            if (control.currentMouse.ScrollWheelValue < control.previousMouse.ScrollWheelValue)
            { Selected--; if (Selected < 0) { Selected = Size - 1; } }


            // Move Hover
            if (control.currentMouse.X >= 20 && control.currentMouse.X <= 60)
            {
                int currentY = control.currentMouse.Y - 20;
                MouseOver = currentY / 45;
                // add more checks here
            }
            else { MouseOver = -1; }

            // Mousedown on button
            if ( MouseOver > -1 && control.currentMouse.LeftButton == ButtonState.Pressed && control.previousMouse.LeftButton == ButtonState.Released)
            {
                MouseDown = MouseOver;
            }

            // While hovering
            if (MouseDown > -1)
            {
                if (MouseDown != MouseOver && Items.Items[MouseDown] != null) // If the player is now dragging the item
                {
                    held_item = Items.Items[MouseDown];
                    Items.Items[MouseDown] = null;
                }
            }

            // Mouseup
            if (MouseDown > -1 && control.currentMouse.LeftButton == ButtonState.Released && control.previousMouse.LeftButton == ButtonState.Pressed)
            {
                if (held_item != null && MouseOver > -1) // Drop on quickbar, moving its position
                {
                    Item swapItem = Items.Items[MouseOver];
                    Items.Items[MouseOver] = held_item;
                    held_item = swapItem;
                }
                else if (held_item != null)
                {
                    Selected = MouseOver;
                }
                MouseDown = -1;
            }

            // Using Items

            // Place blocks
            if (Selected > -1 && control.currentMouse.LeftButton == ButtonState.Pressed && control.MouseUi == false && Items.Items[Selected] != null)
            {
                UsingItem.Update(Items.Items[Selected].Type.Use);
            }
            else
            {
                UsingItem.Clear();
            }

        }

        public void useItem( int item )
        {
            if (Items.Items[item] == null) { return; }
            Items.Items[item].Stack--;
            if(Items.Items[item].Stack <= 0){
                Items.Items[item] = null;
            }
        }

        public void Draw(SpriteBatch spriteBatch, Texture2D inventoryTexture, ItemPackage itemPackage, SpriteFont font)
        {
            // Draw Quickbar
            for (int i = 0; i < Size; i++)
            {
                spriteBatch.Draw(inventoryTexture, new Rectangle(20, 20 + (45 * i), 40, 40), new Rectangle(0, 0, 40, 40), Color.White);

                // Draw item
                if (Items.Items[i] != null)
                {
                    spriteBatch.Draw(itemPackage.ItemTexture, new Rectangle(20, 20 + (45 * i), 40, 40), Items.Items[i].Type.Location, Color.White);
                }

                if (Items.Items[i] != null && Items.Items[i].Type.Stacksize > 0)
                {
                    spriteBatch.DrawString(font, Items.Items[i].Stack.ToString(), new Vector2(27, 40 + (45 * i)), Color.White, 0, Vector2.Zero, 0.7f, SpriteEffects.None, 0);
                }
            }

            // States
            spriteBatch.Draw(inventoryTexture, new Rectangle(20, 20 + (45 * Selected), 40, 40), new Rectangle(80, 0, 40, 40), Color.White);

            if (MouseOver > -1)
            {
                spriteBatch.Draw(inventoryTexture, new Rectangle(20, 20 + (45 * MouseOver), 40, 40), new Rectangle(40, 0, 40, 40), Color.White);
            }

        }


    }
}
