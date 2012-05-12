using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Blueprint
{
    class Quickbar
    {

        public ItemCollection Items; // Collection of all the items the quickbar contains
        public int Size; // Size of the quickbar


        #region Positioning and layout

        /// <summary>
        /// left, center, right
        /// </summary>
        public string Horizontal;

        /// <summary>
        /// top,center,bottom.
        /// </summary>
        public string Vertical;

        /// <summary>
        /// horizontal or vertical
        /// </summary>
        public string Layout;

        /// <summary>
        /// Offset for the quiclkbar from its Anchor
        /// </summary>
        public Vector2 Offset;

        /// <summary>
        /// Margin of each button, will always expand away from alignment anchor
        /// </summary>
        public byte Margin;

        /// <summary>
        /// Scale in size, default is 1f
        /// </summary>
        public float Scale;

        /// <summary>
        /// The area that the quickbar occupies
        /// </summary>
        public Rectangle Area;

        private int IncrementX { get { if (Layout == "horizontal") { return 64 + Margin; } else { return 0; } } }
        private int IncrementY { get { if (Layout == "vertical") { return 64 + Margin; } else { return 0; } } }

        #endregion

        // States
        public int Selected; // Which item is selected on the quickbar
        public int MouseOver; // What quickbar slot is currently being hovered
        public int MouseDown; // What quickbar slot has been mouse
        public bool Locked; // If locked, dragging must be done with shift

        // Using Items
        public ItemUse UsingItem;
        public bool IsUsingItem;
        public Item CurrentItem
        {
            get { return Items.Items[Selected]; }
        }

        public Quickbar( int size  )
        {

            Size = size;
            Items = new ItemCollection(size);
            Locked = false;
            UsingItem = new ItemUse();

            // Default Positioning
            Vertical = "bottom";
            Horizontal = "center";
            Layout = "horizontal";
            Offset = new Vector2(0, 10);
            Scale = 1f;
            Margin = 2;

            // Default States
            Selected = 0;
            MouseOver = -1;
            MouseDown = -1;
        }

        public void Update(Camera camera, Control control, ref Item held_item)
        {

            #region Scrolling and Hotkeys

            if (control.currentMouse.ScrollWheelValue > control.previousMouse.ScrollWheelValue)
            { Selected++; if (Selected > Size - 1) { Selected = Size - 1; } }
            if (control.currentMouse.ScrollWheelValue < control.previousMouse.ScrollWheelValue)
            { Selected--; if (Selected < 0) { Selected = 0; } }

            if (!control.Typing){
                if (control.currentKeyboard.IsKeyDown(Keys.D1) && control.previousKeyboard.IsKeyUp(Keys.D1) && Size >= 1){ Selected = 0; }
                if (control.currentKeyboard.IsKeyDown(Keys.D2) && control.previousKeyboard.IsKeyUp(Keys.D2) && Size >= 2) { Selected = 1; }
                if (control.currentKeyboard.IsKeyDown(Keys.D3) && control.previousKeyboard.IsKeyUp(Keys.D3) && Size >= 3) { Selected = 2; }
                if (control.currentKeyboard.IsKeyDown(Keys.D4) && control.previousKeyboard.IsKeyUp(Keys.D4) && Size >= 4) { Selected = 3; }
                if (control.currentKeyboard.IsKeyDown(Keys.D5) && control.previousKeyboard.IsKeyUp(Keys.D5) && Size >= 5) { Selected = 4; }
                if (control.currentKeyboard.IsKeyDown(Keys.D6) && control.previousKeyboard.IsKeyUp(Keys.D6) && Size >= 6) { Selected = 5; }
                if (control.currentKeyboard.IsKeyDown(Keys.D7) && control.previousKeyboard.IsKeyUp(Keys.D7) && Size >= 7) { Selected = 6; }
                if (control.currentKeyboard.IsKeyDown(Keys.D8) && control.previousKeyboard.IsKeyUp(Keys.D8) && Size >= 8) { Selected = 7; }
                if (control.currentKeyboard.IsKeyDown(Keys.D9) && control.previousKeyboard.IsKeyUp(Keys.D9) && Size >= 9) { Selected = 8; }
                if (control.currentKeyboard.IsKeyDown(Keys.D0) && control.previousKeyboard.IsKeyUp(Keys.D0) && Size >= 10) { Selected = 9; }
            }

            #endregion

            #region Positioning

            if (Layout == "horizontal")
            {
                Area.Height = 64;
                Area.Width = Size * (64 + Margin);
            }
            else
            {
                Area.Width = 64;
                Area.Height = Size * (64 + Margin);
            }

            if (Horizontal == "left")
                { Area.X = (int)Offset.X; }
            else if (Horizontal == "right")
                { Area.X = (int)(camera.Width - (Offset.X + Area.Width)); }
            else
                { Area.X = (int)(camera.Width - (Offset.X + Area.Width)) / 2; }

            if (Vertical == "top")
                { Area.Y = (int)Offset.Y; }
            else if (Vertical == "bottom")
                { Area.Y = (int)(camera.Height - (Offset.Y + Area.Height)); }
            else
                { Area.Y = (int)(camera.Height - (Offset.Y + Area.Height)) / 2; }

            #endregion

            #region Selection and item management

            bool locked_use = false;

            // Calculate if the mouse is hovering over any buttons or not
            if (Area.Intersects(control.MousePos))
            {
                locked_use = true;
                int innerpos = 0;
                if (Layout == "horizontal")
                {
                    int mousepos = Math.DivRem(control.currentMouse.X - Area.X, 64 + Margin, out innerpos);
                    if (innerpos <= 64) { MouseOver = mousepos; } else { MouseOver = -1; }
                }
                else
                {
                    int mousepos = Math.DivRem(control.currentMouse.Y - Area.Y, 64 + Margin, out innerpos);
                    if (innerpos <= 64) { MouseOver = mousepos; } else { MouseOver = -1; }
                }
            } else { MouseOver = -1; }

            // Check if the user just clicked their mouse, if so mark the button as being currently mouse downed on
            if ( MouseOver > -1 && control.currentMouse.LeftButton == ButtonState.Pressed && control.previousMouse.LeftButton == ButtonState.Released)
                { MouseDown = MouseOver; }

            // If the mouse is down on a button and it leaves the button, start dragging the item
            if (MouseDown > -1)
            {
                if (MouseDown != MouseOver && Items.Items[MouseDown] != null)
                {
                    held_item = Items.Items[MouseDown];
                    Items.Items[MouseDown] = null;
                }
            }

            // If the mouse was down and the user has now released the left mouse button
            if (MouseDown > -1 && control.currentMouse.LeftButton == ButtonState.Released && control.previousMouse.LeftButton == ButtonState.Pressed)
            {
                if (held_item != null && MouseOver > -1) // Holding and item and hovering over a button
                {
                    Item swapItem = Items.Items[MouseOver];
                    Items.Items[MouseOver] = held_item;
                    held_item = swapItem;
                }
                else if (held_item != null) // Holding an item and not overing over any other button
                {
                    Selected = MouseOver;
                }
                else if (MouseDown == MouseOver)
                {
                    Selected = MouseOver;
                }
                MouseDown = -1;
            }

            #endregion

            // Using Items
            if (!locked_use && Selected > -1 && control.currentMouse.LeftButton == ButtonState.Pressed && control.MouseUi == false && Items.Items[Selected] != null)
                { UsingItem.Update(Items.Items[Selected].Type.Use); }
            else if(control.currentMouse.LeftButton == ButtonState.Released && control.previousMouse.LeftButton == ButtonState.Pressed){
                // Wait up, some weapons want to finish
            }
            else { UsingItem.Clear(); }

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

                Rectangle current_rect = new Rectangle((int)Area.X + (IncrementX * i), (int)Area.Y + (IncrementY * i), 64, 64);

                spriteBatch.Draw(inventoryTexture, current_rect, new Rectangle(0, 0, 64, 64), Color.White);

                // Draw item
                if (Items.Items[i] != null)
                {
                    Rectangle item_rec = new Rectangle(Items.Items[i].Type.Location.X + 3, Items.Items[i].Type.Location.Y + 3, Items.Items[i].Type.Location.Width - 6, Items.Items[i].Type.Location.Height - 6);
                    spriteBatch.Draw(itemPackage.ItemTexture, current_rect, item_rec, Color.White);
                }
                
                if (i == MouseOver || i == Selected)
                {
                    spriteBatch.Draw(inventoryTexture, current_rect, new Rectangle(128, 0, 64, 64), Color.White);
                }
                else
                {
                    spriteBatch.Draw(inventoryTexture, current_rect, new Rectangle(64, 0, 64, 64), Color.White);
                }

                // Draw keybindings
                string keybind_string;
                if (i == 9) { keybind_string = "0"; } else { keybind_string = (i + 1).ToString(); }
                TextHelper.DrawString(spriteBatch, font, keybind_string, new Vector2(current_rect.X + 50, current_rect.Y + 5), Color.White, 0.7f);

                if (Items.Items[i] != null && Items.Items[i].Type.Stacksize > 0)
                {
                    TextHelper.DrawString(spriteBatch, font, Items.Items[i].Stack.ToString(), new Vector2(current_rect.X + 56, current_rect.Y + 48), Color.White, 0.6f, align: "right");
                }
            }

            // States
            //spriteBatch.Draw(inventoryTexture, new Rectangle(20, 20 + (45 * Selected), 40, 40), new Rectangle(80, 0, 40, 40), Color.White);

            if (MouseOver > -1)
            {
               // spriteBatch.Draw(inventoryTexture, new Rectangle(20, 20 + (45 * MouseOver), 40, 40), new Rectangle(40, 0, 40, 40), Color.White);
            }

        }


    }
}
