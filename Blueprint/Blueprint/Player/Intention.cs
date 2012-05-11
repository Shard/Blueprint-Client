using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blueprint
{

    /// <summary>
    /// Used to describe what a player is trying to do, used extensivly in the Player class and networking classes.
    /// </summary>
    class Intention
    {

        public bool Left;
        public bool Right;
        public bool Jumping;
        public bool Sprinting;

        public Intention()
        {
            Left = false;
            Right = false;
            Jumping = false;
            Sprinting = false;
        }

        public Intention(string message)
        {
            if (message.Length == 4)
            {
                Left = (message[0] == '1');
                Right = (message[1] == '1');
                Jumping = (message[2] == '1');
                Sprinting = (message[3] == '1');
            }
        }

        public Intention(bool moveLeft, bool moveRight, bool jumping, bool sprinting)
        {
            Left = moveLeft;
            Right = moveRight;
            Jumping = jumping;
            Sprinting = sprinting;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            if (Left) { builder.Append('1'); } else { builder.Append('0'); }
            if (Right) { builder.Append('1'); } else { builder.Append('0'); }
            if (Jumping) { builder.Append('1'); } else { builder.Append('0'); }
            if (Sprinting) { builder.Append('1'); } else { builder.Append('0'); }

            return builder.ToString();
        }

        /// <summary>
        /// Stop all movement aspirations
        /// </summary>
        public void Stop()
        {
            Left = false;
            Right = false;
            Jumping = false;
            Sprinting = false;
        }

    }
}
