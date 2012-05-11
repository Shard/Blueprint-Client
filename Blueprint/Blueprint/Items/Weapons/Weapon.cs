using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Blueprint
{
    class Weapon
    {

        public int Id;
        public WeaponType Type;
        public string Name;
        public Rectangle Sprite;

        public Weapon(int id, WeaponType type, string name, Rectangle sprite)
        {
            Id = id;
            Type = type;
            Name = name;
            Sprite = sprite;
        }

    }
}
