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
        public Movement Movement; // Handles movement and collisions
        public bool PositionCamera;

        // Sprites
        public Texture2D PlayerTexture;

        // Animation
        public int FrameCount; // How many frames are avaliable for looping
        public int FrameUpto; // What frame the player is up to
        public int FrameSkipCount; // How many frames to wait before an animation occurs.
        public int FrameSkipUpto; // How many skip frames the player is up to

        public Player(Vector2 position)
        {
            Movement = new Movement(position, 32, 44);
        }

        public void Initialize(Texture2D playerTexture)
        {

            PlayerTexture = playerTexture;

            FrameCount = 4;
            FrameSkipCount = 8;
            FrameSkipUpto = 1;

            Health = 100;
            Mana = 100;
            Speed = 4f;
            Name = "Firebolt";
            Inventory = new Inventory();

        }

        public void UpdateControls()
        {

        }

        public void UpdatePosition()
        {

        }

        public void Update(Control control, Map map)
        {
            Movement.Update(map);
            Animate(control);

            // Killing
            if (Movement.Area.Y > map.SizeY * 24)
            {
                Damage(5000);
            }
        }

        public void Animate(Control control)
        {

            // Animation
            if (control.currentKeyboard.IsKeyDown(Keys.A) && control.previousKeyboard.IsKeyDown(Keys.A) && !control.currentKeyboard.IsKeyDown(Keys.D))
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
            }
            else if (control.currentKeyboard.IsKeyDown(Keys.D) && control.previousKeyboard.IsKeyDown(Keys.D) && !control.currentKeyboard.IsKeyDown(Keys.A))
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
            }
            else
            {
                FrameUpto = 1;
                FrameSkipUpto = 1;
            }

        }

        public void Draw(SpriteBatch spriteBatch, Camera camera)
        {

            if (PositionCamera)
            {
                // TODO, recenter camera
                PositionCamera = false;
            }

            int spriterow = 44;
            if (Movement.Direction == "left")
            {
                spriterow = 0;
            }

            int spritecol = (FrameUpto - 1) * 32;


            if (Movement.Intention.Jumping)
            {
                spriterow += (2 * 44);
            } else if (Movement.Falling)
            {
                spriterow += (4 * 44);
            }
            
            spriteBatch.Draw(PlayerTexture, camera.FromRectangle(Movement.Area), new Rectangle(spritecol, spriterow, 32, 44), Color.White);

        }

        /// <summary>
        /// Generates a refresh command which is used by the net classes for communication
        /// </summary>
        /// <returns></returns>
        public string NetCommand()
        {
            string command = "r:";

            // Location
            command += Movement.Area.X.ToString() + ',' + Movement.Area.Y.ToString();

            return command + ";";
        }

        public bool Damage(int amount)
        {

            Health -= amount;
            if (Health <= 0)
            {
                // Die
                Health = 100;
                Movement = new Movement(new Vector2(100, -100), 32, 44);
                PositionCamera = true;
                return true;
            }

            return false;
        }
    }
}
