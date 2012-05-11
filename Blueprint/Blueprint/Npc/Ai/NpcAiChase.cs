using System;
using System.Collections.Generic;

namespace Blueprint
{
    class NpcAiChase : NpcAi
    {

        public override void Update(Movement movement, Player player)
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

            if (movement.Area.Center.Y > player.Movement.Area.Center.Y + 20)
            {
                movement.Intention.Jumping = true;
            }
            else
            {
                movement.Intention.Jumping = false;
            }

            if (movement.Area.Center.X - player.Movement.Area.X > -40 && movement.Area.Center.X - player.Movement.Area.X < 40)
            {
                movement.Intention.Left = false;
                movement.Intention.Right = false;
                movement.Intention.Jumping = false;
            }

        }

    }
}
