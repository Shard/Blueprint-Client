using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Blueprint
{
    class Chat
    {

        /// <summary>
        /// The raw chat log
        /// </summary>
        public List<ChatMessage> Log;

        /// <summary>
        /// Maximun amount of chat messages to show at one time
        /// </summary>
        public int Max;

        /// <summary>
        /// How many frames before to start fading
        /// </summary>
        public int FadeAt;

        /// <summary>
        /// Keys to watch for and allow when the user is typing
        /// </summary>
        private List<Keys> TypedKeys;

        /// <summary>
        /// How many frames the backspace key has been held for
        /// </summary>
        private byte BackspaceHold;

        /// <summary>
        /// Used for spacing out the backspace deletion, lowering its speed
        /// </summary>
        private byte BackspaceHold2;

        /// <summary>
        /// The chat writter class which contains low level functions for maniupulating chat messages
        /// </summary>
        public ChatWriter Writer;

        private Texture2D Texture;

        /// <summary>
        /// How many frames then fading should be locked for
        /// </summary>
        private int LockFade = 0;

        private bool CapsLock = false;

        /// <summary>
        /// Is true if the user is currently typing
        /// </summary>
        private bool Typing;

        public Chat()
        {
            Log = new List<ChatMessage>();
            Max = 10;
            FadeAt = 10;
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
            TypedKeys.Add((Keys.NumPad0));
            TypedKeys.Add((Keys.NumPad1));
            TypedKeys.Add((Keys.NumPad2));
            TypedKeys.Add((Keys.NumPad3));
            TypedKeys.Add((Keys.NumPad4));
            TypedKeys.Add((Keys.NumPad5));
            TypedKeys.Add((Keys.NumPad6));
            TypedKeys.Add((Keys.NumPad7));
            TypedKeys.Add((Keys.NumPad8));
            TypedKeys.Add((Keys.NumPad9));
            TypedKeys.Add(Keys.Space);
            TypedKeys.Add(Keys.OemQuotes);
            TypedKeys.Add(Keys.OemQuestion);
            TypedKeys.Add(Keys.OemSemicolon);
            TypedKeys.Add(Keys.OemPeriod);
            TypedKeys.Add(Keys.OemComma);
            TypedKeys.Add(Keys.OemOpenBrackets);
            TypedKeys.Add(Keys.OemCloseBrackets);
            TypedKeys.Add(Keys.D0);
            TypedKeys.Add(Keys.D1);
            TypedKeys.Add(Keys.D2);
            TypedKeys.Add(Keys.D3);
            TypedKeys.Add(Keys.D4);
            TypedKeys.Add(Keys.D5);
            TypedKeys.Add(Keys.D6);
            TypedKeys.Add(Keys.D7);
            TypedKeys.Add(Keys.D8);
            TypedKeys.Add(Keys.D9);
        }

        public void Initialize(Texture2D texture)
        {
            Texture = texture;
        }

        public void Update( ref Control control, GameTime time  )
        {

            if (LockFade > 0) { LockFade--; }

            // Toggle the opening and submitting of chat messages
            if (control.previousKeyboard.IsKeyUp(Keys.Enter) && control.currentKeyboard.IsKeyDown(Keys.Enter))
            {
                if (control.Typing)
                {
                    control.Typing = false;
                    if (Writer.Message.Length > 0) { 
                        // Submit message
                        Add(Writer.Message.ToString(), time, "Firebolt"); 
                        Writer.Clear();
                    }
                }
                else {control.Typing = true;}
            }

            // If typing
            if (control.Typing)
            {
                RefreshLog();
                
                if (control.currentKeyboard.IsKeyDown(Keys.CapsLock) && control.previousKeyboard.IsKeyUp(Keys.CapsLock))
                { if (CapsLock) { CapsLock = false; } else { CapsLock = true; } }

                bool Shift;
                if (CapsLock || control.currentKeyboard.IsKeyDown(Keys.LeftShift) || control.currentKeyboard.IsKeyDown(Keys.RightShift))
                    { Shift = true; } else { Shift = false; }

                // If key is allowed, add it
                foreach (Keys key in TypedKeys)
                {
                    if (control.previousKeyboard.IsKeyUp(key) && control.currentKeyboard.IsKeyDown(key))
                    {
                        if (key == Keys.Space) { Writer.Type(" ");  } else
                        if (key == Keys.OemQuotes) { if (Shift) { Writer.Type("\""); } else { Writer.Type("'"); } } else 
                        if (key == Keys.OemQuestion) { if (Shift) { Writer.Type("?"); } else { Writer.Type("/"); }; } else 
                        if (key == Keys.OemComma) { if (Shift) { Writer.Type("<"); } else { Writer.Type(","); } } else 
                        if (key == Keys.OemPeriod) { if (Shift) { Writer.Type(">"); } else { Writer.Type("."); } } else 
                        if (key == Keys.OemSemicolon) { if (Shift) { Writer.Type(":"); } else { Writer.Type(";"); } } else
                        if (key == Keys.OemOpenBrackets) { if (Shift) { Writer.Type("{"); } else { Writer.Type("["); } } else 
                        if (key == Keys.OemCloseBrackets) { if (Shift) { Writer.Type("}"); } else { Writer.Type("]"); } } else 
                        if (key == Keys.D0) { if (Shift) { Writer.Type(")"); } else { Writer.Type("0"); } } else 
                        if (key == Keys.D1) { if (Shift) { Writer.Type("!"); } else { Writer.Type("1"); } } else 
                        if (key == Keys.D2) { if (Shift) { Writer.Type("@"); } else { Writer.Type("2"); } } else 
                        if (key == Keys.D3) { if (Shift) { Writer.Type("#"); } else { Writer.Type("3"); } } else 
                        if (key == Keys.D4) { if (Shift) { Writer.Type("$"); } else { Writer.Type("4"); } } else 
                        if (key == Keys.D5) { if (Shift) { Writer.Type("%"); } else { Writer.Type("5"); } } else 
                        if (key == Keys.D6) { if (Shift) { Writer.Type("^"); } else { Writer.Type("6"); } } else 
                        if (key == Keys.D7) { if (Shift) { Writer.Type("&"); } else { Writer.Type("7"); } } else
                        if (key == Keys.D8) { if (Shift) { Writer.Type("*"); } else { Writer.Type("8"); } } else 
                        if (key == Keys.D9) { if (Shift) { Writer.Type("("); } else { Writer.Type("9"); } }  
                        else
                            if (CapsLock || control.currentKeyboard.IsKeyDown(Keys.LeftShift) || control.currentKeyboard.IsKeyDown(Keys.RightShift))
                            { Writer.Type(key.ToString()); }
                            else
                            { Writer.Type(key.ToString().ToLower()); }
                    }
                }

                // User presses backspace
                if (control.previousKeyboard.IsKeyUp(Keys.Back) && control.currentKeyboard.IsKeyDown(Keys.Back))
                    { Writer.Backspace(); BackspaceHold = 0; }

                // User holds the backspace key
                if (control.currentKeyboard.IsKeyDown(Keys.Back))
                {
                    BackspaceHold++;
                    if (BackspaceHold > 30)
                    {
                        BackspaceHold2++;
                        if (BackspaceHold2 >= 2)
                        { Writer.Backspace(); BackspaceHold2 = 0; }
                    }
                }

                // User presses delete
                if (control.previousKeyboard.IsKeyUp(Keys.Delete) && control.currentKeyboard.IsKeyDown(Keys.Delete)) { Writer.Delete(); }

                // Navigate cursor
                if (control.previousKeyboard.IsKeyUp(Keys.Left) && control.currentKeyboard.IsKeyDown(Keys.Left)) {Writer.MoveCursor(-1);}
                if (control.previousKeyboard.IsKeyUp(Keys.Right) && control.currentKeyboard.IsKeyDown(Keys.Right)){ Writer.MoveCursor(1); }
            }

            // Update typing status
            Typing = control.Typing;
        }

        public void Draw(SpriteBatch spriteBatch, SpriteFont font, GameTime time)
        {

            Rectangle viewport = spriteBatch.GraphicsDevice.Viewport.TitleSafeArea;
            int upto = 0;
            for (int i = Log.Count - 1; i >= 0; i--)
            {
                if (Log[i].Opacity == 0) { continue; }
                if( time.TotalGameTime.Seconds - Log[i].Created > FadeAt && LockFade == 0 ){
                    Log[i].Opacity--;
                }
                
                // Draw Chat line
                float offset = 0;
                if (Log[i].Author != null)
                {
                    offset = Log[i].Author.Length * (11 * 0.7f) + 13;
                    TextHelper.DrawString(spriteBatch, font, Log[i].Author + ":", new Vector2(10, viewport.Height - (upto * 20) - 50), new Color(Log[i].Opacity / 2, Log[i].Opacity, Log[i].Opacity, Log[i].Opacity), 0.7f);
                }
                TextHelper.DrawString(spriteBatch, font, Log[i].Message, new Vector2(10 + offset, viewport.Height - (upto * 20) - 50), new Color(Log[i].Opacity, Log[i].Opacity, Log[i].Opacity, Log[i].Opacity), 0.7f);
                upto++;
                if (upto > Max) { break; }
            }

            // Draw pointer
            if (Typing)
            {
                spriteBatch.Draw(Texture, new Rectangle(10, viewport.Height - 30, 274, 22), Color.Black);
                TextHelper.DrawString(spriteBatch, font, Writer.Message.ToString(), new Vector2(10, viewport.Height - 25), Color.White, 0.7f);
                TextHelper.DrawString(spriteBatch, font, "|", new Vector2(8 + ((11 * 0.7f) * Writer.Cursor), viewport.Height - 25), Color.White, 0.7f);
            }

        }

        public void RefreshLog()
        {
            for (int i = Log.Count - 1; i >= 0; i--)
            {
                Log[i].Opacity = (byte)255;
            }
            LockFade = 250;
        }

        public void ParseCommand(string message)
        {

        }

        #region Add Functions

        public bool Add(List<ChatMessage> Messages)
        {
            foreach (ChatMessage message in Messages)
            {
                Log.Add(message);
            }
            Messages.Clear();
            return true;
        }

        public bool Add(List<string> Messages, GameTime time, string author = null)
        {
            foreach (string message in Messages)
            {
                Log.Add(new ChatMessage(message, time, author));
            }
            Messages.Clear();
            return true;
        }

        public bool Add(string message, GameTime time, string author = null)
        {
            Log.Add(new ChatMessage(message, time, author));
            return true;
        }

        #endregion

    }
}
