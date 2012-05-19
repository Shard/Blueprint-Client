using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Blueprint
{
    class Movement
    {

        // Metrics

        #region Speed

        /// <summary>How quickly the object through Running/Flying can speed up towards max speed.</summary>
        public float Acceleration;

        /// <summary>The max speed achievable through movement (Running/Flying).</summary>
        public float MaxSpeed;

        /// <summary>Basicially the absolute max speed the object can travel.</summary>
        public float TerminalSpeed;

        /// <summary>The final value that determines where a user moves</summary>
        public Vector2 Velocity;

        #endregion

        #region Jumping and Gravity

        /// <summary>The amount of downwards force that is being applied to the object.</summary>
        public float Gravity;

        /// <summary> The amount of force that is applied against the object when it is not trying to move</summary>
        public float Drag;

        /// <summary>The speed at which the object moves up when Intention.Jumping</summary>
        public float JumpSpeed;

        /// <summary>The amount of jump power an object has left, is depleted while Intention.Jumping and restored on grounding</summary>
        public float JumpPower;

        /// <summary>The amount an object can jump before it looses power</summary>
        public float MaxJumpPower;

        /// <summary>True is the mover is actually jumping upwards</summary>
        public bool IsJumping;

        /// <summary> When true, gravity is acting</summary>
        public bool Falling;

        #endregion

        /// <summary>The area that the object occupies;</summary>
        public Rectangle Area;

        /// <summary>Not sure</summary>
        public Vector2 Moved;

        /// <summary>Defines how much bounceback the mover will recieve on collision</summary>
        public float Bouncy;

        /// <summary>The intention that is linked to the movement object</summary>
        public Intention Intention;

        /// <summary>If the object has solid footing</summary>
        public bool Solid;

        /// <summary>The direction the object is facing</summary>
        public string Direction;

        public Movement( Vector2 position, int width, int height, float gravity = .35f, float drag = 0.2f)
        {
            Acceleration = 0.25f;
            MaxSpeed = 4f;
            TerminalSpeed = 12f;
            Gravity = gravity;
            Drag = drag; 
            JumpPower = 7f;
            MaxJumpPower = 7f;
            JumpSpeed = 3f;
            Velocity = new Vector2();
            Area = new Rectangle((int)position.X - width / 2, (int)position.Y - height / 2, width, height);
            Moved = new Vector2();
            Bouncy = 1f;

            IsJumping = false;
            Falling = true;
            Intention = new Intention();
            Solid = false;
            Direction = "right";
        }


        /// <summary>Converts user controls into movement intentions</summary>
        public void HandleControls( Control control )
        {

            if (control.currentKeyboard.IsKeyDown(Keys.A)) { Intention.Left = true; } else { Intention.Left = false; }
            if (control.currentKeyboard.IsKeyDown(Keys.D)) { Intention.Right = true; } else {Intention.Right = false; }
            if (control.currentKeyboard.IsKeyDown(Keys.D) && control.currentKeyboard.IsKeyDown(Keys.A)) { Intention.Right = false; Intention.Left = false; }
            if (control.previousKeyboard.IsKeyUp(Keys.Space) && control.currentKeyboard.IsKeyDown(Keys.Space)) { Intention.Jumping = true; }
            if (control.currentKeyboard.IsKeyUp(Keys.Space)) { Intention.Jumping = false; }
            if (control.currentKeyboard.IsKeyUp(Keys.Space) && control.previousKeyboard.IsKeyDown(Keys.Space) && !Intention.Jumping) { Falling = true; }
            if (Moved.Y > 0 && Intention.Jumping) { Intention.Jumping = false; }
        }

        /// <summary>
        /// Standard Update Function
        /// </summary>
        public void Update( Map map )
        {

            Vector2 originalPosition = new Vector2(Area.X, Area.Y);

            // Turn intentions into changes in velocity
            if (Intention.Left && Velocity.X > MaxSpeed * -1) // If moving left and not at the current max speed
            {
                Velocity.X -= Acceleration;
                if (Velocity.X < MaxSpeed * -1) { Velocity.X = MaxSpeed * -1; } // If accelatred too fast, level it out to the max speed
            }
            if (Intention.Right && Velocity.X < MaxSpeed)  // If moving right and not at the current max speed
            { 
                Velocity.X += Acceleration;
                if (Velocity.X > MaxSpeed) { Velocity.X = MaxSpeed; } // If accelatred too fast, level it out to the max speed
            }
            if (Intention.Jumping && JumpPower >= 0) // If trying to jump and still has jump power left
            {
                Velocity.Y -= JumpSpeed;
                JumpPower -= JumpSpeed;
                if (JumpPower < 0) { Falling = true; } else { Falling = false; } // Once jump runs out then active gravity
            }
            else
            {
                // Check for solid footing
                Solid = SolidFooting(Area,map);
                if (!Solid)
                {
                    Falling = true;
                }
                else
                {
                    Falling = false;
                }
            }

            if (Falling) {
                if (Velocity.Y < 1)
                { // Less gravity
                    Velocity.Y += Gravity / 1.5f;
                }
                else
                {
                    Velocity.Y += Gravity;
                }
            }

            if (!Intention.Left && !Intention.Right) // If not moving left or right, apply drag
            {
                if(Velocity.X < 0)
                {
                    if (Falling) { Velocity.X += Drag * 0.3f; } else { Velocity.X += Drag; }
                    if (Velocity.X > 0) { Velocity.X = 0; }
                } else {
                    if (Falling) { Velocity.X -= Drag * 0.3f; } else { Velocity.X -= Drag; }
                    if (Velocity.X < 0) { Velocity.X = 0; }
                }
            }

            // Terminal speed
            if (Velocity.X > TerminalSpeed) { Velocity.X = TerminalSpeed; }
            if (Velocity.X < TerminalSpeed * -1) { Velocity.X = TerminalSpeed * -1; }
            if (Velocity.Y > TerminalSpeed) { Velocity.Y = TerminalSpeed; }
            if (Velocity.Y < TerminalSpeed * -1) { Velocity.Y = TerminalSpeed * -1; }

            // Direct
            if (Intention.Left) { Direction = "left"; }
            if (Intention.Right) { Direction = "right"; }

            // Apply Movement
            Area.X += (int)Velocity.X;
            Area.Y += (int)Velocity.Y;

            // Collisions
            bool collisionSolved = false;
            int collisionRuns = 0;
            while (!collisionSolved)
            {
                collisionRuns++;
                collisionSolved = SolveCollision(map);
                if (collisionRuns > 20) { Falling = false; break; }
            }

            // Finishing Changes
            Moved = new Vector2(Area.X, Area.Y) - originalPosition;
            

        }

        /// <summary>
        /// Checks to see if a rectangle has solid footing mover has solid footing
        /// </summary>
        /// <param name="area">The area which is being checked</param>
        /// <param name="map">Checks against all collidables</param>
        /// <returns>Returns whether the area has solid footing or not</returns>
        public bool SolidFooting(Rectangle area, Map map)
        {
            int BlockAtX = Area.Center.X / 24;
            int BlockAtY = Area.Center.Y / 24;
            int totalWidth = 0;
            int totalHeight = 0;
            Vector2 collisionCenter = Vector2.Zero;
            area.Y++;

            for (int x = BlockAtX - 5; x < BlockAtX + 5; x++)
            {
                for (int y = BlockAtY - 5; y < BlockAtY + 5; y++)
                {
                    Block block = map.getBlock(x, y);
                    if (block == null) { continue; }
                    Rectangle blockArea = new Rectangle(x * 24, y * 24, 24, 24);
                    Rectangle intersection = Rectangle.Intersect(area, blockArea);

                    if (intersection != Rectangle.Empty)
                    {
                        totalHeight += intersection.Height;
                        totalWidth += intersection.Width;
                        if (collisionCenter == Vector2.Zero)
                        {
                            collisionCenter = new Vector2(blockArea.Center.X, blockArea.Center.Y);
                        }
                        else
                        {
                            collisionCenter = new Vector2((collisionCenter.X + blockArea.Center.X) / 2, (collisionCenter.Y + blockArea.Center.Y) / 2);
                        }
                    }
                }
            }

            if (totalWidth > 0 || totalHeight > 0)
            {
                if (totalWidth > totalHeight) // Horizontal
                {
                    if (collisionCenter.Y > area.Center.Y) // Colliding from the bottom
                    {
                        return true;
                    }
                }
            }

            return false;

        }

        /// <summary>
        /// Applies force on the mover from a location
        /// </summary>
        /// <param name="location">The location which the force is comming from</param>
        /// <param name="force">How much force is being used</param>
        public void PushbackFrom( Vector2 location, float force )
        {

            double angle = Geometry.Angle(location, Area);
            Vector2 apply = Vector2.Zero;

            // Option to reverse angle?

            // Apply angle and force
            if (angle <= 90)
            {
                apply.Y = (float)angle / 90 -1;
                apply.X = (float)angle / 90;
            } else if (angle <= 180)
            {
                apply.X = 1 - (float)angle / 90 + 1;
                apply.Y = (float)angle / 90 - 1;
            }
            else if (angle <= 270)
            {
                apply.X = (1 - (float)angle / 90 + 1);
                apply.Y = (((float)angle / 90 - 2) * -1) + 1;
            }
            else
            {
                apply.X = (((float)angle / 90) - 4);
                apply.Y = (((float)angle / 90) - 3) * -1;
            }

            Velocity -= apply * force;

        }

        #region Pathfinding

        private int PathfindEstimate(Point from, Point to)
        {
            int x = from.X + to.X;
            int y = from.Y + to.Y;
            if (x < 0) { x *= -1; }
            if (y < 0) { y *= -1; }
            return x + y ;
        }

        private Point[] PathfindNeighbours(Point loc, Map map)
        {

            Point[] points = new Point[4];
            if (map.getBlock(loc.X, loc.Y + 1) == null) { points[1] = new Point(loc.X, loc.Y + 1); } else { points[0] = Point.Zero; }
            if (map.getBlock(loc.X, loc.Y - 1) == null) { points[0] = new Point(loc.X, loc.Y - 1); } else { points[1] = Point.Zero; }
            if (map.getBlock(loc.X - 1, loc.Y) == null) { points[2] = new Point(loc.X - 1, loc.Y); } else { points[2] = Point.Zero; }
            if (map.getBlock(loc.X + 1, loc.Y) == null) { points[3] = new Point(loc.X + 1, loc.Y); } else { points[3] = Point.Zero; }
            return points;
        }

        public void Pathfind(Vector2 from, Vector2 to, Map map)
        {
            Pathfind(new Point((int)from.X, (int)from.Y), new Point((int)to.X, (int)to.Y), map);
        }

        public Stack<Point> Pathfind(Point from, Point goal, Map map)
        {

            int search_limit = 2000;
            List<Point> closed = new List<Point>();
            List<Point> open = new List<Point>();
            Point[,] movements = new Point[map.SizeX, map.SizeY];
            open.Add( from );

            int[,] g_score = new int[map.SizeX, map.SizeY];
            int[,] h_score = new int[map.SizeX, map.SizeY];
            int[,] f_score = new int[map.SizeX, map.SizeY];
            g_score[from.X, from.Y] = 0;
            h_score[from.X, from.Y] = PathfindEstimate(from, goal);
            f_score[from.X, from.Y] = h_score[from.X, from.Y] + g_score[from.X, from.Y];


            while (open.Count > 0 && search_limit > 0)
            {
                search_limit--;
                // Get node with lowest f score
                int lowest_score = 0;
                int lowest_score_index = 0;
                int lowest_y = 0;
                for (int i = 0; i < open.Count; i++)
                {
                    if (lowest_score == 0 || f_score[ open[i].X, open[i].Y ] < lowest_score || (open[i].Y > lowest_y && f_score[ open[i].X, open[i].Y ] <= lowest_score)) {
                        lowest_score = f_score[open[i].X, open[i].Y];
                        lowest_score_index = i;
                        lowest_y = open[i].Y;
                    }
                }
                Point current = open[lowest_score_index];

                if (current == goal)
                    return PathfindConstruct(movements, goal);

                open.Remove(current);
                closed.Add(current);
                Point[] neighbours = PathfindNeighbours(current, map);
                foreach (Point neighbour in neighbours)
                {
                    if (closed.Contains(neighbour) || neighbour.X < 0 || neighbour.Y < 0 || neighbour == Point.Zero) { continue; }
                    int tmp_g_score = g_score[current.X, current.Y] + PathfindEstimate(neighbour, goal);
                    bool tmp_is_better = false;

                    if (!open.Contains(neighbour))
                    {
                        open.Add(neighbour);
                        h_score[neighbour.X, neighbour.Y] = PathfindEstimate(neighbour, goal);
                        tmp_is_better = true;
                    }
                    else if (tmp_g_score < g_score[neighbour.X, neighbour.Y])
                    {
                        tmp_is_better = true;
                    }

                    if (tmp_is_better)
                    {
                        movements[neighbour.X, neighbour.Y] = current;
                        g_score[neighbour.X, neighbour.Y] = tmp_g_score;
                        f_score[neighbour.X, neighbour.Y] = g_score[neighbour.X, neighbour.Y] + h_score[neighbour.X, neighbour.Y];
                    }
                }

            }
            return null;

        }

        private Stack<Point> PathfindConstruct(Point[,] movements, Point node)
        {
            Stack<Point> path = new Stack<Point>();

            Point current_node = node;
            while (true)
            {
                if (movements[current_node.X, current_node.Y] != Point.Zero)
                {
                    path.Push(movements[current_node.X, current_node.Y]);
                    current_node = movements[current_node.X, current_node.Y];
                    continue;
                }
                else
                {
                    return path;
                }
            }
        }

        #endregion

        /// <summary>
        /// Solves a collision for the movement object.
        /// </summary>
        /// <param name="map">The Map</param>
        /// <returns>Returns false if the collision was not fully solved</returns>
        public bool SolveCollision(Map map)
        {

            int BlockAtX = Area.Center.X / 24;
            int BlockAtY = Area.Center.Y / 24;
            int totalWidth = 0;
            int totalHeight = 0;
            int averageWidthW = 0;
            int averageWidthH = 0;
            int averageHeightW = 0;
            int averageHeightH = 0;
            Vector2 collisionCenterH = Vector2.Zero;
            Vector2 collisionCenterW = Vector2.Zero;

            #region Block Collisions

            for (int x = BlockAtX - 5; x < BlockAtX + 5; x++)
            {
                for (int y = BlockAtY - 5; y < BlockAtY + 5; y++)
                {
                    Block block = map.getBlock(x, y);
                    if (block == null) { continue; }
                    Rectangle blockArea = new Rectangle(x * 24, y * 24, 24, 24);
                    Rectangle intersection = Rectangle.Intersect(Area, blockArea);
                    
                    if (intersection != Rectangle.Empty) // Only handle intersection if it exists
                    {

                        totalHeight += intersection.Height;
                        totalWidth += intersection.Width;
                        if (totalWidth > totalHeight)
                        {
                            if (collisionCenterW == Vector2.Zero)
                            {
                                averageWidthW += intersection.Width;
                                averageHeightW += intersection.Height;
                                collisionCenterW = new Vector2(blockArea.Center.X, blockArea.Center.Y);
                            }
                            else
                            {
                                averageWidthW = (averageWidthW + intersection.Width) / 2;
                                averageHeightW = (averageHeightW + intersection.Height) / 2;
                                collisionCenterW = new Vector2((collisionCenterW.X + blockArea.Center.X) / 2, (collisionCenterW.Y + blockArea.Center.Y) / 2);
                            }
                        }
                        else
                        {
                            if (collisionCenterH == Vector2.Zero)
                            {
                                averageWidthH += intersection.Width;
                                averageHeightH += intersection.Height;
                                collisionCenterH = new Vector2(blockArea.Center.X, blockArea.Center.Y);
                            }
                            else
                            {
                                averageWidthH = (averageWidthH + intersection.Width) / 2;
                                averageHeightH = (averageHeightH + intersection.Height) / 2;
                                collisionCenterH = new Vector2((collisionCenterH.X + blockArea.Center.X) / 2, (collisionCenterH.Y + blockArea.Center.Y) / 2);
                            }
                        }
                    }
                }
            }

            #endregion

            #region Entity Collisions

            foreach (Entity entity in map.Entities.Entities)
            {
                if (!entity.Solid) { continue; }
                Rectangle intersection = Rectangle.Intersect(Area, entity.Area);
                if (intersection != new Rectangle())
                {
                    totalHeight += intersection.Height;
                    totalWidth += intersection.Width;
                    if (totalWidth > totalHeight)
                    {
                        if (collisionCenterW == Vector2.Zero)
                        {
                            averageWidthW += intersection.Width;
                            averageHeightW += intersection.Height;
                            collisionCenterW = new Vector2(entity.Area.Center.X, entity.Area.Center.Y);
                        }
                        else
                        {
                            averageWidthW = (averageWidthW + intersection.Width) / 2;
                            averageHeightW = (averageHeightW + intersection.Height) / 2;
                            collisionCenterW = new Vector2((collisionCenterW.X + entity.Area.Center.X) / 2, (collisionCenterW.Y + entity.Area.Center.Y) / 2);
                        }
                    }
                    else
                    {
                        if (collisionCenterH == Vector2.Zero)
                        {
                            averageWidthH += intersection.Width;
                            averageHeightH += intersection.Height;
                            collisionCenterH = new Vector2(entity.Area.Center.X, entity.Area.Center.Y);
                        }
                        else
                        {
                            averageWidthH = (averageWidthH + intersection.Width) / 2;
                            averageHeightH = (averageHeightH + intersection.Height) / 2;
                            collisionCenterH = new Vector2((collisionCenterH.X + entity.Area.Center.X) / 2, (collisionCenterH.Y + entity.Area.Center.Y) / 2);
                        }
                    }
                }
            }

            #endregion

            if (totalWidth > 0 || totalHeight > 0){
                if (totalWidth > totalHeight ) // Horizontal
                {
                    if (collisionCenterW.Y < Area.Center.Y) // Colliding from the top
                    { Area.Y += averageHeightW; Velocity.Y = 0; }
                    else // Colliding from the bottom
                    {
                        Area.Y -= averageHeightW;
                        JumpPower = MaxJumpPower;
                        Falling = false;
                        if (Velocity.Y > 0) { Velocity.Y = 0; } }
                }
                else
                {
                    if (collisionCenterH.X > Area.Center.X) // Colliding from the right
                    { Area.X -= averageWidthH; if (Velocity.X > 0) { Velocity.X = 0; } }
                    else // Colliding from the left
                    { Area.X += averageWidthH; if (Velocity.X < 0) { Velocity.X = 0; } }
                }
                return false;
            }

            return true;

        }

        /// <summary>
        /// Take a part out of the main collision code to allow for smaller collision detection
        /// </summary>
        public void CalculateCollision()
        {

        }

    }
}
