using System;


namespace Blueprint
{
    class WeaponType
    {

        public string Name;
        public bool Wieldable;
        public bool IsAmmo;

        // Swinging Arc
        public float SwingStart;
        public float SwingEnd;

        public WeaponType(string name, bool wieldable)
        {
            Name = name;
            Wieldable = wieldable;
            IsAmmo = false;
            SwingStart = 0f;
            SwingEnd = 2.5f;
        }

    }
}
