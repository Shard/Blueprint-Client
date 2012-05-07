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
