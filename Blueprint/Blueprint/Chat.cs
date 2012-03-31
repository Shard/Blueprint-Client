using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Blueprint
{
    class Chat
    {

        public List<ChatMessage> Log;
        public int Max; // Maximun amount of messages at one time
        public int FadeAt;

        public Chat()
        {
            Log = new List<ChatMessage>();
            Max = 10;
            FadeAt = 3;
        }

        public void Update(  )
        {

        }

        public void Draw(SpriteBatch spriteBatch, SpriteFont font, GameTime time)
        {

            Rectangle viewport = spriteBatch.GraphicsDevice.Viewport.TitleSafeArea;
            int upto = 0;
            for (int i = Log.Count - 1; i >= 0; i--)
            //foreach( ChatMessage message in Log)
            {
                if( time.TotalGameTime.Seconds - Log[i].Created > FadeAt ){ // Fading
                    // Fade Out
                }
                spriteBatch.DrawString(font, Log[i].Message, new Vector2(80, viewport.Height - (upto * 20) - 30), new Color(255, 255, 255, (byte)Log[i].Opacity), 0, Vector2.Zero, 0.7f, SpriteEffects.None, 0);
                upto++;
                if (upto > Max) { break; }
            }
        }

        public bool Add(List<ChatMessage> Messages)
        {
            foreach (ChatMessage message in Messages)
            {
                Log.Add(message);
            }
            Messages.Clear();
            return true;
        }

        public bool Add(List<string> Messages, GameTime time)
        {
            foreach (string message in Messages)
            {
                Log.Add(new ChatMessage(message, time));
            }
            Messages.Clear();
            return true;
        }

        public bool Add(string message, GameTime time)
        {
            Log.Add(new ChatMessage(message, time));
            return true;
        }

    }

    class ChatMessage
    {

        public string Message;
        public string Type;
        public int Created;
        public float Opacity;

        public ChatMessage(string message, GameTime time)
        {
            Message = message;
            Type = "normal";
            Created = time.TotalGameTime.Seconds;
            Opacity = 255;
        }

    }
}
