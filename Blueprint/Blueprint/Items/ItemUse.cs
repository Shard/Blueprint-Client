﻿using System;
using System.Collections.Generic;

namespace Blueprint
{
    class ItemUse
    {
        /// <summary>
        /// Raw use string
        /// </summary>
        public string Use;

        /// <summary>
        /// The value of the item use
        /// </summary>
        public string Value;

        /// <summary>
        /// The type of item use
        /// </summary>
        public string Type;

        /// <summary>
        /// The current charge that has built on the current item
        /// </summary>
        public float Charge;

        /// <summary>
        /// The maximun charge that can be built on the current item
        /// </summary>
        public int MaxCharge;

        public int IntValue{
            get { return Int32.Parse(Value); } 
        }

        public ItemUse() { }

        public ItemUse(string data)
        {
            Update(data);
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
            Type = Use.Substring(0, Use.IndexOf(':'));
        }

    }
}
