using System;
using System.Collections.Generic;

namespace Blueprint
{
    class Weapon
    {

        public WeaponType Type;
        public string Name;

        public Weapon(WeaponType type, string name)
        {
            Type = type;
            Name = name;
        }

    }
}
