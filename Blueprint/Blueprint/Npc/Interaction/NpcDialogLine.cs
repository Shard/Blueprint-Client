using System;
using System.Collections.Generic;

namespace Blueprint
{
    class NpcDialogLine
    {

        public NpcDialogLine(string line, NpcInteraction.NpcInteractionState state)
        {
            Line = line;
            State = state;
        }

        public NpcInteraction.NpcInteractionState State;
        public string Line;

    }
}
