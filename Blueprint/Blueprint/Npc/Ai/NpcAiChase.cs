using System;
using System.Collections.Generic;

namespace Blueprint
{
    class NpcAiChase : NpcAi
    {

        public void Update(Movement movement, Player player)
        {
            if (movement.Area.Center.X > player.Movement.Area.Center.X)
            {
                movement.Intention.Left = true;
                movement.Intention.Right = false;
            }
            else
            {
                movement.Intention.Left = false;
                movement.Intention.Right = true;
            }

        }

    }
}
