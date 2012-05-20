using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Blueprint
{

    /// <summary>
    /// Base class inherited by weapons
    /// </summary>
    class LiveWeapon
    {

        public Vector2 Location;
        public Weapon Weapon;
        public float Rotation;

        public float Speed;

        public RotatedRectangle currentLocation
        {
            get { return new RotatedRectangle(new Rectangle((int)Location.X,(int)Location.Y, Weapon.Sprite.Width, Weapon.Sprite.Height), Rotation); }
        }

        public LiveWeapon(Weapon weapon, Vector2 location)
        {
            Weapon = weapon;
            Location = location;
        }

    }
}
