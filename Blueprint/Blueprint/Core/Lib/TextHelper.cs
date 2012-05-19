using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Blueprint
{
    static class TextHelper
    {


        public static string[] SplitWrap(string s, int maxwidth, SpriteFont font, float scale = 1f)
        {

            Queue<string> word_stack = new Queue<string>(s.Split(' '));
            List<string> lines = new List<string>();
            string current_line = "";

            while (word_stack.Count > 0)
            {
                string word = word_stack.Dequeue();
                if (font.MeasureString(current_line + word).X * scale >= maxwidth)
                {
                    lines.Add(current_line.Trim());
                    current_line = word;
                }
                else
                {
                    current_line += " " + word;
                }
            }
            if (current_line.Length > 0)
                lines.Add(current_line.Trim());


            return lines.ToArray();
        }

        public static void DrawString(
            SpriteBatch spriteBatch,
            SpriteFont font,
            string text,
            Vector2 position,
            Color color,
            float scale = 1f,
            bool shadow = true,
            string align = "left",
            string valign = "top",
            Camera camera = null,
            int width_wrap = 0
        ){

            if (width_wrap > 0)
            {
                string[] lines = SplitWrap(text, width_wrap, font, scale);
                Vector2 measure = font.MeasureString("s");
                int y_offset = 0;
                foreach (string line in lines)
                {
                    DrawString(spriteBatch, font, line, new Vector2(position.X, position.Y + y_offset), color, scale, shadow, align, camera:camera);
                    y_offset += (int)measure.Y -4;
                }
                return;
            }

            // Camera offset
            if (camera != null)
                position -= camera.ToVector2();

            // Alignment
            if (align == "right")
                position.X -= font.MeasureString(text).X * scale;
            else if (align == "center")
                position.X -= (font.MeasureString(text).X * scale) * 0.5f;

            if (valign == "center")
                position.Y -= (font.MeasureString(text).Y * scale) * 0.5f;
            else if (valign == "bottom")
                position.Y -= font.MeasureString(text).Y * scale;

            if (shadow)
            {
                spriteBatch.DrawString(font, text, new Vector2(position.X + (scale * 1), position.Y + (scale * 1)), new Color(20, 20, 20, color.A), 0f, Vector2.Zero, scale, SpriteEffects.None, 0);
            }
            spriteBatch.DrawString(font, text, position, color, 0f, Vector2.Zero, scale, SpriteEffects.None, 0);

        }

    }
}
