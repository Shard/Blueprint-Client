using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Blueprint
{
    class FloraPackage
    {

        public Texture2D Sprite;
        public Flora[,] Flora;
        public FloraType[] Types;

        public void Initialize(Map map, Texture2D floraTexture)
        {
            Sprite = floraTexture;
            Flora = new Flora[map.SizeX, map.SizeY];
            Types = new FloraType[50];

            Types[1] = new FloraType("Grass", new Rectangle(0, 0, 24, 24), null, 10, 2);
            Types[2] = new FloraType("Grass", new Rectangle(24, 0, 24, 24), null, 10, 3);
            Types[3] = new FloraType("Grass", new Rectangle(48, 0, 24, 24), null, 10, 4);
            Types[4] = new FloraType("Grass", new Rectangle(72, 0, 24, 24), null, 10, 5);
            Types[5] = new FloraType("Grass", new Rectangle(96, 0, 24, 24), null, 10, 6);
            Types[6] = new FloraType("Grass", new Rectangle(120, 0, 24, 24), null, 10, 0);

            Types[10] = new FloraType("Cyan Flower", new Rectangle(0, 24, 24, 24));
            Types[11] = new FloraType("Pink Flower", new Rectangle(24, 24, 24, 24));
            Types[12] = new FloraType("Blue Flower", new Rectangle(24, 24, 24, 24));
            Types[13] = new FloraType("Orange Flower", new Rectangle(24, 24, 24, 24));
            Types[14] = new FloraType("Purple Flower", new Rectangle(24, 24, 24, 24));

            Types[30] = new FloraType("Vine", new Rectangle(0, 48, 24,24));
            Types[31] = new FloraType("Vine", new Rectangle(24, 48, 24, 24));
            Types[32] = new FloraType("Vine", new Rectangle(48, 48, 24, 24));
            Types[33] = new FloraType("Vine", new Rectangle(72, 48, 24, 24));

        }

        public void LoadContent()
        {

        }

        public void Update(Map map)
        {
            Random random = new Random();

            for (int x = 1; x < map.SizeX; x++)
            {
                for (int y = 1; y < map.SizeY; y++)
                {

                    // If square empty, attempt to place flowers and grass
                    if (map.Blocks[x,y] != null && map.Blocks[x,y - 1] == null && Flora[x, y - 1] == null)
                    {
                        int rand = random.Next(0, 600);
                        if (rand > 0 && rand < 20)
                        {
                            Flora[x, y - 1] = new Flora(Types[1]);
                        }
                        else if (rand == 21)
                        {
                            Flora[x, y - 1] = new Flora(Types[10]);
                        }
                        else if (rand == 22)
                        {
                            Flora[x, y - 1] = new Flora(Types[11]);
                        }
                        else if (rand == 23)
                        {
                            Flora[x, y - 1] = new Flora(Types[12]);
                        }
                    }

                    // If square is empty, attempt to place vines
                    if (map.Blocks[x,y] != null && map.getBlock(x, y + 1) == null)
                    {
                        int rand = random.Next(0, 30);
                        if (rand == 1)
                        {
                            Flora[x, y + 1] = new Flora(Types[30]);
                        }
                    }

                    //if(Flora[x,y]

                    if (Flora[x, y] != null && Flora[x,y].Type.GrowInto != 0)
                    {
                        Flora[x, y].GrowUpto++;
                        if (Flora[x, y].GrowUpto >= Flora[x, y].Type.GrowAt)
                            { Flora[x, y] = new Flora( Types[Flora[x,y].Type.GrowInto] ); }
                    }
                }
            }

        }

        public void Draw(SpriteBatch spriteBatch)
        {



        }

    }
}
