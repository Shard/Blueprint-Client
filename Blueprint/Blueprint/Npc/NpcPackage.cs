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

        public void Update( Map map, Player player )
        {
            for (int i = 0; i < ActiveNpcs.Count; i++)
            {
                //ActiveNpcs[i].Npc.Ai.Update(ActiveNpcs[i].Movement, player);

                ActiveNpc npc = ActiveNpcs[i];

                // Follow the path
                if (npc.CurrentPath != null)
                {
                    if (npc.CurrentDestination == Point.Zero)
                        npc.CurrentDestination = npc.CurrentPath.Pop();

                    if (npc.Movement.Area.Intersects(new Rectangle(npc.CurrentDestination.X * 24, npc.CurrentDestination.Y * 24, 24, 24)))
                    {
                        if (npc.CurrentPath.Count > 0)
                        { npc.CurrentDestination = npc.CurrentPath.Pop(); }
                        else 
                            { npc.CurrentPath = null; npc.CurrentDestination = Point.Zero; }
                            
                    }
                }

                if (npc.CurrentDestination != Point.Zero)
                {
                    MovementChase(npc.Movement, npc.CurrentDestination);
                }
                else
                {
                    npc.Movement.Intention.Stop();
                }
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

        private void MovementChase(Movement movement, Point dest)
        {
            MovementChase(movement, new Rectangle(dest.X * 24, dest.Y * 24, 24,24));
        }

        private void MovementChase(Movement movement, Rectangle dest)
        {
            if (movement.Area.Center.X > dest.Center.X)
            {
                movement.Intention.Left = true;
                movement.Intention.Right = false;
            }
            else
            {
                movement.Intention.Left = false;
                movement.Intention.Right = true;
            }

            Console.WriteLine(movement.Area.Center.Y.ToString() + " - " + dest.Bottom.ToString());

            if (movement.Area.Center.Y >= dest.Bottom)
                movement.Intention.Jumping = true;
            else
                movement.Intention.Jumping = false;

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
            npc.Movement.PushbackFrom(pushbackFrom, 8f);
            if (npc.Health <= 0){ActiveNpcs.Remove(npc);}
            return true;
        }

    }
}
