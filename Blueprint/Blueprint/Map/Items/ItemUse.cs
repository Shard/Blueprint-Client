using System;
using System.Collections.Generic;

namespace Blueprint
{
    class ItemUse
    {

        public string Use;
        public string Value;
        public string Type;
        public int IntValue{
            get { return Int32.Parse(Value); } 
        }

        public void Clear()
        {
            Use = null;
            Value = null;
            Type = null;
        }

        public void Update(string raw_data)
        {
            Use = raw_data;
            Value = Use.Substring(Use.IndexOf(':') + 1);
            switch (Use.Substring(0, Use.IndexOf(':')))
            {
                case "block":
                    Type = "Block";break;
                case "weapon":
                    Type = "Weapon";break;
                case "wall":
                    Type = "BackgroundTile";break;
                case "mine":
                    Type = "Mine";break;
                default:
                    Type = null;break;
            }
        }

    }
}
