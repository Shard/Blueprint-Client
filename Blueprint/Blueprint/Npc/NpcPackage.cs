using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Blueprint
{
    class NpcPackage
    {

        // Collections
        public List<ActiveNpc> ActiveNpcs;
        public NpcRace[] Races;
        public NpcAi[] Ai;
        public Npc[] Npcs;

        // Other
        public Texture2D NpcTexture;

        public NpcPackage()
        {

            ActiveNpcs = new List<ActiveNpc>();
            Ai = new NpcAi[10];
            Races = new NpcRace[10];
            Npcs = new Npc[10];

        }

        public void Initialize(Texture2D npcTexture)
        {
            NpcTexture = npcTexture;
        }

        public void Update( Control control )
        {

        }

        public void Draw( SpriteBatch spriteBatch )
        {

        }

    }
}
