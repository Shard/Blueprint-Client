using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Blueprint
{
    class LiveSword : LiveWeapon
    {

        public float SwingStart;
        public float SwingEnd;
        public string Direction; // The direction of the player when the weapon was created


        public LiveSword(Weapon weapon, Vector2 location, string direction)
            : base(weapon, location)
        {

            SwingStart = 0f;
            SwingEnd = 2.5f;

            Direction = direction;
            if (Direction == "left") { SwingStart = SwingStart * -1; }
            if (Direction == "left") { SwingEnd = SwingEnd * -1; }
            if (Direction == "left") { Speed = 0.1f * -1; } else { Speed = 0.1f; }
            Rotation = SwingStart;
        }

    }
}
