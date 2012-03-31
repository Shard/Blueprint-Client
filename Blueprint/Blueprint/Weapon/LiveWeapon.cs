using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Blueprint
{
    class LiveWeapon
    {

        public Vector2 Location;
        public Weapon Weapon;
        public float Rotation;
        public string Direction; // The direction of the player when the weapon was created

        public float SwingStart;
        public float SwingEnd;
        public float Speed;

        public LiveWeapon(Weapon weapon, Vector2 location, string direction)
        {
            Weapon = weapon;
            Location = location;
            Rotation = weapon.Type.SwingStart;
            Direction = direction;

            if (Direction == "left") { SwingStart = Weapon.Type.SwingStart * -1; } else { SwingStart = Weapon.Type.SwingStart; }
            if (Direction == "left") { SwingEnd = Weapon.Type.SwingEnd * -1; } else { SwingEnd = Weapon.Type.SwingEnd; }
            if (Direction == "left") { Speed = 0.1f * -1; } else { Speed = 0.1f; }
        }

    }
}
