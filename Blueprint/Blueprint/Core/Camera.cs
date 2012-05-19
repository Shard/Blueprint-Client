using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Blueprint
{
    class Camera
    {

        #region Variables

        /// <summary>
        /// The X offset of the camera relative to the upper left of the canvas
        /// </summary>
        public float X;

        /// <summary>
        /// The X offset of the camera relative to the upper left of the canvas
        /// </summary>
        public float Y;

        /// <summary>
        /// The current width of the viewport
        /// </summary>
        public int Width;

        /// <summary>
        /// The current height of the viewport
        /// </summary>
        public int Height;

        /// <summary>
        /// The maximun length of the deadzone in each direction
        /// </summary>
        private float MaxDeadzone = 6f;

        /// <summary>
        /// The current deadzone based on the x axis
        /// </summary>
        private float Deadzone;

        #endregion

        public Camera(GraphicsDevice graphics, Player player)
        {
            Width = graphics.Viewport.Width;
            Height = graphics.Viewport.Height;
            X = (player.Movement.Area.Center.X * -1) + graphics.Viewport.Width / 2;
            Y = (player.Movement.Area.Center.Y * -1) + graphics.Viewport.Height / 2;
        }

        /// <summary>
        /// Syncs the camera with the current player
        /// </summary>
        /// <param name="movement"></param>
        public void Update( Vector2 movement )
        {

            if (movement.X > 0 && Deadzone < MaxDeadzone)
            {
                Deadzone += movement.X;
                if (Deadzone > MaxDeadzone)
                    { movement.X = Deadzone - MaxDeadzone; Deadzone = MaxDeadzone; }
                else
                    movement.X = 0;
            }
            else if(movement.X < 0 && Deadzone > MaxDeadzone * -1)
            {
                Deadzone += movement.X;
                if (Deadzone < MaxDeadzone * -1)
                    { movement.X = Deadzone - MaxDeadzone * -1; Deadzone = MaxDeadzone * -1; }
                else
                    movement.X = 0;
            }

            X -= movement.X;
            Y -= movement.Y;
        }

        /// <summary>
        /// Returns a vector 2 based of the X,Y values of the camera
        /// </summary>
        /// <returns></returns>
        public Vector2 ToVector2()
        {
            return new Vector2(X, Y);
        }

        /// <summary>
        /// Takes a rectangle and applies the camera offset to it, realigning the rectangle to be offset the current viewport x,y
        /// </summary>
        /// <param name="area">The rectangle that will be updated</param>
        /// <returns></returns>
        public Rectangle FromRectangle(Rectangle area)
        {
            return new Rectangle(area.X + (int)X, area.Y + (int)Y, area.Width, area.Height);
        }

        

    }
}
