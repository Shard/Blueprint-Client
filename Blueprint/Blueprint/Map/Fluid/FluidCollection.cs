using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Blueprint.Fluid
{
    class FluidCollection
    {

        public Fluid Water;
        public Texture2D Texture;

        public FluidCollection()
        {
            Water = new FluidWater();
        }

        public void Initialize(int width, int height, Texture2D texture)
        {
            Water.Initialize(width, height);
            Texture = texture;
        }

        public void Update(Map map)
        {
            Water.Update(ref map);
        }

        public void Draw(SpriteBatch spriteBatch, Camera camera)
        {
            Water.Draw(spriteBatch, Texture, camera);
        }

    }
}
