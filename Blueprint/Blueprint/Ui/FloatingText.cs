using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Blueprint.Ui
{

    class FloatingTextCollection
    {

        public List<FloatingText> Texts;

        /// <summary>
        /// How many frames the update should skip, used for debugging
        /// </summary>
        private byte FrameSkip;

        public FloatingTextCollection()
        {
            Texts = new List<FloatingText>();
        }

        public void Update()
        {

            if (FrameSkip > 0) { FrameSkip--; return; }

            for (int i = 0; i < Texts.Count; i++)
            {
                //FrameSkip = 30;
                FloatingText text = Texts[i];
                text.Counter++;

                // Scaling
                if(text.ScaleSwitch >= text.Counter)
                    text.FinalScale = text.Low + ((text.High - text.Low) * ((text.ScaleSwitch - (float)text.Counter) / text.ScaleSwitch));
                else if(text.ScaleSwitch <= text.Counter && text.ScaleEnd >= text.Counter)
                    text.FinalScale = text.Low + (text.Standard - text.Low) * (text.Counter - text.ScaleSwitch) / (text.ScaleEnd - text.ScaleSwitch);
                else
                    text.FinalScale = text.Standard;

                // Color / Fading
                if (text.StartFade <= text.Counter)
                {
                    float scale = 1f - ((float)text.Counter - (float)text.StartFade) / ((float)text.EndFade - (float)text.StartFade);
                    text.FinalColor = new Color((byte)(text.Color.R * scale), (byte)(text.Color.G * scale), (byte)(text.Color.B * scale), (byte)(text.Color.A * scale));
                }
                else
                    text.FinalColor = text.Color;
                
                // Movement
                if (text.MovementStart <= text.Counter)
                    text.Location += text.Direction;

                // Removal
                if (text.Counter > text.EndFade)
                    Texts.Remove(text);

            }

        }

        public void Draw(SpriteBatch spriteBatch, Camera camera, SpriteFont font)
        {

            foreach (FloatingText text in Texts)
            {
                TextHelper.DrawString(spriteBatch, font, text.Message, text.Location + camera.ToVector2(), text.FinalColor, text.FinalScale, align:"center", valign: "center");
            }

        }

        public void Add(string message, Vector2 location)
        {
            Texts.Add(new FloatingText(message,location));
        }

    }

    class FloatingText
    {

        /// <summary>
        /// The string message of the floating text
        /// </summary>
        public string Message;

        /// <summary>
        /// The x,y position of the floating text
        /// </summary>
        public Vector2 Location;

        /// <summary>
        /// The original color of the floating text
        /// </summary>
        public Color Color;

        /// <summary>
        /// The color that will be used for drawing
        /// </summary>
        public Color FinalColor;

        #region Metrics

        /// <summary>
        /// The font scaling for the text at its height
        /// </summary>
        public float High = 3.0f;

        /// <summary>
        /// The font scaling for the text at its lowest point
        /// </summary>
        public float Low = 0.6f;

        /// <summary>
        /// The font scaling at its standard point
        /// </summary>
        public float Standard = 1f;

        /// <summary>
        /// The scale that is used when drawing
        /// </summary>
        public float FinalScale;

        /// <summary>
        /// The direction of the floating text
        /// </summary>
        public Vector2 Direction = new Vector2(0,-1);

        #endregion

        #region Timing

        /// <summary>
        /// How many frames have so far been played out
        /// </summary>
        public byte Counter = 0;

        /// <summary>
        /// When this many frames have been played out, start fading out
        /// </summary>
        public byte StartFade = 20;

        /// <summary>
        /// When this many frames is reached, the text will be full faded out and deleted
        /// </summary>
        public byte EndFade = 40;

        /// <summary>
        /// The point at which the scaling animation switches from High-Low to Low-Standard
        /// </summary>
        public byte ScaleSwitch = 8;

        /// <summary>
        /// The point at which the scaling animation finishes
        /// </summary>
        public byte ScaleEnd = 16;

        /// <summary>
        /// The point when the text will start moving
        /// </summary>
        public byte MovementStart = 5;

        #endregion

        public FloatingText(string message, Vector2 location)
        {
            Random rand = new Random();
            Message = message;
            Location = location += new Vector2((float)rand.Next(-1000, 1000) / 100f, (float)rand.Next(-1000, 1000) / (float)100);
            Color = Color.DarkOrange;
            Direction = new Vector2((float)rand.Next(-100, 100) / 100f, (float)rand.Next(-100, 100) / (float)100);

            High = (float)rand.Next(20, 40) / 10f;
        }

    }
}
