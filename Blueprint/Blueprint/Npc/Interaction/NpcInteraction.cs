using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Blueprint
{
    class NpcInteraction
    {

        public enum NpcInteractionState
        {
            None,
            Intro,
            Shop,
            Gossip
        }
        public NpcInteractionState State;

        public void Initialize()
        {

        }

        public void Draw(SpriteBatch spriteBatch, SpriteFont font)
        {
            if (State == NpcInteractionState.Intro)
            {
                TextHelper.DrawString(spriteBatch, font, "Intro", new Vector2(100, 100), Color.White);
            }
        }
        

    }
}
