using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blueprint.Blocks.Generation
{


    /// <summary>
    /// Acts as a base class that is used in map generation
    /// </summary>
    abstract class BlockGenerator
    {

        /// <summary>
        /// Overideable function for map generation
        /// </summary>
        /// <returns></returns>
        abstract public Map Generate(Map map, BlockArea area);

    }
}
