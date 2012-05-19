using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Blueprint
{
    class Control
    {

        #region Raw States

        /// <summary>
        /// The current keyboard state
        /// </summary>
        public KeyboardState currentKeyboard;

        /// <summary>
        /// The previous keyboard state
        /// </summary>
        public KeyboardState previousKeyboard;

        /// <summary>
        /// The previous mouse state
        /// </summary>
        public MouseState previousMouse;

        /// <summary>
        /// The current mouse state
        /// </summary>
        public MouseState currentMouse;

        #endregion

        #region Mouse Helpers

        /// <summary>
        /// The x location of the tile the mouse is currently at
        /// </summary>
        public int AtTileX; 

        /// <summary>
        /// The y location of the tile the mouse is current at
        /// </summary>
        public int AtTileY;

        /// <summary>
        /// A Point containing the current tile location the mouse is at
        /// </summary>
        public Point AtTile { get { return new Point(AtTileX, AtTileY); } }

        /// <summary>
        /// A rectangle at the current location of the mouse, used for rectangle intersection
        /// </summary>
        public Rectangle MousePos { get { return new Rectangle(currentMouse.X, currentMouse.Y, 1, 1); } }

        #endregion

        #region Ui States

        /// <summary>
        /// If true, lock the keyboard controls for the game as they are being used by gui
        /// </summary>
        public bool IsLocked
        {
            get { return Typing; }
        }

        /// <summary>
        /// If true, the player is typing and actions such as movement should be locked
        /// </summary>
        public bool Typing;

        /// <summary>
        ///  If true, the mouse is affecting ui and thus should no propogate to the game canvas
        /// </summary>
        public bool MouseLock
        {
            set
            {
                if (value == true) { MouseLockCounter = 3; }
                _MouseLock = value;
            }
            get { return _MouseLock; }
        }

        private bool _MouseLock;

        private byte MouseLockCounter;

        #endregion

        #region Cursor

        /// <summary>
        /// The different states the control object can be in, relating the mouse cursor
        /// </summary>
        public enum CursorStates
        {
            Default,
            Interact,
            Mine,
            Chop,
            Hammer,
            Attack,
            Custom
        };

        /// <summary>
        /// The current mouse cursor state
        /// </summary>
        public CursorStates State;

        /// <summary>
        /// Cursor texture
        /// </summary>
        private Texture2D Texture;

        /// <summary>
        /// The source rectangle of the current mouse sprite
        /// </summary>
        private Rectangle Sprite
        {
            get { switch (State) {
                case CursorStates.Mine:
                    return new Rectangle(32,0,32,32);
                case CursorStates.Attack:
                    return new Rectangle(64,0,32,32);
                case CursorStates.Custom:
                case CursorStates.Interact:
                    return new Rectangle(96, 0, 32, 32);
                default:
                    return new Rectangle(0, 0, 32, 32);
            } }
        }

        #endregion

        public void Initialize(Texture2D texture)
        {
            Texture = texture;
        }

        public void Update(KeyboardState keyboard, MouseState mouse, Camera camera )
        {

            State = CursorStates.Default;

            previousKeyboard = currentKeyboard;
            currentKeyboard = keyboard;
            previousMouse = currentMouse;
            currentMouse = mouse;

            AtTileX = (currentMouse.X - (int)camera.X) / 24;
            AtTileY = (currentMouse.Y - (int)camera.Y) / 24;

            if (MouseLock == true && MouseLockCounter > 0)
                MouseLockCounter--;
            else if (MouseLock == true)
                MouseLock = false;

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, new Rectangle(currentMouse.X, currentMouse.Y, 24, 24), Sprite, Color.White);
        }

        public void ChangeCursor(CursorStates state)
        {
            State = state;
        }

        /// <summary>
        /// Check if a key was pressed
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Pressed(Keys key)
        {
            if (currentKeyboard.IsKeyDown(key) && previousKeyboard.IsKeyUp(key)) { return true; } else { return false; }
        }

        /// <summary>
        /// Check if a mouse button was clicked, default is left button
        /// </summary>
        /// <param name="left_button"></param>
        /// <returns></returns>
        public bool Click(bool left_button = true)
        {
            if (left_button)
                { if (currentMouse.LeftButton == ButtonState.Pressed && previousMouse.LeftButton == ButtonState.Released) { return true; } else { return false; } }
            else
                { if (currentMouse.RightButton == ButtonState.Pressed && previousMouse.RightButton == ButtonState.Released) { return true; } else { return false; } }
        }

    }
}
