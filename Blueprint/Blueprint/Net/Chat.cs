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
        private List<Keys> TypedKeys;

        // States
        public bool IsTyping;
        public ChatWriter Writer;

        public Chat()
        {
            Log = new List<ChatMessage>();
            Max = 10;
            FadeAt = 3;
            IsTyping = false;
            Writer = new ChatWriter();

            TypedKeys = new List<Keys>();
            TypedKeys.Add((Keys.A));
            TypedKeys.Add((Keys.B));
            TypedKeys.Add((Keys.C));
            TypedKeys.Add((Keys.D));
            TypedKeys.Add((Keys.E));
            TypedKeys.Add((Keys.F));
            TypedKeys.Add((Keys.G));
            TypedKeys.Add((Keys.H));
            TypedKeys.Add((Keys.I));
            TypedKeys.Add((Keys.J));
            TypedKeys.Add((Keys.K));
            TypedKeys.Add((Keys.L));
            TypedKeys.Add((Keys.M));
            TypedKeys.Add((Keys.N));
            TypedKeys.Add((Keys.O));
            TypedKeys.Add((Keys.P));
            TypedKeys.Add((Keys.Q));
            TypedKeys.Add((Keys.R));
            TypedKeys.Add((Keys.S));
            TypedKeys.Add((Keys.T));
            TypedKeys.Add((Keys.U));
            TypedKeys.Add((Keys.V));
            TypedKeys.Add((Keys.W));
            TypedKeys.Add((Keys.X));
            TypedKeys.Add((Keys.Y));
            TypedKeys.Add((Keys.Z));
        }

        public void Update( Control control, GameTime time  )
        {
            if (control.previousKeyboard.IsKeyUp(Keys.Enter) && control.currentKeyboard.IsKeyDown(Keys.Enter))
            {
                if (IsTyping)
                {
                    IsTyping = false;
                    if (Writer.Message.Length > 0)
                    {
                        Add(Writer.Message.ToString(), time);
                        Writer.Clear();
                    }
                }
                else
                {
                    IsTyping = true;
                }
            }

            if (IsTyping)
            {
                foreach (Keys key in TypedKeys)
                {
                    if (control.previousKeyboard.IsKeyUp(key) && control.currentKeyboard.IsKeyDown(key))
                    {
                        Writer.Type(key.ToString());
                    }
                }
                if (control.previousKeyboard.IsKeyUp(Keys.Back) && control.currentKeyboard.IsKeyDown(Keys.Back))
                {
                    Writer.Backspace();
                }
                if (control.previousKeyboard.IsKeyUp(Keys.Delete) && control.currentKeyboard.IsKeyDown(Keys.Delete))
                {
                    Writer.Delete();
                }
                if (control.previousKeyboard.IsKeyUp(Keys.Left) && control.currentKeyboard.IsKeyDown(Keys.Left))
                {
                    Writer.MoveCursor(-1);
                }
                if (control.previousKeyboard.IsKeyUp(Keys.Right) && control.currentKeyboard.IsKeyDown(Keys.Right))
                {
                    Writer.MoveCursor(1);
                }
            }
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

            if (IsTyping)
            {
                spriteBatch.DrawString(font, Writer.Message.ToString(), new Vector2(100,100), Color.White);
                spriteBatch.DrawString(font, "|", new Vector2(96 + (11 * Writer.Cursor), 100), Color.Red);
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
