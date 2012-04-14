using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Blueprint
{
    class Weapon
    {

        public WeaponType Type;
        public string Name;
        public Rectangle Sprite;

        public Weapon(WeaponType type, string name, Rectangle sprite)
        {
            Type = type;
            Name = name;
            Sprite = sprite;
        }

    }
}
