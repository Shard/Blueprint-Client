using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Blueprint
{

    /// <summary>
    /// Contains and maintains npcs
    /// </summary>
    class NpcPackage
    {

        #region Collections

        /// <summary>
        /// A list of all active npcs
        /// </summary>
        public List<ActiveNpc> ActiveNpcs;

        /// <summary>
        /// An array of all npc races
        /// </summary>
        public NpcRace[] Races;

        /// <summary>
        /// An array of all npc ais
        /// </summary>
        public NpcAi[] Ai;

        /// <summary>
        /// An array of all Npc Types
        /// </summary>
        public NpcType[] Types;

        /// <summary>
        /// Handles player to npc interaction
        /// </summary>
        public NpcInteraction Interaction;

        #endregion

        // Other
        /// <summary>
        /// The ui texture, will be replaced will ui class
        /// </summary>
        public Texture2D UiTexture;

        public NpcPackage()
        {
            Interaction = new NpcInteraction();
            ActiveNpcs = new List<ActiveNpc>();
            Ai = new NpcAi[10];
            Races = new NpcRace[10];
            Types = new NpcType[10];

        }

        public void Initialize(ContentManager content, Package package, Texture2D uiTexture)
        {

            UiTexture = uiTexture;

            // Ai
            Ai[0] = new NpcAiDummy();
            Ai[1] = new NpcAiChase();

            // Races
            Races[0] = new NpcRace("Lynch", content.Load<Texture2D>("Npcs/Sprites/lynch"), package.LocalString("c:/blueprint/lynch.xml", false));

            // Npcs
            Types[0] = new NpcType("The Lynch", Races[0], Ai[1]);
            Types[0].Dialog.Add("Hello {playername}", NpcInteraction.NpcInteractionState.Intro);
            Types[0].Dialog.Add("Here is some interesting information {playername}", NpcInteraction.NpcInteractionState.Gossip);


            // Active Npcs
            ActiveNpc npc = new ActiveNpc(Types[0], new Vector2(200, -50));
            ActiveNpcs.Add(npc);
 
        }

        public void Update( Map map, Player player, Control control, Camera camera, SpriteFont font )
        {
            for (int i = 0; i < ActiveNpcs.Count; i++)
            {
                //ActiveNpcs[i].Npc.Ai.Update(ActiveNpcs[i].Movement, player); ai off

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
                ActiveNpcs[i].Type.Race.Animation.Update(ActiveNpcs[i].Movement);


                #region Interaction

                if (control.MousePos.Intersects(camera.FromRectangle(npc.Movement.Area)))
                {
                    control.State = Control.CursorStates.Interact;
                    if (control.Click(false))
                        Interaction.Start(npc);
                }

                if (Interaction.State != NpcInteraction.NpcInteractionState.None)
                {
                    if (Geometry.Range(player.Movement.Area, npc.Movement.Area) > 7)
                        Interaction.End();
                }

                Interaction.Update(camera, control, font);

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

            foreach (ActiveNpc npc in ActiveNpcs)
            {
                npc.Type.Race.Animation.Draw(spriteBatch, camera, npc.Type.Race.Texture);
            }

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
