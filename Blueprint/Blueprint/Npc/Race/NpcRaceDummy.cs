using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Blueprint
{
    class NpcRaceDummy : NpcRace
    {

        public NpcRaceDummy()
        {
            Name = "Dummy Race";
            Health = 1000;
            Mana = 100;
            DefaultSprite = new Rectangle(0, 0, 25, 48);
        }

    }
}
