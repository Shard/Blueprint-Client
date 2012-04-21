using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Blueprint
{
    class Bag
    {

        public ItemCollection Items;
        public int Size;

        // States
        public bool Open;
        public Int16 OffsetX;
        public Int16 OffsetY;

        public int Selected; // Which item is selected on the quickbar
        public int MouseOver; // What quickbar slot is currently being hovered
        public int MouseDown; // What quickbar slot has been mouse
        public bool Locked; // If locked, dragging must be done with shift

        /// <summary>
        /// The width of the inventory menu in terms of blocks
        /// </summary>
        public Byte Width;

        public Bag(int size)
        {
            Size = size;
            Items = new ItemCollection(size);
            Open = false;
            OffsetX = 100;
            OffsetY = 100;
            Width = 8;

            Selected = -1;
            MouseDown = -1;
            MouseOver = -1;
        }

        public void Update(Control control, ref Item held_item)
        {

            if (control.previousKeyboard.IsKeyUp(Keys.B) && control.currentKeyboard.IsKeyDown(Keys.B))
            {
                if (Open) { Open = false; } else { Open = true; }
            }

            if (Open)
            {

                // Mouse is inside the bag area
                Rectangle bounds = new Rectangle(OffsetX,OffsetY,Width * 50, (Size / 8) * 50);
                if (bounds.Intersects(new Rectangle(control.currentMouse.X, control.currentMouse.Y, 1, 1)))
                {
                    // Mouse is inside the inventory area
                    int current_col = (control.currentMouse.X - OffsetX) / 50;
                    int current_row = (control.currentMouse.Y - OffsetY) / 50;

                    int position = current_col + current_row * 8;
                    MouseOver = (int)MathHelper.Clamp(position, 0, Items.Size);

                    // While hovering
                    if (MouseDown > -1)
                    {
                        if (MouseDown != MouseOver && Items.Items[MouseDown] != null) // If the player is now dragging the item
                        {
                            held_item = Items.Items[MouseDown];
                            Items.Items[MouseDown] = null;
                        }
                    }

                    // Mousedown on button
                    if (MouseOver > -1 && control.currentMouse.LeftButton == ButtonState.Pressed && control.previousMouse.LeftButton == ButtonState.Released)
                    {
                        MouseDown = MouseOver;
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
                }
                else
                {
                    MouseOver = -1;
                }

                
            }

        }

        public void Draw(SpriteBatch spriteBatch, Texture2D texture, ItemPackage package, SpriteFont font)
        {

            if (Open)
            {

                byte row_upto = 0;
                byte col_upto = 0;

                foreach (Item item in Items.Items)
                {
                    spriteBatch.Draw(texture, new Rectangle(OffsetX + col_upto * 50, OffsetY + row_upto * 50, 40, 40), new Rectangle(0, 0, 40, 40), Color.White);
                    if (item != null)
                    {
                        spriteBatch.Draw(package.ItemTexture, new Rectangle(OffsetX + col_upto * 50, OffsetY + row_upto * 50, 40, 40), item.Type.Location, Color.White);
                    }
                    col_upto++;
                    if (col_upto >= Width)
                    {
                        row_upto++;
                        col_upto = 0;
                    }
                }
                
                // Hover
                if (MouseOver > -1)
                {
                    int col;
                    int row = Math.DivRem(MouseOver, 8, out col);

                    spriteBatch.Draw(texture, new Rectangle(OffsetX + (col * 50), OffsetY + (row * 50), 40, 40), new Rectangle(40, 0, 40, 40), Color.White);
                }

            }

        }


    }
}
