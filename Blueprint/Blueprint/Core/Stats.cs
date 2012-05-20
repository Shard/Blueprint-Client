using System;
using System.Collections.Generic;

namespace Blueprint
{

    /// <summary>
    /// A struct that can be used for storing player attributes
    /// </summary>
    struct Stats
    {

        #region Variables

        /// <summary>
        /// Strength
        /// </summary>
        public int Str;

        /// <summary>
        /// Inteligence
        /// </summary>
        public int Int;

        /// <summary>
        /// Agility
        /// </summary>
        public int Agi;

        /// <summary>
        /// Vitality
        /// </summary>
        public int Vit;

        /// <summary>
        /// Defense
        /// </summary>
        public int Def;

        #endregion

        public Stats(int str = 10, int intel = 10, int agi = 10, int vit = 10, int def = 10)
        {
            Str = str;
            Int = intel;
            Agi = agi;
            Vit = vit;
            Def = def;
        }

    }
}
