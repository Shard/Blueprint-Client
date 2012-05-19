using System;
using System.Collections.Generic;

namespace Blueprint
{
    class NpcDialog
    {

        public List<NpcDialogLine> Lines;

        public NpcDialog()
        {
            Lines = new List<NpcDialogLine>();
        }

        /// <summary>
        /// Gets a dialog line to use via an npc interaction state
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public NpcDialogLine Get(NpcInteraction.NpcInteractionState state)
        {
            foreach (NpcDialogLine line in Lines)
            {
                if (line.State == state) { return line; }
            }
            return null;
        }

        public void Add(string line, NpcInteraction.NpcInteractionState state)
        {
            Lines.Add(new NpcDialogLine(line, state));
        }

    }
}
