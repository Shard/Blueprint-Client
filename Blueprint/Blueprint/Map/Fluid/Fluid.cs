using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Blueprint.Fluid
{
    class Fluid
    {

        public byte[,] Blocks;
        public int MaxSkip;
        public int CurrentSkip;
        public int MapWidth;
        public int MapHeight;
        /// <summary>
        /// The maximun amount a block fluid can shift at one time
        /// </summary>
        public byte MaxMove;
        public byte MaxDownMove;

        private List<Vector2> Shifted;

        public void Initialize(int width, int height)
        {
            Blocks = new byte[width, height];
            MapWidth = width;
            MapHeight = height;
        }

        public void Update ( ref Map map)
        {

            CurrentSkip++;
            if (CurrentSkip < MaxSkip)
            {
                return;
            }

            Shifted = new List<Vector2>();

            for (int x = 0; x < map.SizeX; x++)
            {
                for (int y = 0; y < map.SizeY; y++)
                {
                    if (Blocks[x, y] == 0) { continue; }
                    if (Shifted.Contains(new Vector2(x, y))) { continue; }

                    bool MoveDown = false;
                    bool MoveLeft = false;
                    bool MoveRight = false;

                    // Find out where the liquid can move
                    if (map.getBlock(x,y+1) == null) {
                        MoveDown = true;
                        if (Blocks[x, y+1] != 0 && Blocks[x, y+1] == 9) { MoveDown = false; }
                    }
                    if (map.getBlock(x - 1, y) == null)
                    {
                        MoveLeft = true;
                        if (Blocks[x - 1, y] != 0 && Blocks[x - 1, y] == 9) { MoveLeft = false; }
                    }
                    if (map.getBlock(x + 1, y) == null)
                    {
                        MoveRight = true;
                        if (Blocks[x + 1, y] != 0 && Blocks[x + 1, y] == 9) { MoveRight = false; }
                    }

                    if (MoveDown)
                    {
                        Shift(x, y + 1, x, y, MaxDownMove, true);
                    }
                    else {

                        if (MoveRight)
                        {
                            Shift(x + 1, y, x, y, MaxMove);
                        }

                        if (MoveLeft)
                        {
                            Shift(x - 1, y, x, y, MaxMove);
                        }
                    }
                }
            }

            CurrentSkip = 0;

        }

        public void Shift(int destx, int desty, int sourcex, int sourcey, byte max = 0, bool forced = false)
        {
            
            if (Blocks[sourcex, sourcey] == 0) { return; }
            if (!forced && Blocks[sourcex, sourcey] <= Blocks[destx, desty]) { return; }

            byte amount_moving = Blocks[sourcex, sourcey];
            if (amount_moving > max) { amount_moving = max; }
            if (amount_moving + Blocks[destx, desty] > 9) { amount_moving -= (byte)(Blocks[destx, desty] + amount_moving - 9); }

            Blocks[sourcex, sourcey] -= amount_moving;
            Blocks[destx, desty] += amount_moving;

            Shifted.Add(new Vector2(destx, desty));
        }

        public void Draw(SpriteBatch spriteBatch, Texture2D texture, Camera camera)
        {

            for (int x = 0; x < MapWidth; x++)
            {
                for (int y = 0; y < MapHeight; y++)
                {
                    if (Blocks[x, y] != 0)
                    {
                        spriteBatch.Draw(texture, camera.FromRectangle(new Rectangle(x * 24, y * 24, 24, 24)),new Rectangle( (Blocks[x,y] - 1) * 24 ,48,24,24), Color.White);
                    }
                }
            }

        }

    }
}
