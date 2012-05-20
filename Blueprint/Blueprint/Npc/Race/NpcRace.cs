using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Blueprint
{

    /// <summary>
    /// Handles functions shared amoung npcs of the same race such as animation and base stats
    /// </summary>
    class NpcRace
    {

        /// <summary>
        /// The name of the race
        /// </summary>
        public string Name;

        /// <summary>
        /// Handles animations for the npc's race
        /// </summary>
        public Animations Animation;

        /// <summary>
        /// The sprite texture that will be used for rendering
        /// </summary>
        public Texture2D Texture;

        public NpcRace(string name, Texture2D sprite, string xml_animation)
        {
            Name = name;
            Texture = sprite;
            Animation = new Animations(xml_animation);
        }

    }
}
