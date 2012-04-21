using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Blueprint
{
    class FurniturePackage
    {

        public Texture2D Texture;

        public Furniture[] Furniture;
        public FurnitureType[] Types;

        public FurniturePackage()
        {
            Types = new FurnitureType[10];
            Types[0] = new FurnitureType(1, "flower", new Rectangle(0, 0, 0, 0), false, 1,1);

            Furniture = new Furniture[10];
            Furniture[0] = new Furniture(Types[0], 7,7);
        }

        public void Initialize(Texture2D texture)
        {
            Texture = texture;
        }

        public void Update()
        {

        }

        public void Draw(SpriteBatch spriteBatch, Camera camera)
        {

            foreach (Furniture item in Furniture)
            {
                if (item == null) { continue; }
                spriteBatch.Draw(Texture, camera.FromRectangle(item.Area), Color.White);
            }

        }

    }
}
