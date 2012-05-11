using System;
using System.Collections.Generic;
using Blueprint.Blocks.Generation;

namespace Blueprint
{

    /// <summary>
    /// Defines an area to be generated with the map generator
    /// </summary>
    class BlockGenerationArea : BlockArea
    {

        public BlockGenerationArea(int x, int y, int width, int height, BlockGenerator generator)
            :base(x,y,width,height)
        {
            Generator = generator;
        }

        public BlockGenerator Generator;

        public void Generate(Map map)
        {
            map = Generator.Generate(map, this);
        }


    }
}
