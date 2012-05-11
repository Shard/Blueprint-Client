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

        public float Speed;

        public RotatedRectangle currentLocation
        {
            get
            {
                RotatedRectangle rect = new RotatedRectangle(new Rectangle((int)Location.X,(int)Location.Y, Weapon.Sprite.Width, Weapon.Sprite.Height), Rotation);
                rect.Origin = new Vector2(rect.Width, rect.Height);
                return rect;

            }
        }

        public LiveWeapon(Weapon weapon, Vector2 location)
        {
            Weapon = weapon;
            Location = location;
        }

    }
}
