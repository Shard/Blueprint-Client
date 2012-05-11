using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Blueprint.Fluid
{
    class Fluid
    {
        /// <summary>
        /// Contains all instanced liquids
        /// </summary>
        public byte[,] Blocks;

        /// <summary>
        /// Tempororay blocks array
        /// </summary>
        private byte[,] TempBlocks;
        public int MaxSkip;
        public int CurrentSkip;
        public int MapWidth;
        public int MapHeight;

        /// <summary>
        /// The maximun amount a block fluid can shift into an empty tile
        /// </summary>
        public byte MaxEmptyMove;

        /// <summary>
        /// The maximun amount a block fluid can shift at one time
        /// </summary>
        public byte MaxMove;

        /// <summary>
        /// The maximun amount a block fluid can shift downwards per frame
        /// </summary>
        public byte MaxDownMove;


        public void Initialize(int width, int height)
        {
            Blocks = new byte[width, height];
            TempBlocks = new byte[width, height];
            MapWidth = width;
            MapHeight = height;
        }

        public void Update ( Map map)
        {

            CurrentSkip++;
            if (CurrentSkip < MaxSkip){ return; }

            TempBlocks = new byte[MapWidth, MapHeight];

            for (int x = 0; x < map.SizeX; x++)
            {
                for (int y = map.SizeY - 1; y > 0; y--)
                {
                    if (Blocks[x, y] == 0) { continue; }

                    bool MoveDown = false;
                    bool MoveLeft = false;
                    bool MoveRight = false;

                    // Find out where the liquid can move
                    if (map.getBlock(x,y+1) == null) {
                        MoveDown = true;
                        if (Blocks[x, y+1] != 0 && Blocks[x, y+1] == 24) { MoveDown = false; }
                    }
                    if (map.getBlock(x - 1, y) == null)
                    {
                        MoveLeft = true;
                        if (Blocks[x - 1, y] != 0 && Blocks[x - 1, y] == 24) { MoveLeft = false; }
                    }
                    if (map.getBlock(x + 1, y) == null)
                    {
                        MoveRight = true;
                        if (Blocks[x + 1, y] != 0 && Blocks[x + 1, y] == 24) { MoveRight = false; }
                    }

                    if (MoveDown)
                    {
                        Shift(x, y + 1, x, y, MaxDownMove, true);
                    }
                    else if(MoveLeft && MoveRight){
                        Shift(x + 1, y, x, y, (byte)(Blocks[x, y] / 4) );
                        Shift(x - 1, y, x, y, (byte)(Blocks[x, y] / 4));
                    } else {

                        if (MoveRight)
                        {
                            Shift(x + 1, y, x, y);
                        }

                        if (MoveLeft)
                        {
                            Shift(x - 1, y, x, y);
                        }
                    }
                }
            }

            // Apply Temp blocks
            for (int x = 0; x < map.SizeX; x++)
            {
                for (int y = map.SizeY - 1; y > 0; y--)
                {
                    if (TempBlocks[x, y] > 0) {
                        Blocks[x, y] += TempBlocks[x, y];
                    }
                }
            }

            CurrentSkip = 0;

        }

        /// <summary>
        /// Attemps to shift a liquid
        /// </summary>
        /// <param name="destx"></param>
        /// <param name="desty"></param>
        /// <param name="sourcex"></param>
        /// <param name="sourcey"></param>
        /// <param name="max"></param>
        /// <param name="forced">If true, will move the water even if the new tile has more water</param>
        public void Shift(int destx, int desty, int sourcex, int sourcey, byte max = 0, bool forced = false)
        {

            //if ( Blocks[destx, desty] != 0 && Shifted.Contains(new Vector2(sourcex, sourcey))) { return; }

            if(max == 0){
                max = MaxMove;
                // Moving to an empty block
                if (Blocks[destx, desty] == 0) { 
                    max = MaxEmptyMove;
                }
            }

            if (Blocks[sourcex, sourcey] == 0) { return; }
            if (!forced && Blocks[sourcex, sourcey] <= Blocks[destx, desty]) { return; }

            byte amount_moving = Blocks[sourcex, sourcey];
            if (amount_moving > max) { amount_moving = max; }
            if (amount_moving + Blocks[destx, desty] > 24) { amount_moving -= (byte)(Blocks[destx, desty] + amount_moving - 24); }

            if (amount_moving == 1 && Blocks[destx, desty] == 0) { return; }
            if (amount_moving < 4) { amount_moving = 1; }

            Blocks[sourcex, sourcey] -= amount_moving;
            TempBlocks[destx, desty] += amount_moving;

        }

        public void Draw(SpriteBatch spriteBatch, Texture2D texture, Camera camera)
        {

            int startx = (int)MathHelper.Clamp(camera.X * -1 / 24f, 0, MapWidth);
            int endx = 2 + startx + camera.Width / 24;
            int starty = (int)MathHelper.Clamp(camera.Y * -1 / 24f, 0, MapHeight);
            int endy = 2 + starty + camera.Height / 24;

            for (int x = startx; x <= endx; x++)
            {
                for (int y = starty; y <= endy; y++)
                {
                    if (Blocks[x, y] != 0)
                    {
                        byte h = Blocks[x, y];
                        if(Blocks[x,y-1] != 0){ h = 24; }
                        if (Blocks[x, y + 1] == 24 && Blocks[x, y] < 4) { continue; } // little hack
                        
                        spriteBatch.Draw(texture, camera.FromRectangle(new Rectangle(x * 24, y * 24 + 24 - h, 24, h)), new Rectangle(8 * 24, 48, 24, h), Color.White);
                    }
                }
            }

        }

    }
}
