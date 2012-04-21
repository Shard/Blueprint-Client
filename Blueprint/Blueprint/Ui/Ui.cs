using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Blueprint.Ui
{
    class Ui
    {

        public FloatingTextCollection FloatingTexts;

        public Ui()
        {
            FloatingTexts = new FloatingTextCollection();
        }

        public void Initialize()
        {

        }

        public void Update()
        {
            FloatingTexts.Update();
        }

        public void Draw(SpriteBatch spriteBatch, Camera camera, SpriteFont font)
        {
            FloatingTexts.Draw(spriteBatch, camera, font);
        }

    }
}
