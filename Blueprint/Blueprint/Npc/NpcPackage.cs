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

            // Ai
            Ai[0] = new NpcAiDummy();
            Ai[1] = new NpcAiChase();

            // Races
            Races[0] = new NpcRaceDummy();

            // Npcs
            Npcs[0] = new Npc("The guide", Races[0], Ai[1]);

            // Active Npcs
            ActiveNpcs.Add(new ActiveNpc(Npcs[0], new Vector2(100,-50)));
 
        }

        public void Update( Map map )
        {
            for (int i = 0; i < ActiveNpcs.Count; i++)
            {
                //ActiveNpcs[i].Npc.Ai.Update(ActiveNpcs[i].Movement,);
                ActiveNpcs[i].Movement.Update(map);
                ActiveNpcs[i].Invunerable -= 1;
            }
        }

        public void Draw( SpriteBatch spriteBatch, Camera camera )
        {
            for (int i = 0; i < ActiveNpcs.Count; i++)
            {
                spriteBatch.Draw(NpcTexture, camera.FromRectangle(ActiveNpcs[i].Movement.Area), ActiveNpcs[i].Npc.Race.DefaultSprite, Color.White);
            }
        }

        /// <summary>
        /// Does damage to a unit
        /// </summary>
        /// <param name="npc"></param>
        /// <param name="amount"></param>
        /// <param name="pushbackFrom"></param>
        /// <returns>Returns true if damage was done</returns>
        public bool Damage(ActiveNpc npc, int amount, Vector2 pushbackFrom)
        {
            if (npc.Invunerable > 0) { return false; }
            npc.Health -= amount;
            npc.Invunerable = 30;
            npc.Movement.PushbackFrom(pushbackFrom, 4f);
            if (npc.Health <= 0){ActiveNpcs.Remove(npc);}
            return true;
        }

    }
}
