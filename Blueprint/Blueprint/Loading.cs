using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Blueprint
{
    class Loading
    {

        public bool IsLoading;


        public Loading()
        {
            IsLoading = false;
        }

        public void Update(GameTime gameTime)
        {

            if (gameTime.TotalGameTime.Seconds > 3)
            {
                IsLoading = false;
            }
        }

        public void Draw(SpriteBatch spriteBatch, GraphicsDevice GraphicsDevice, SpriteFont font)
        {
            GraphicsDevice.Clear(Color.White);
            spriteBatch.Begin();


            spriteBatch.DrawString(font, "Loading Map Data", new Vector2(GraphicsDevice.Viewport.Width / 2, 0), Color.Black, 0f, new Vector2(10,0), 1, SpriteEffects.None, 0);

            spriteBatch.End();

        }

    }
}
