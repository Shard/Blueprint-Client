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
        public float Acceleration; // How quickly the object through Running/Flying can speed up towards max speed.
        public float MaxSpeed; // The max speed achievable through movement (Running/Flying).
        public float TerminalSpeed; // Basicially the absolute max speed the object can travel.
        public float Gravity; // The amount of downwards force that is being applied to the object.
        public float Drag; // The amount of force that is applied against the object when it is not trying to move
        public float JumpSpeed; // The speed at which the object moves up when jumping
        public float JumpPower; // The amount of jump power an object has left, is depleted while jumping and restored on grounding
        public float MaxJumpPower; // The amount an object can jump before it looses power
        public Vector2 Velocity; // The final value that determines where a user moves
        public Rectangle Area; // The area that the object occupies;
        public Vector2 Moved;

        // Intention
        public bool MovingLeft;
        public bool MovingRight;
        public bool Jumping;
        public bool Sprinting;

        // States
        public bool Falling; // When true, gravity is acting
        public bool Solid; // If the object has solid footing
        public string Direction; // The direction the object is facing

        public Movement( Vector2 position)
        {
            Acceleration = 0.3f;
            MaxSpeed = 4f;
            TerminalSpeed = 8f;
            Gravity = 0.3f;
            Drag = 0.3f;
            JumpPower = 8f;
            MaxJumpPower = 8f;
            JumpSpeed = 4f;
            Velocity = new Vector2();
            Area = new Rectangle((int)position.X, (int)position.Y, 32, 44);
            Moved = new Vector2();

            Falling = true;
            MovingLeft = false;
            MovingRight = false;
            Jumping = false;
            Sprinting = false;
            Solid = false;
            Direction = "right";
        }

        
        // Converts user controls into movement intentions
        public void HandleControls( Control control )
        {

            if (control.currentKeyboard.IsKeyDown(Keys.A)) { MovingLeft = true; } else { MovingLeft = false; }
            if (control.currentKeyboard.IsKeyDown(Keys.D)) { MovingRight = true; } else {MovingRight = false; }
            if (control.currentKeyboard.IsKeyDown(Keys.D) && control.currentKeyboard.IsKeyDown(Keys.A)) { MovingRight = false; MovingLeft = false; }
            if (control.currentKeyboard.IsKeyDown(Keys.Space)) { Jumping = true; } else { Jumping = false; }
            if (control.currentKeyboard.IsKeyUp(Keys.Space) && control.previousKeyboard.IsKeyDown(Keys.Space)) { Falling = true; }
        }

        public void Update( Map map )
        {

            Vector2 originalPosition = new Vector2(Area.X, Area.Y);

            // Turn intentions into changes in velocity
            if (MovingLeft && Velocity.X > MaxSpeed * -1) // If moving left and not at the current max speed
            {
                Velocity.X -= Acceleration;
                if (Velocity.X < MaxSpeed * -1) { Velocity.X = MaxSpeed * -1; } // If accelatred too fast, level it out to the max speed
            }
            if (MovingRight && Velocity.X < MaxSpeed)  // If moving right and not at the current max speed
            { 
                Velocity.X += Acceleration;
                if (Velocity.X > MaxSpeed) { Velocity.X = MaxSpeed; } // If accelatred too fast, level it out to the max speed
            }
            if (Jumping && JumpPower >= 0) // If trying to jump and still has jump power left
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

            if (Falling) { Velocity.Y += Gravity; }

            if (!MovingLeft && !MovingRight) // If not moving left or right, apply drag
            {
                if(Velocity.X < 0)
                {
                    Velocity.X += Drag;
                    if (Velocity.X > 0) { Velocity.X = 0; }
                } else {
                    Velocity.X -= Drag;
                    if (Velocity.X < 0) { Velocity.X = 0; }
                }
            }

            // Terminal speed
            if (Velocity.X > TerminalSpeed) { Velocity.X = TerminalSpeed; }
            if (Velocity.X < TerminalSpeed * -1) { Velocity.X = TerminalSpeed * -1; }
            if (Velocity.Y > TerminalSpeed) { Velocity.Y = TerminalSpeed; }
            if (Velocity.Y < TerminalSpeed * -1) { Velocity.Y = TerminalSpeed * -1; }

            // Direct
            if (Velocity.X > 0) { Direction = "right"; }
            if (Velocity.X < 0) { Direction = "left"; }

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
                if (collisionRuns > 20) { break; }
            }

            Moved = new Vector2(Area.X, Area.Y) - originalPosition;
            //Log();
        }

        public void Log()
        {
            Console.WriteLine("Falling: " + Falling.ToString() + ", Jumping: " + Jumping.ToString() + ", X: " + Area.X.ToString() + ", Y: " + Area.Y.ToString() + ", Footing: " + Solid.ToString());
        }

        public bool SolidFooting(Rectangle area, Map map)
        {
            int BlockAtX = Area.Center.X / 32;
            int BlockAtY = Area.Center.Y / 32;
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
                    Rectangle blockArea = new Rectangle(x * 32, y * 32, 32, 32);
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

        public bool SolveCollision(Map map)
        {

            int BlockAtX = Area.Center.X / 32;
            int BlockAtY = Area.Center.Y / 32;
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
                    Rectangle blockArea = new Rectangle(x * 32, y * 32, 32, 32);
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
