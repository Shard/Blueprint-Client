using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Blueprint.Blocks.Generation;

namespace Blueprint
{


    /// <summary>
    /// Main class used when generating maps
    /// </summary>
    class MapGenerator
    {

        int Width;
        int Height;
        BlockGenerationArea[] Areas;

        public void Setup(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public Map Generate(Map map)
        {


            // Define Areas
            Areas = new BlockGenerationArea[10];
            Areas[0] = new BlockGenerationArea(0, 10, 200, 70, new FlatBlockGenerator());
            
            // Do Generation
            foreach (var area in Areas)
            {
                if (area == null) { continue; }
                area.Generate(map);
            }

            return map;
        }

    }
}
