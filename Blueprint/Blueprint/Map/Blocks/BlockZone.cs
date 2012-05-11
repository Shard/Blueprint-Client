using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Blueprint
{
    class BlockZone
    {

        public List<Vector2> Blocks;

        public BlockZone()
        {
            Blocks = new List<Vector2>();
        }

        public void Apply(Rectangle rect, bool add = true)
        {
            for (int x = rect.X; x < rect.X + rect.Width; x++)
            {
                for (int y = rect.Y; y < rect.Y + rect.Height; y++)
                {
                    if(Blocks.IndexOf(new Vector2(x,y)) != null)
                    {
                        if (!add) { Blocks.Remove(new Vector2(x, y)); }
                    } else {
                        if (add) { Blocks.Add(new Vector2(x, y)); }
                    }
                }
            }
        }

    }
}
