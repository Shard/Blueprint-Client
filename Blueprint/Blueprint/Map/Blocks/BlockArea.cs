using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Blueprint
{
    class BlockArea
    {

        public int X;
        public int Y;
        public int Width;
        public int Height;

        public BlockArea(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public Vector2[] ToArray()
        {

            Vector2[] array = new Vector2[ Width * Height];

            for (int x = X; x < Width; x++)
            {
                for (int y = Y; y < Height; y++)
                {
                    array[array.Length] = new Vector2(x, y);
                }
            }

            return array;
        }

    }
}
