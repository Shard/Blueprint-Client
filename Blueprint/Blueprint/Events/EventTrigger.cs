using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blueprint.Events
{
    struct EventTrigger
    {

        public enum Type
        {
            PlayerInteract,
            PlayerEnter,
            PlayerLeave,
            PlayerNear
        }

        public string Data;

    }
}
