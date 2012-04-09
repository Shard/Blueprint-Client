using System;
using System.Collections.Generic;

namespace Blueprint
{
    class NpcAiChase : NpcAi
    {

        public void Update(Movement movement, Player player)
        {
            Console.WriteLine("a");
            if (movement.Area.Center.X > player.Movement.Area.Center.X)
            {
                movement.MovingLeft = true;
                movement.MovingRight = false;
            }
            else
            {
                movement.MovingLeft = false;
                movement.MovingRight = true;
            }

        }

    }
}
