using System;
using System.Collections.Generic;
using System.Xml;

namespace Blueprint
{

    /// <summary>
    /// Manages animations for objects such as players, npcs and furniture
    /// </summary>
    class Animations
    {

        public Animation[] Collection;

        public Animations(string data)
        {
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(data);

            Int16 global_width = Int16.Parse(xml.DocumentElement.GetAttribute("width"));
            Int16 global_height = Int16.Parse(xml.DocumentElement.GetAttribute("height"));

            foreach (XmlNode animationNode in xml.DocumentElement.ChildNodes)
            {
                if (animationNode.Name != "Animations") { continue; }

                Collection = new Animation[ animationNode.ChildNodes.Count ];

                foreach (XmlNode node in animationNode.ChildNodes)
                {
                    if(node.Name != "animation") {continue; }

                    Animation currentAnimation = new Animation(node.Attributes["name"].ToString());



                    Collection[Collection.Length] = currentAnimation;

                }
            }

        }

    }
}
