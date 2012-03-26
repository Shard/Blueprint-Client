using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Blueprint
{
    class Player
    {

        // Game Variables
        public string Name; // Name of the player
        public int Health; // Current Health of the player
        public int Mana; // Current Mana of the player
        public float Speed; // Current Speed of the palyer
        public Inventory Inventory; // Inventory of the player

        // Positioning and movement
        public Vector2 Position; // The X/Y position of the player
        public Rectangle PlayerArea; // The rectangle the player occupies
        public Vector2 LastMovement; // Last movement that was applied
        public bool Jumping; // If player is jumping
        public bool Falling; // If the player is falling, and thus cannot jump
        public float AmountJumped; // The amount the player has jumped
        public float MaxJump; // Maximun Jump Height

        // Sprites
        public Texture2D PlayerTexture;

        // Animation
        public int FrameCount; // How many frames are avaliable for looping
        public int FrameUpto; // What frame the player is up to
        public int FrameSkipCount; // How many frames to wait before an animation occurs.
        public int FrameSkipUpto; // How many skip frames the player is up to
        public string Direction; // Direction the player is facing

        public void Initialize(Texture2D playerTexture, Vector2 position)
        {

            PlayerTexture = playerTexture;
            Position = position;

            Jumping = false;
            AmountJumped = 0;
            MaxJump = 100;

            FrameCount = 4;
            FrameSkipCount = 8;
            FrameSkipUpto = 1;

            Health = 100;
            Mana = 100;
            Speed = 4f;
            Name = "Firebolt";
            Inventory = new Inventory();
        }

        public void Update(KeyboardState currentKeyboardState, KeyboardState previousKeyboardState, Map map)
        {

            // Collision
            PlayerArea = new Rectangle((int)Position.X, (int)Position.Y, 32, 48);
            bool stopLeft = false;
            bool stopRight = false;
            bool stopTop = false;
            bool stopBottom = false;
            bool stopDiagonal = false;


            int totalWidth = 0; // Total width of the collision edges
            int totalHeight = 0; // Total Height of the collision edges
            Vector2 initialCenterBlock = Vector2.Zero; // The center of the first block, used to check for diagonal collision
            Vector2 centerBlock = Vector2.Zero; // Center of all the blocks put together
            
            // Collect Intersections and calculate some numbers
            for (int x = 0; x < 100; x++)
            {
                for (int y = 0; y < 100; y++)
                {
                    if (map.Blocks[x, y] == null) { continue; }
                    Rectangle blockArea = new Rectangle(x*32,y*32,32,32);
                    Rectangle intersection = Rectangle.Intersect(PlayerArea, blockArea);

                    if (intersection != Rectangle.Empty)
                    {
                        totalHeight += intersection.Height;
                        totalWidth += intersection.Width;
                        if (centerBlock == Vector2.Zero)
                        {
                            centerBlock = new Vector2(blockArea.Center.X, blockArea.Center.Y);
                            initialCenterBlock = centerBlock;
                        }
                        else
                        {
                            centerBlock.X = (centerBlock.X + blockArea.Center.X) / 2;
                            centerBlock.Y = (centerBlock.Y + blockArea.Center.Y) / 2;
                        }
                    }
                }
            }

            // If collision detected, Stop movement
            if (centerBlock != Vector2.Zero)
            {
                if (totalHeight > totalWidth)
                {
                    // Horizontal
                    if (PlayerArea.Center.X > centerBlock.X)
                    {
                        stopLeft = true;
                    }
                    else
                    {
                        stopRight = true;
                    }

                    if (initialCenterBlock.X != centerBlock.X) // Diagonal collision
                    {
                        stopDiagonal = true;
                    }
                }

                if (totalHeight < totalWidth || stopDiagonal)
                {
                    // Vertical
                    // Horizontal
                    if (PlayerArea.Center.Y > centerBlock.Y)
                    {
                        stopTop = true;
                        Jumping = false;
                        Falling = true;
                    }
                    else
                    {
                        stopBottom = true;
                    }
                }
            }

            LastMovement = Vector2.Zero;

            // Movement

            if (currentKeyboardState.IsKeyDown(Keys.A) && !stopLeft)
            {
                LastMovement.X -= Speed;
                Direction = "left";
            }
            if (currentKeyboardState.IsKeyDown(Keys.D) && !stopRight)
            {
                LastMovement.X += Speed;
                Direction = "right";
            }

            if (currentKeyboardState.IsKeyDown(Keys.Space))
            {
                if (previousKeyboardState.IsKeyDown(Keys.Space) && Jumping) // Continue Jumping
                {
                    AmountJumped += Speed;
                }

                if (!previousKeyboardState.IsKeyDown(Keys.Space) && !Falling && !Jumping) // Started Jumping
                {
                    AmountJumped = Speed;
                    Jumping = true;
                }
            }

            if (AmountJumped >= MaxJump || stopTop)
            {
                Jumping = false;
                Falling = true;
            }

            if (previousKeyboardState.IsKeyDown(Keys.Space) && !currentKeyboardState.IsKeyDown(Keys.Space) && Jumping)
            {
                Falling = true;
                Jumping = false;
                AmountJumped = MaxJump;
            }

            if (Jumping)
            {
                LastMovement.Y -= Speed * (float)2.5;
            }


            if (!stopBottom) // Gravity
            {
                LastMovement.Y += Speed;
            } else {
                Falling = false;
            }

            // Animation
            if (currentKeyboardState.IsKeyDown(Keys.A) && previousKeyboardState.IsKeyDown(Keys.A) && !currentKeyboardState.IsKeyDown(Keys.D))
            {
                FrameSkipUpto++;
                if(FrameSkipUpto >= FrameSkipCount){
                    FrameSkipUpto = 1;
                    FrameUpto++;
                    if (FrameUpto >= FrameCount)
                    {
                        FrameUpto = 1;
                    }
                }
            }
            else if (currentKeyboardState.IsKeyDown(Keys.D) && previousKeyboardState.IsKeyDown(Keys.D) && !currentKeyboardState.IsKeyDown(Keys.A))
            {
                FrameSkipUpto++;
                if (FrameSkipUpto >= FrameSkipCount)
                {
                    FrameSkipUpto = 1;
                    FrameUpto++;
                    if (FrameUpto >= FrameCount)
                    {
                        FrameUpto = 1;
                    }
                }
            } else {
                FrameUpto = 1;
                FrameSkipUpto = 1;
            }

            Position += LastMovement;

        }

        public void Draw(SpriteBatch spriteBatch, Vector2 camera)
        {

            int spriterow = 44;
            if (Direction == "left")
            {
                spriterow = 0;
            }

            int spritecol = (FrameUpto - 1) * 32;

            if (Falling)
            {
                spriterow += (4 * 44);
            } else if (Jumping)
            {
                spriterow += (2 * 44);
            }

            spriteBatch.Draw(PlayerTexture, Position + camera, new Rectangle(spritecol, spriterow, 32, 44), Color.White);

        }

    }
}
