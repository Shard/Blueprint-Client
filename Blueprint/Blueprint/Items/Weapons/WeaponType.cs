using System;


namespace Blueprint
{
    class WeaponType
    {

        public string Name;
        public bool Wieldable;
        public bool IsAmmo;

        public WeaponType(string name, bool wieldable)
        {
            Name = name;
            Wieldable = wieldable;
            IsAmmo = false;
        }

    }
}
