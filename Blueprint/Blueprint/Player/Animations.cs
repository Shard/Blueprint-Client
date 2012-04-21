using System;
using System.Collections.Generic;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Blueprint
{

    /// <summary>
    /// Manages animations for objects such as players, npcs and furniture
    /// </summary>
    class Animations
    {

        public Animation[] Collection;

        public Animation CurrentAnimation;
        public short TimeUpto;
        public short FrameUpto;
        public string CurrentDirection;
        public Rectangle CurrentLocation;

        public Animations(string data)
        {

            TimeUpto = 0;
            FrameUpto = 0;

            XmlDocument xml = new XmlDocument();
            xml.LoadXml(data);

            Int16 global_width = Int16.Parse(xml.DocumentElement.GetAttribute("width"));
            Int16 global_height = Int16.Parse(xml.DocumentElement.GetAttribute("height"));

            foreach (XmlNode animationNode in xml.DocumentElement.ChildNodes)
            {
                if (animationNode.Name != "Animations") { continue; }

                Collection = new Animation[ animationNode.ChildNodes.Count ];
                byte i = 0;
                foreach (XmlNode node in animationNode.ChildNodes)
                {
                    if(node.Name != "animation") {continue; }

                    Animation currentAnimation = new Animation(node.Attributes["name"].Value, global_width, global_height, (short)node.ChildNodes.Count);

                    foreach (XmlAttribute item in node.Attributes)
                    {
                        switch (item.Name)
                        {
                            case "width":
                                currentAnimation.Width = Int16.Parse(item.Value);break;
                            case "height":
                                currentAnimation.Height = Int16.Parse(item.Value);break;
                            case "offsetx":
                                currentAnimation.OffsetX = Int16.Parse(item.Value);break;
                            case "offsety":
                                currentAnimation.OffsetY = Int16.Parse(item.Value);break;
                            case "state":
                                currentAnimation.State = item.Value;break;
                            case "time":
                                currentAnimation.Time = Int16.Parse(item.Value);break;
                            case "frames":
                                short frames = Int16.Parse(item.Value);
                                if (frames > 0) { currentAnimation.AutoGenerate(frames); }
                                break;
                            case "hitboxheight":
                                currentAnimation.HitboxHeight = Int16.Parse(item.Value);
                                break;
                            case "hitboxwidth":
                                currentAnimation.HitboxWidth = Int16.Parse(item.Value);
                                break;
                            default:
                                break;
                        }
                    }


                    if (node.ChildNodes.Count > 0)
                    {
                        foreach (XmlNode frame in node.ChildNodes)
                        {
                            currentAnimation.AddFrame(frame);
                        }
                    }
                    

                    Collection[i] = currentAnimation;
                    i++;
                }
            }

            CurrentAnimation = Get("default");


        }

        public Animation Get(string name, string state = "loop")
        {
            foreach (Animation item in Collection)
            {
                if (item.Name == name && item.State == state) { return item; }
            }
            if (name == "default" && state == "loop") { throw new NotFiniteNumberException(); }
            return null;
        }

        public void UseAnimation(string name, string state = "loop")
        {
            CurrentAnimation = Get(name, state);
            FrameUpto = 0;
            TimeUpto = 0;
        }

        public void Update(Movement movement)
        {

            if (movement.Direction != CurrentDirection)
            {
                CurrentDirection = movement.Direction;
            }

            // Same animation, moving on
            TimeUpto++;
            if (TimeUpto > CurrentAnimation.Time)
            {
                TimeUpto = 0;
                FrameUpto++;
                if (FrameUpto >= CurrentAnimation.Frames.Length)
                {
                    UseAnimation(CurrentAnimation.Name, "loop");
                }

            }

            CurrentLocation = movement.Area;

        }

        public void Draw(SpriteBatch spriteBatch, Camera camera, Texture2D texture)
        {
            SpriteEffects effects = SpriteEffects.None;
            if (CurrentDirection == "right")
            {
                effects = SpriteEffects.FlipHorizontally;
            }

            spriteBatch.Draw(texture, camera.FromRectangle(CurrentLocation), CurrentAnimation.Frames[FrameUpto].ToRectangle(), Color.White, 0f, Vector2.Zero, effects, 0f);
        }

    }
}
