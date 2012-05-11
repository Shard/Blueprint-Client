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
        public NpcInteraction Interaction;

        // Other
        public Texture2D NpcTexture;

        public NpcPackage()
        {
            Interaction = new NpcInteraction();
            ActiveNpcs = new List<ActiveNpc>();
            Ai = new NpcAi[10];
            Races = new NpcRace[10];
            Npcs = new Npc[10];

        }

        public void Initialize(Texture2D npcTexture)
        {
            NpcTexture = npcTexture;
            Interaction.Initialize();

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

        public void Update( Map map, Player player, Control control, Camera camera   )
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
                    MovementChase(npc.Movement, npc.CurrentDestination);
                else
                    npc.Movement.Intention.Stop();
                ActiveNpcs[i].Movement.Update(map);

                #region Interaction

                if (control.MousePos.Intersects(camera.FromRectangle(npc.Movement.Area)))
                {
                    control.State = Control.CursorStates.Interact;
                    if (control.Click(false))
                        Interaction.State = NpcInteraction.NpcInteractionState.Intro;
                }

                if (Interaction.State != NpcInteraction.NpcInteractionState.None)
                {
                    if (Geometry.Range(player.Movement.Area, npc.Movement.Area) > 5)
                        Interaction.State = NpcInteraction.NpcInteractionState.None;
                }

                #endregion

                #region Damage / Combat

                if(npc.Movement.Area.Intersects(player.Movement.Area))
                {
                    player.Damage(5);
                }

                #endregion

                ActiveNpcs[i].Invunerable -= 1;
            }


        }

        public void Draw( SpriteBatch spriteBatch, Camera camera, SpriteFont font )
        {
            for (int i = 0; i < ActiveNpcs.Count; i++)
            {
                spriteBatch.Draw(NpcTexture, camera.FromRectangle(ActiveNpcs[i].Movement.Area), ActiveNpcs[i].Npc.Race.DefaultSprite, Color.White);
            }
            Interaction.Draw(spriteBatch, font);
        }

        private void MovementChase(Movement movement, Point dest)
        {
            MovementChase(movement, new Rectangle(dest.X * 24, dest.Y * 24, 24,24));
        }

        /// <summary>
        /// Applies intentions to movement related to moving towards dest
        /// </summary>
        /// <param name="movement"></param>
        /// <param name="dest"></param>
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
