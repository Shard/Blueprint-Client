using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blueprint.Events
{
    struct EventAction
    {

        public enum Type
        {
            Dialogue,
            Bank,
            Explode,
            Light,
            Solid,
            Animation
        }

        public string Data;

    }
}
