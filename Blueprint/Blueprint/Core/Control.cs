using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Blueprint
{
    class Control
    {

        public KeyboardState currentKeyboard;
        public KeyboardState previousKeyboard;
        public MouseState previousMouse;
        public MouseState currentMouse;

        // Tells which block the cursor is at
        public int AtBlockX; 
        public int AtBlockY;
        public Vector2 AtBlock { get { return new Vector2(AtBlockX, AtBlockY); } }
        public Rectangle MousePos { get { return new Rectangle(currentMouse.X, currentMouse.Y, 1, 1); } }

        public bool IsLocked
        {
            get { return Typing; }
        }
        public bool Typing; // If true, the player is typing and actions such as movement should be locked
        public bool MouseUi; // If true, the mouse is affecting ui and thus should no propogate to the game canvas
        public enum CursorStates
        {
            Default,
            Interact,
            Mine,
            Chop,
            Hammer
        };
        public CursorStates State;

        public Control()
        {

            Typing = false;
            MouseUi = false;
        }

        public void Update(KeyboardState keyboard, MouseState mouse, Camera camera )
        {

            previousKeyboard = currentKeyboard;
            currentKeyboard = keyboard;
            previousMouse = currentMouse;
            currentMouse = mouse;

            AtBlockX = (currentMouse.X - (int)camera.X) / 24;
            AtBlockY = (currentMouse.Y - (int)camera.Y) / 24;

        }

        public void Draw(SpriteBatch spriteBatch)
        {

        }

        public void ChangeCursor(CursorStates state)
        {
            State = state;
        }

        public bool Pressed(Keys key)
        {
            if (currentKeyboard.IsKeyDown(key) && previousKeyboard.IsKeyUp(key)) { return true; } else { return false; }
        }

        public bool Click(bool left_button = true)
        {
            if (left_button)
                { if (currentMouse.LeftButton == ButtonState.Pressed && previousMouse.LeftButton == ButtonState.Released) { return true; } else { return false; } }
            else
                { if (currentMouse.RightButton == ButtonState.Pressed && previousMouse.RightButton == ButtonState.Released) { return true; } else { return false; } }
        }

    }
}
