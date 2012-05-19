using System;
using System.Collections.Generic;

namespace Blueprint
{
    class NpcInteractionAction
    {

        /// <summary>
        /// The label of the action
        /// </summary>
        public string Label;


        public enum Interaction
        {
            End,
            Gossip,
            Intro,
            Shop,
            Follow,
            GoHome
        }

        public List<NpcInteractionAction> Actions;
    }
}
