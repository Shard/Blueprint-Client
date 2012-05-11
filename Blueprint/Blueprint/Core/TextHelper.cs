using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Blueprint
{
    static class TextHelper
    {

        public static void DrawString(SpriteBatch spriteBatch, SpriteFont font, string text, Vector2 position, Color color, float scale = 1f, bool shadow = true, string align = "left")
        {

            // Alignment
            if (align == "right")
                { position.X -= font.MeasureString(text).X * scale; }

            if (shadow)
            {
                spriteBatch.DrawString(font, text, new Vector2(position.X + (scale * 1), position.Y + (scale * 1)), new Color(20, 20, 20, color.A), 0f, Vector2.Zero, scale, SpriteEffects.None, 0);
            }
            spriteBatch.DrawString(font, text, position, color, 0f, Vector2.Zero, scale, SpriteEffects.None, 0);

        }

    }
}
