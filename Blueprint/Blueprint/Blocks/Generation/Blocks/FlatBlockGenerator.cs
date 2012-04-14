using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blueprint.Blocks.Generation
{

    /// <summary>
    /// Fills the area with blocks
    /// </summary>
    class FlatBlockGenerator : BlockGenerator
    {

        public override Map Generate(Map map, BlockArea area)
        {

            for (int x = area.X; x < area.Width; x++)
            {
                for (int y = area.Y; y < area.Height; y++)
                {
                    map.Blocks[x, y] = new Block(map.Types[0]);
                }
            }


            return map;

        }

    }
}
