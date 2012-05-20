using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Blueprint
{
    class Player
    {

        #region Base Variables

        /// <summary>
        /// The base max health of the player
        /// </summary>
        protected int BaseMaxHeath;

        /// <summary>
        /// The base max mana of the player
        /// </summary>
        protected int BaseMaxMana;

        #endregion

        #region Game Variables

        /// <summary>
        /// Name of the player
        /// </summary>
        public string Name;

        /// <summary>
        /// Current Health of the player
        /// </summary>
        public int Health; // Current Health of the player

        /// <summary>
        /// The current mana of the player
        /// </summary>
        public int Mana;

        /// <summary>
        /// If is above 0 the player is invunerable, 
        /// </summary>
        public byte Invunerable;
        public float Speed; // Current Speed of the palyer
        public Inventory Inventory; // Inventory of the player
        public int HealthRegenCounter;

        #endregion

        // Positioning and movement
        public Movement Movement; // Handles movement and collisions
        public bool PositionCamera;

        // Sprites
        public Texture2D PlayerTexture;
        public Texture2D BarsTexture;

        // Animation
        Animations Animation;

        public void Initialize(Texture2D playerTexture, Texture2D barsTexture, Package package, Vector2 position)
        {

            BarsTexture = barsTexture;
            PlayerTexture = playerTexture;

            Health = 100;
            Mana = 100;
            Speed = 4f;
            Name = "Firebolt";
            Inventory = new Inventory();
            
            // Animation
            Animation = new Animations(package.LocalString("C:\\blueprint\\player.xml", false));
            Movement = new Movement(position, 32, 44);
        }

        public void Update(Control control, Map map)
        {
            Movement.Update(map);
            Animation.Update(Movement);

            if (Invunerable > 0) { Invunerable -= 1; }

            HealthRegenCounter++;
            if (HealthRegenCounter >= 30)
            { HealthRegenCounter = 0; if (Health < 100) { Health++; } }

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

        public void DrawUi(SpriteBatch spriteBatch, SpriteFont font)
        {

            // Health Bar
            Rectangle rect =  new Rectangle(Inventory.Quickbar.Area.X , Inventory.Quickbar.Area.Y - 30, Inventory.Quickbar.Area.Width / 2 -2, 28);
            Vector2 textAnchor = new Vector2(Inventory.Quickbar.Area.Center.X - 8, Inventory.Quickbar.Area.Y - 24);
            Rectangle health_rect = rect;
            health_rect.Width = (int)( (Inventory.Quickbar.Area.Width / 2) * (float)(Health * 0.01)) - 2;
            health_rect.X += Inventory.Quickbar.Area.Width / 2 - (int)( (Inventory.Quickbar.Area.Width / 2) * (float)(Health * 0.01));

            spriteBatch.Draw(BarsTexture, health_rect, new Rectangle(0, 56, BarsTexture.Width, 28), Color.White);
            spriteBatch.Draw(BarsTexture, health_rect, new Rectangle(0, 0, BarsTexture.Width, 28), Color.White);
            TextHelper.DrawString(spriteBatch, font, Health.ToString() + "/100", textAnchor, Color.White, align: "right", scale: 0.7f);

            // Mana Bar
            rect.X += Inventory.Quickbar.Area.Width / 2;
            textAnchor.X += 16;
            spriteBatch.Draw(BarsTexture, rect, new Rectangle(0, 28, BarsTexture.Width, 28), Color.White);
            spriteBatch.Draw(BarsTexture, rect, new Rectangle(0, 0, BarsTexture.Width, 28), Color.White);
            TextHelper.DrawString(spriteBatch, font, Mana.ToString() + "/100", textAnchor, Color.White, align: "left", scale: 0.7f);

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
            if (Invunerable > 0) { return false; }
            Health -= amount;
            if (Health <= 0)
            {
                // Die
                Health = 100;
                Movement = new Movement(new Vector2(100, -100), 32, 44);
                PositionCamera = true;
                return true;
            }
            Invunerable = 10;
            return false;
        }
    }
}
