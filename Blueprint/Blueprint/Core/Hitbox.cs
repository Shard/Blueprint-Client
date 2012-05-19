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

        /// <summary>
        /// The compiled defense
        /// </summary>
        public int Defense;

        #endregion

        #region Stats

        public int Str;
        public int Agi;
        public int Int;
        public int Vit;

        #endregion

        #region Misc

        public byte Invunerable;

        public Rectangle Area;

        #endregion

        #region Internal

        private float Timer;

        #endregion

        public void Update(GameTime time)
        {

            Timer += (float)time.ElapsedGameTime.Seconds;

        }

        /// <summary>
        /// Calculates 
        /// </summary>
        public void Calculate()
        {

        }

    }
}
