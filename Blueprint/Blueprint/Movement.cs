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

        // Intention
        public bool MovingLeft;
        public bool MovingRight;
        public bool Jumping;
        public bool Sprinting;


        public Movement()
        {
            Acceleration = 0.1f;
            MaxSpeed = 4f;
            TerminalSpeed = 8f;
            Gravity = 0.3f;
            Drag = 0.3f;
            JumpPower = 4f;
            JumpSpeed = 1f;
            Velocity = new Vector2();
            Area = new Rectangle();

            MovingLeft = false;
            MovingRight = false;
            Jumping = false;
            Sprinting = false;
        }

        
        // Converts user controls into movement intentions
        public void HandleControls( Control control )
        {

            if (control.currentKeyboard.IsKeyDown(Keys.A)) { MovingLeft = true; }
            if (control.currentKeyboard.IsKeyDown(Keys.D)) { MovingRight = true; }
            if (control.currentKeyboard.IsKeyDown(Keys.D) && control.currentKeyboard.IsKeyDown(Keys.A)) { MovingRight = false; MovingLeft = false; }
            if (control.currentKeyboard.IsKeyDown(Keys.Space)) { Jumping = true; }

        }

        public void Update( Map map )
        {

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
            if (Jumping && JumpPower > 0) // If trying to jump and still has jump power left
            {
                Velocity.Y += JumpSpeed;
                JumpPower -= JumpSpeed;
            }

            // Terminal speed
            if (Velocity.X > TerminalSpeed) { Velocity.X = TerminalSpeed; }
            if (Velocity.X < TerminalSpeed * -1) { Velocity.X = TerminalSpeed * -1; }
            if (Velocity.Y > TerminalSpeed) { Velocity.Y = TerminalSpeed; }
            if (Velocity.Y < TerminalSpeed * -1) { Velocity.Y = TerminalSpeed * -1; }

            // Apply Movement
            Area.X += (int)Velocity.X;
            Area.Y += (int)Velocity.Y;

            // Collisions
            bool collisionSolved = false;
            while (!collisionSolved)
            {
                collisionSolved = SolveCollision(map);
            }

            


        }

        public bool SolveCollision(Map map)
        {

            int BlockAtX = Area.Center.X / 32;
            int BlockAtY = Area.Center.Y / 32;

            for (int x = BlockAtX - 5; x < BlockAtX + 5; x++)
            {
                for (int y = BlockAtY - 5; y < BlockAtY + 5; y++)
                {
                    Block block = map.getBlock(x, y);
                    if (block == null) { continue; }
                    Rectangle blockArea = new Rectangle(x * 32, y * 32, 32, 32);
                    Rectangle intersection = Rectangle.Intersect(Area, blockArea);
                    if (intersection != Rectangle.Empty)
                    {
                        if (intersection.Width > intersection.Height) // Horizontal
                        {
                            if (blockArea.Center.Y > Area.Center.Y) // Colliding from the top
                            { Area.Y += intersection.Height; if (Velocity.Y < 0) { Velocity.Y = 0; } }
                            else // Colliding from the bottom
                            { Area.Y -= intersection.Height; if (Velocity.Y > 0) { Velocity.Y = 0; } }
                        }
                        else
                        {
                            if (blockArea.Center.X > Area.Center.X) // Colliding from the right
                            { Area.X -= intersection.Width; if (Velocity.X > 0) { Velocity.X = 0; } }
                            else // Colliding from the left
                            { Area.X += intersection.Width; if (Velocity.X < 0) { Velocity.X = 0; } }
                        }
                        return false;
                    }
                }
            }

            return true;

        }

    }
}
