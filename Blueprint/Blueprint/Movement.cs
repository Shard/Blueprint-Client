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

        /// <summary>How quickly the object through Running/Flying can speed up towards max speed.</summary>
        public float Acceleration;

        /// <summary>The max speed achievable through movement (Running/Flying).</summary>
        public float MaxSpeed;

        /// <summary>Basicially the absolute max speed the object can travel.</summary>
        public float TerminalSpeed;

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

        /// <summary>The final value that determines where a user moves</summary>
        public Vector2 Velocity;

        /// <summary>The area that the object occupies;</summary>
        public Rectangle Area;

        /// <summary>Not sure</summary>
        public Vector2 Moved;

        /// <summary>Defines how much bounceback the mover will recieve on collision</summary>
        public float Bouncy;

        /// <summary>The intention that is linked to the movement object</summary>
        public Intention Intention;

        /// <summary>True is the mover is actually jumping upwards</summary>
        public bool IsJumping;

        /// <summary> When true, gravity is acting</summary>
        public bool Falling;

        /// <summary>If the object has solid footing</summary>
        public bool Solid;

        /// <summary>The direction the object is facing</summary>
        public string Direction;

        public Movement( Vector2 position, int width, int height)
        {
            Acceleration = 0.25f;
            MaxSpeed = 4f;
            TerminalSpeed = 12f;
            Gravity = .35f;
            Drag = 0.2f; 
            JumpPower = 7f;
            MaxJumpPower = 7f;
            JumpSpeed = 3f;
            Velocity = new Vector2();
            Area = new Rectangle((int)position.X, (int)position.Y, width, height);
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

            double angle = 0.0;
            Vector2 apply = Vector2.Zero;

            if (location.X > Area.Center.X)
            {
                if (location.Y > Area.Center.Y)
                {
                    angle = Math.Atan2(location.Y - Area.Center.Y, location.X - Area.Center.X) * (180 / Math.PI) + 90; // Bottom right
                }
                else
                {
                    angle = (90 - Math.Atan2(Area.Center.Y - location.Y, location.X - Area.Center.X) * (180 / Math.PI)); // Top right
                }
            }
            else
            {
                if (location.Y > Area.Center.Y)
                {
                    angle = (90 - Math.Atan2(location.Y - Area.Center.Y, Area.Center.X - location.X) * (180 / Math.PI)) + 180; // Bottom left
                }
                else
                {
                    angle = Math.Atan2(Area.Center.Y - location.Y, Area.Center.X - location.X) * (180 / Math.PI) + 270; // Top Left
                }
            }

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

            if(totalWidth > 0 || totalHeight > 0){
                if (totalWidth > totalHeight) // Horizontal
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

    }
}
