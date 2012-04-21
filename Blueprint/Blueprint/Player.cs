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
        Animations Animation;

        public Player(Vector2 position)
        {
            Movement = new Movement(position, 32, 44);
        }

        public void Initialize(Texture2D playerTexture, Package package)
        {
            

            PlayerTexture = playerTexture;

            Health = 100;
            Mana = 100;
            Speed = 4f;
            Name = "Firebolt";
            Inventory = new Inventory();
            
            // Animation
            Animation = new Animations(package.LocalString("C:\\blueprint\\player.xml", false));
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
            Animation.Update(Movement);

            // Killing
            if (Movement.Area.Y > map.SizeY * 24)
            {
                Damage(5000);
            }
        }

        public void Draw(SpriteBatch spriteBatch, Camera camera)
        {
            Animation.Draw(spriteBatch, camera, PlayerTexture);
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
