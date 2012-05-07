using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Blueprint
{
    class Bag
    {
        /// <summary>
        /// The actual items inside the bag
        /// </summary>
        public ItemCollection Items;

        /// <summary>
        /// How many slots the bag gas
        /// </summary>
        public int Size;

        /// <summary>
        /// Wether the bag is open or not
        /// </summary>
        public bool Open;

        /// <summary>
        /// X offset of the inventory
        /// </summary>
        public Int16 OffsetX;

        /// <summary>
        /// Y offset of the inventory
        /// </summary>
        public Int16 OffsetY;

        /// <summary>
        /// Which item is selected on the quickbar
        /// </summary>
        public int Selected;

        /// <summary>
        ///  What quickbar slot is currently being hovered
        /// </summary>
        public int MouseOver;

        /// <summary>
        /// What quickbar slot has been mouse
        /// </summary>
        public int MouseDown;

        /// <summary>
        /// The width of the inventory menu in terms of blocks
        /// </summary>
        public Byte Width;

        /// <summary>
        /// The margin of each icon
        /// </summary>
        public byte IconMargin;

        /// <summary>
        /// Width of each icon
        /// </summary>
        public Byte IconWidth;

        private int AreaWidth;
        private int AreaHeight;

        public Bag(int size)
        {
            Size = size;
            Items = new ItemCollection(size);
            Open = false;
            OffsetX = 0;
            OffsetY = 200;
            Width = 8;
            IconMargin = 3;
            IconWidth = 40;

            AreaWidth = Width * ( IconWidth + IconMargin ) - IconMargin;
            AreaHeight = (Size / Width) * (IconWidth + IconMargin) - IconMargin;

            Selected = -1;
            MouseDown = -1;
            MouseOver = -1;
        }

        public void Update(Camera camera, Control control, ref Item held_item)
        {

            // Toggle backpack
            if (control.previousKeyboard.IsKeyUp(Keys.B) && control.currentKeyboard.IsKeyDown(Keys.B))
            {
                if (Open) { Open = false; } else { Open = true; }
            }

            if (Open)
            {

                // Check if the mouse is within the bag area
                int offsetx = (camera.Width - (AreaWidth + OffsetX)) / 2;
                int offsety = camera.Height - (AreaHeight + OffsetY);
                Rectangle bounds = new Rectangle(offsetx - 10, offsety - 10, AreaWidth + 20, AreaHeight + 20);
                Rectangle innerBounds = new Rectangle(offsetx, offsety, AreaWidth, AreaHeight);

                if (bounds.Intersects(new Rectangle(control.currentMouse.X, control.currentMouse.Y, 1, 1)))
                {

                    // Mouse is inside the inventory area
                    int col_remainder;
                    int row_remainder;
                    int current_col = Math.DivRem( control.currentMouse.X - offsetx, IconWidth + IconMargin, out col_remainder);
                    int current_row = Math.DivRem(control.currentMouse.Y - offsety, IconWidth + IconMargin, out row_remainder);

                    // Check if mouse landed within the icon area
                    if (col_remainder <= IconWidth && row_remainder <= IconWidth && col_remainder >= 0 && row_remainder >= 0)
                    {

                        int position = current_col + current_row * Width;
                        if (innerBounds.Intersects(new Rectangle(control.currentMouse.X, control.currentMouse.Y, 1, 1)))
                            { MouseOver = (int)MathHelper.Clamp(position, 0, Items.Size); } else { MouseOver = -1; }

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
                    // While hovering
                    if (MouseDown > -1)
                    {
                        if (MouseDown != MouseOver && Items.Items[MouseDown] != null) // If the player is now dragging the item
                        {
                            held_item = Items.Items[MouseDown];
                            Items.Items[MouseDown] = null;
                        }
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
                int offsetx = (spriteBatch.GraphicsDevice.Viewport.Width - (AreaWidth + OffsetX)) / 2;
                int offsety = spriteBatch.GraphicsDevice.Viewport.Height - (AreaHeight + OffsetY);

                foreach (Item item in Items.Items)
                {

                    Rectangle rect = new Rectangle(offsetx + (col_upto * (IconWidth + IconMargin)), offsety + (row_upto * (IconWidth + IconMargin)), IconWidth, IconWidth);

                    spriteBatch.Draw(texture, rect, new Rectangle(0, 0, 64, 64), Color.White);
                    if (item != null)
                    {
                        spriteBatch.Draw(package.ItemTexture, rect, item.Type.Location, Color.White);
                    }
                    col_upto++;
                    if (col_upto >= Width)
                    {
                        row_upto++;
                        col_upto = 0;
                    }
                }
                
                // Mouse over inventory slot
                if (MouseOver > -1)
                {
                    int col;
                    int row = Math.DivRem(MouseOver, Width, out col);
                    Rectangle rect = new Rectangle(offsetx + (col * (IconWidth + IconMargin)), offsety + (row * (IconWidth + IconMargin)), IconWidth, IconWidth);
                    spriteBatch.Draw(texture, rect, new Rectangle(128, 0, 64, 64), Color.White);
                }

            }

        }


    }
}
