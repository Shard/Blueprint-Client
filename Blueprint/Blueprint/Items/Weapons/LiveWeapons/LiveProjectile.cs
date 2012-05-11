using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Blueprint
{
    class LiveProjectile : LiveWeapon
    {

        public Movement Movement;

        /// <summary>
        /// Time to live once the projectile has landed
        /// </summary>
        public int TimeToLive;

        public LiveProjectile(Weapon weapon, Vector2 location, float force, Control control, Camera camera)
            : base(weapon, location)
        {
            Movement = new Movement(location, 24, 6);
            Movement.Gravity = 0.15f;
            Movement.Drag = 0.06f;
            Movement.PushbackFrom(new Vector2(control.currentMouse.X - camera.X, control.currentMouse.Y - camera.Y), force);
            TimeToLive = 250;
        }

    }
}
