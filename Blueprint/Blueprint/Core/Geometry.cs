using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Blueprint
{
    static class Geometry
    {

        #region Angle Calculation

        static public double Angle(Rectangle From, Vector2 To)
        {
            return Angle(new Vector2(From.Center.X, From.Center.Y), To);
        }

        static public double Angle(Rectangle From, Rectangle To)
        {
            return Angle(new Vector2(From.Center.X, From.Center.Y), new Vector2(To.Center.X, To.Center.Y));
        }

        static public double Angle(Vector2 From, Rectangle To)
        {
            return Angle(From, new Vector2(To.Center.X, To.Center.Y));
        }

        /// <summary>
        /// Calculates the angle between two points
        /// </summary>
        /// <param name="From"></param>
        /// <param name="To"></param>
        /// <returns></returns>
        static public double Angle(Vector2 From, Vector2 To)
        {

            if (From.X > To.X)
            {
                if (From.Y > To.Y)
                {
                    return Math.Atan2(From.Y - To.Y, From.X - To.X) * (180 / Math.PI) + 90; // Bottom right
                }
                else
                {
                    return (90 - Math.Atan2(To.Y - From.Y, From.X - To.X) * (180 / Math.PI)); // Top right
                }
            }
            else
            {
                if (From.Y > To.Y)
                {
                    return (90 - Math.Atan2(From.Y - To.Y, To.X - From.X) * (180 / Math.PI)) + 180; // Bottom left
                }
                else
                {
                    return Math.Atan2(To.Y - From.Y, To.X - From.X) * (180 / Math.PI) + 270; // Top Left
                }
            }
        }

        #endregion

        #region Range

        public static int Range(Vector2 from, Vector2 to)
        {
            return Range(new Point((int)from.X, (int)from.Y), new Point( (int)to.X, (int)to.Y));
        }

        public static int Range(Rectangle from, Rectangle to)
        {
            return Range(new Point(from.Center.X / 24, from.Center.Y / 24), new Point(to.Center.X / 24, to.Center.Y / 24));
        }

        public static int Range(Point from, Point to)
        {
            int x = from.X - to.X;
            int y = from.Y - to.Y;
            if (x < 0) { x *= -1; }
            if (y < 0) { y *= -1; }
            if (x > y) { return x; } else { return y; }
        }

        #endregion

    }
}
