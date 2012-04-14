using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Blueprint.Ui
{

    class FloatingTextCollection
    {

        public List<FloatingText> Texts;

        public FloatingTextCollection()
        {
            Texts = new List<FloatingText>();
        }

        public void Update()
        {
            for (int i = 0; i < Texts.Count; i++)
            {
                Texts[i].FramesLeft -= 1;
                Texts[i].Color = new Color((byte)(255 * Texts[i].FramesLeft / 100), (byte)0, (byte)0, (byte)Texts[i].FramesLeft);
                Texts[i].Location.Y -= 1;

                if (Texts[i].FramesLeft <= 0)
                {
                    Texts.Remove(Texts[i]);
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch, Camera camera, SpriteFont font)
        {

            foreach (FloatingText text in Texts)
            {
                spriteBatch.DrawString(font, text.Message, text.Location + camera.ToVector2(), text.Color);
            }

        }

        public void Add(string message, Vector2 location)
        {
            Texts.Add(new FloatingText(message,location));
        }

    }

    class FloatingText
    {

        public string Message;
        public Vector2 Location;
        public Single FramesLeft;
        public Color Color;

        public FloatingText(string message, Vector2 location)
        {
            Message = message;
            Location = location;
            FramesLeft = 100;
            Color = Color.DarkOrange;
        }

    }
}
