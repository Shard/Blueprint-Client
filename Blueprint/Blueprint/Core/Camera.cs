using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Blueprint
{
    class Camera
    {

        public float X;
        public float Y;

        public Camera(GraphicsDevice graphics)
        {
            X = graphics.Viewport.TitleSafeArea.X + graphics.Viewport.TitleSafeArea.Width / 2 - 100;
            Y = graphics.Viewport.TitleSafeArea.Y + graphics.Viewport.TitleSafeArea.Height / 2 + 100;
        }

        public void Update( Vector2 movement )
        {
            X -= movement.X;
            Y -= movement.Y;
        }

        public Vector2 ToVector2()
        {
            return new Vector2(X, Y);
        }

        public Rectangle FromRectangle(Rectangle area)
        {
            return new Rectangle(area.X + (int)X, area.Y + (int)Y, area.Width, area.Height);
        }

    }
}
