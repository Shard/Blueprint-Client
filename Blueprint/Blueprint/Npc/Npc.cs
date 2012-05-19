using System;
using System.Collections.Generic;

namespace Blueprint
{
    class Npc
    {

        public string Name;
        public int Health;

        public Weapon Weapon; // Weapon Equiped
        public Shop Shop; // Contains items that the NPC is selling
        public NpcRace Race; // The Npc's race
        public NpcAi Ai; // The Npc's AI
        public List<DropChance> Drops; // Possible Items that an NPC can drop
        public bool Friendly;
        public NpcDialog Dialog;

        public Npc(string name, NpcRace race, NpcAi ai)
        {
            Name = name;
            Race = race;
            Ai = ai;
            Weapon = null;
            Shop = null;
            Drops = new List<DropChance>();
            Dialog = new NpcDialog();
            Dialog.Add("Intro text", NpcInteraction.NpcInteractionState.Intro);
            Friendly = true;
            Health = Race.Health;
        }

    }
}
