using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Blueprint
{


    /// <summary>
    /// Attached to objects that can be damaged, handles health, mana, energy and damage
    /// </summary>
    class Hitbox
    {
        #region Calculated Stats

        /// <summary>
        /// The current health of the object
        /// </summary>
        public int Health;

        /// <summary>
        /// The maximun health of the object
        /// </summary>
        public int MaxHealth;

        /// <summary>
        /// The health regeneration per second
        /// </summary>
        public int HealthRegen;

        /// <summary>
        /// The current mana of the object
        /// </summary>
        public int Mana;

        /// <summary>
        /// The max mana of the object
        /// </summary>
        public int MaxMana;

        /// <summary>
        /// The mana regeneration per second
        /// </summary>
        public int ManaRegen;

        /// <summary>
        /// The current energy of the object
        /// </summary>
        public int Energy;

        /// <summary>
        /// The max energy of the object
        /// </summary>
        public int MaxEnergy;

        /// <summary>
        /// The energy regeneration per second
        /// </summary>
        public int EnergyRegen;

        /// <summary>
        /// A movement speed modifier based off compiled stats
        /// </summary>
        public float Speed;

        /// <summary>
        /// A speed modifider for attack speed based off compiled stats
        /// </summary>
        public float AttackSpeed;

        #endregion

        #region Stats

        /// <summary>
        /// The current strenght of the object
        /// </summary>
        public int Str;

        /// <summary>
        /// The current agility of the object
        /// </summary>
        public int Agi;

        /// <summary>
        /// The current inteligence of the object
        /// </summary>
        public int Int;

        /// <summary>
        /// The current vitality of the object
        /// </summary>
        public int Vit;

        /// <summary>
        /// The compiled defense
        /// </summary>
        public int Defense;

        #endregion

        #region Misc

        public byte Invunerable;

        public Rectangle Area;

        #endregion

        #region Internal

        /// <summary>
        /// Used for tracking the update tick which occurs once every second
        /// </summary>
        private float Timer;

        #endregion

        public Hitbox()
        {

        }

        public void Update(GameTime time)
        {

            Timer += (float)time.ElapsedGameTime.Seconds;

            if (Timer >= 1f)
            {

                // Apply Regen

                Timer -= 1f;
            }

        }

        /// <summary>
        /// Calculates 
        /// </summary>
        public void Calculate()
        {

        }

    }
}
