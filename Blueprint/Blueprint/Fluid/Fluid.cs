using System;
using System.Collections.Generic;

namespace Blueprint.Fluid
{
    class Fluid
    {

        public FluidBlock[,] Blocks;
        public int MaxSkip;
        public int CurrentSkip;


        public void Initialize(int width, int height)
        {
            Blocks = new FluidBlock[width, height];
        }

        public void Update(Map map)
        {
            for (int x = 0; x < map.SizeX; x++)
            {
                for (int y = 0; y < map.SizeY; y++)
                {
                    if (Blocks[x, y] == null) { continue; }

                    bool MoveDown = false;
                    bool MoveLeft = false;
                    bool MoveRight = false;

                    // Find out where the liquid can move
                    if (map.getBlock(x,y+1) == null) {
                        MoveDown = true;
                        if (Blocks[x, y+1] != null && Blocks[x, y+1].Height == 7) { MoveDown = false; }
                    }
                    if (map.getBlock(x - 1, y) == null)
                    {
                        MoveLeft = true;
                        if (Blocks[x - 1, y] != null && Blocks[x - 1, y].Height == 7) { MoveLeft = false; }
                    }
                    if (map.getBlock(x + 1, y) == null)
                    {
                        MoveRight = true;
                        if (Blocks[x + 1, y] != null && Blocks[x + 1, y].Height == 7) { MoveRight = false; }
                    }

                    

                    

                }
            }
        }

        /// <summary>
        /// Attempts to move liquid to another block, returns the amount of liquid that was left after the move
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public byte MoveTo(int x, int y)
        {

            if (Blocks[x, y] == null)
            {
                Blocks[x, y] = new FluidBlock(7);
                return 0;
            }
            else
            {
                byte moved = (byte)(7 - Blocks[x, y].Height);
                Blocks[x, y].Height += moved;
                return moved;
            }

        }

        public void Draw()
        {

        }

    }
}
