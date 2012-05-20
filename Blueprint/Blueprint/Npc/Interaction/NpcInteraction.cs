using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Blueprint
{
    class NpcInteraction
    {

        public List<NpcInteractionAction> Actions;

        #region States

        /// <summary>
        /// Enum of all avaliable npc interaction states
        /// </summary>
        public enum NpcInteractionState
        {
            None,
            Intro,
            Shop,
            Gossip
        }

        /// <summary>
        /// The current npc animation state
        /// </summary>
        public NpcInteractionState State;

        #endregion

        #region Ui

        /// <summary>
        /// The width of the current interaction window
        /// </summary>
        public int Width = 350;

        /// <summary>
        /// The height of the current interaction window
        /// </summary>
        public int Height;

        /// <summary>
        /// The current text content used in the dialog box
        /// </summary>
        private string CurrentText = "Collaboratively administrate empowered markets via plug-and-play networks.Collaboratively administrate empowered markets via plug-and-play networks.";

        /// <summary>
        /// Count of the number of dialog lines are current being shown
        /// </summary>
        private int CurrentLines;

        /// <summary>
        /// The area of the current dialog body
        /// </summary>
        private Rectangle CurrentBody;

        /// <summary>
        /// The area of the current dialog header
        /// </summary>
        private Rectangle CurrentHeader;

        /// <summary>
        /// A string containing the current dialog selection (Hover)
        /// </summary>
        private string CurrentSelection;

        /// <summary>
        /// Set when there is a change in UI, allows the UI to be delayed by 1 frame to give it time for the game logic to catch up
        /// </summary>
        private bool SkipFrame;

        #endregion

        #region Settings

        /// <summary>
        /// The scaling used for the action text
        /// </summary>
        public float ScaleActions = 0.7f;

        /// <summary>
        /// The scaling used for the main text
        /// </summary>
        public float ScaleText = 0.75f;

        

        #endregion

        /// <summary>
        /// The npc the current user is interacting with
        /// </summary>
        public ActiveNpc Subject;


        public void Update(Camera camera, Control control, SpriteFont font)
        {

            #region Setup Dialog Area

            if (State == NpcInteractionState.Intro || State == NpcInteractionState.Gossip)
            {
                // Calculate dialog area
                CurrentLines = TextHelper.SplitWrap(CurrentText, 330, font, 0.75f).Length;
                CurrentBody = camera.FromRectangle(new Rectangle(Subject.Movement.Area.Center.X - Width / 2, Subject.Movement.Area.Top - Height - 20, Width, Height));
                CurrentHeader = camera.FromRectangle(new Rectangle(Subject.Movement.Area.Center.X - Width / 2, Subject.Movement.Area.Top - Height - 52, Width, 32));
                Height = 30 + (int)(font.MeasureString("S").Y - 4) * (CurrentLines + 1);

                if (control.MousePos.Intersects(CurrentBody))
                    control.MouseLock = true;
            }

            #endregion

            #region Intro

            if (State == NpcInteractionState.Intro)
            {

                // Actions
                Vector2 footer_offset = new Vector2(CurrentBody.X + 10, CurrentBody.Y + Height - 25);
                CurrentSelection = "";
                Vector2 current_font;

                // if shop
                current_font = font.MeasureString("Shop") * ScaleActions;
                if (control.MousePos.Intersects(new Rectangle((int)footer_offset.X, (int)footer_offset.Y, (int)current_font.X, (int)current_font.Y)))
                {
                    CurrentSelection = "Shop";
                    if (control.Click()) { State = NpcInteractionState.Shop; }
                }
                footer_offset.X += (int)(font.MeasureString("Shop").X * 0.7f) + 10;

                // if gossip
                current_font = font.MeasureString("Gossip") * ScaleActions;
                if (control.MousePos.Intersects(new Rectangle((int)footer_offset.X, (int)footer_offset.Y, (int)current_font.X, (int)current_font.Y)))
                {
                    CurrentSelection = "Gossip";
                    if (control.Click()) { State = NpcInteractionState.Gossip; CurrentText = Subject.Type.Dialog.Get(State).Line; }
                }
                footer_offset.X += (int)(font.MeasureString("Gossip").X * 0.7f) + 10;

                // if follow
                current_font = font.MeasureString("Follow Me") * ScaleActions;
                if (control.MousePos.Intersects(new Rectangle((int)footer_offset.X, (int)footer_offset.Y, (int)current_font.X, (int)current_font.Y)))
                {
                    CurrentSelection = "Follow Me";
                    if (control.Click()) { State = NpcInteractionState.None; }
                }
                footer_offset.X += (int)(font.MeasureString("Follow Me").X * 0.7f) + 10;

            }

            #endregion

        }

        public void Draw(SpriteBatch spriteBatch, SpriteFont font, Camera camera, Texture2D uiTexture)
        {

            #region Other

            if (SkipFrame == true)
            {
                SkipFrame = false;
                return;
            }

            // Setup Dialog Box
            Rectangle npc_area;
            Rectangle sprite;
            Rectangle bust_dest;
            Rectangle bust_source;

            if (State == NpcInteractionState.Intro || State == NpcInteractionState.Gossip)
            {
                // Setup the dialog box
                npc_area = Subject.Movement.Area;
                sprite = Subject.Type.Race.Animation.CurrentFrame.ToRectangle();
                bust_dest = new Rectangle(CurrentHeader.X + 5, CurrentHeader.Y + CurrentHeader.Height - sprite.Height / 2, sprite.Width, sprite.Height / 2);
                bust_source = new Rectangle(sprite.X, sprite.Y, sprite.Width, sprite.Height / 2);

                // Draw the dialog box
                spriteBatch.Draw(Subject.Type.Race.Texture, bust_dest, bust_source, Color.White); // Bust
                TextHelper.DrawString(spriteBatch, font, Subject.Name, new Vector2(CurrentHeader.X + 20 + bust_dest.Width, CurrentHeader.Y + 10), Color.White); // Nametag
                //spriteBatch.Draw(uiTexture, CurrentHeader, new Rectangle(50, 0, 50, 50), Color.White); // CurrentHeader gradient
                spriteBatch.Draw(uiTexture, new Rectangle(CurrentBody.Center.X - 25, CurrentBody.Bottom, 50, 50), new Rectangle(100, 0, 50, 50), Color.White); // Spearch bubble arrow
                spriteBatch.Draw(uiTexture, CurrentBody, new Rectangle(0, 0, 50, 50), Color.White);
            }

            #endregion

            #region Intro

            if (State == NpcInteractionState.Intro)
            {

                TextHelper.DrawString(spriteBatch, font, CurrentText, new Vector2(CurrentBody.X + 10, CurrentBody.Y + 10), Color.White, 0.75f, width_wrap: 330);

                // Footer actions
                Vector2 footer_offset = new Vector2(CurrentBody.X + 10, CurrentBody.Y + Height - 25);
                Color color;

                // If shop
                if (CurrentSelection == "Shop") { color = Color.DeepSkyBlue; } else { color = Color.White; }
                TextHelper.DrawString(spriteBatch, font, "Shop", footer_offset, color, 0.7f);
                footer_offset.X += (int)(font.MeasureString("Shop").X * 0.7f) + 10;

                // If gossip
                if (CurrentSelection == "Gossip") { color = Color.DeepSkyBlue; } else { color = Color.White; }
                TextHelper.DrawString(spriteBatch, font, "Gossip", footer_offset, color, 0.7f);
                footer_offset.X += (int)(font.MeasureString("Gossip").X * 0.7f) + 10;

                // If follow
                if (CurrentSelection == "Follow Me") { color = Color.DeepSkyBlue; } else { color = Color.White; }
                TextHelper.DrawString(spriteBatch, font, "Follow Me", footer_offset, color, 0.7f);
                footer_offset.X += (int)(font.MeasureString("Follow Me").X * 0.7f) + 10;
            }

            #endregion

            #region Gossip

            if (State == NpcInteractionState.Gossip)
            {
                TextHelper.DrawString(spriteBatch, font, CurrentText, new Vector2(CurrentBody.X + 10, CurrentBody.Y + 10), Color.White, 0.75f, width_wrap: 330);
            }

            #endregion

            #region Shop

            if (State == NpcInteractionState.Shop)
            {
                TextHelper.DrawString(spriteBatch, font, "Shop open", new Vector2(100, 100), Color.White);
            }

            #endregion
        }

        #region Helper Functions

        /// <summary>
        /// Called at the start of a conversation between the player and an npc
        /// </summary>
        /// <param name="subject">The subject npc of the interaction</param>
        public void Start(ActiveNpc subject)
        {
            Subject = subject;
            State = NpcInteractionState.Intro;
            CurrentText = subject.Type.Dialog.Get(State).Line;
            SkipFrame = true;
        }

        /// <summary>
        /// End the conversation
        /// </summary>
        public void End()
        {
            Subject = null;
            State = NpcInteractionState.None;
        }

        #endregion

    }
}
