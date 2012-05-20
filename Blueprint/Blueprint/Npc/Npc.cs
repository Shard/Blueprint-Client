using System;
using System.Collections.Generic;

namespace Blueprint
{

    /// <summary>
    /// Represents the definition of a npc, contains dialog, drops, race, etc.
    /// </summary>
    class NpcType
    {

        /// <summary>
        /// The name of the npc type, will be used at the real npc by default
        /// </summary>
        public string Name;

        /// <summary>
        /// The shop that the npc runs
        /// </summary>
        public Shop Shop;

        /// <summary>
        /// The race the npc belongs to
        /// </summary>
        public NpcRace Race;

        /// <summary>
        /// The default Ai for the npc
        /// </summary>
        public NpcAi DefaultAi;

        /// <summary>
        /// A list of possible drops
        /// </summary>
        public List<DropChance> Drops;
        
        /// <summary>
        /// The dialog options for the npc
        /// </summary>
        public NpcDialog Dialog;

        /// <summary>
        /// The base stats of the npc
        /// </summary>
        public Stats BaseStats;

        public NpcType(string name, NpcRace race, NpcAi ai)
        {
            Name = name;
            Race = race;
            DefaultAi = ai;
            Shop = null;
            Drops = new List<DropChance>();
            Dialog = new NpcDialog();
            Dialog.Add("Intro text", NpcInteraction.NpcInteractionState.Intro);
            BaseStats = new Stats();
        }

    }
}
