using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace Blueprint
{
    class ChatMessage
    {

        public string Message;
        public string Type;
        public int Created;
        public byte Opacity;
        public string Author;

        public ChatMessage(string message, GameTime time, string author = null)
        {
            Message = message;
            Type = "normal";
            Author = author;
            Created = time.TotalGameTime.Seconds;
            Opacity = 255;
        }

    }
}
