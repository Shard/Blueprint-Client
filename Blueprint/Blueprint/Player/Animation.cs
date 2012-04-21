using System;
using System.Collections.Generic;
using System.Xml;

namespace Blueprint
{

    /// <summary>
    /// Used to describe animations which are managed by the Animations class
    /// </summary>
    class Animation
    {

        public string Name;
        public string State;
        public short Time;
        public short Width;
        public short Height;
        public short HitboxHeight;
        public short HitboxWidth;
        public short OffsetX;
        public short OffsetY;
        public AnimationFrame[] Frames;

        private short frame_upto;

        /// <summary>
        /// Constructer for the animation class
        /// </summary>
        /// <param name="name">Name of the animation sequence</param>
        /// <param name="width">Default width of the animation sequence</param>
        /// <param name="height">Default height of the animation sequence</param>
        /// <param name="offsetX">Offset in pixels for all frames</param>
        /// <param name="offsetY">Offset in pixels for all frames</param>
        /// <param name="state">start, loop or end</param>
        /// <param name="time">Amount of frames to wait before progressing</param>
        /// <param name="hitboxHeight">Hitbox height</param>
        /// <param name="hitboxWidth">Hitbox width</param>
        /// <param name="frames">The amount of frames to autogenerate</param>
        public Animation(string name, short width, short height, short frame_count, short offsetX = 0, short offsetY = 0, string state = "loop", short time = 1, short hitboxHeight = 0, short hitboxWidth = 0, short frames = 0)
        {
            Name = name;
            Width = width;
            Height = height;
            OffsetX = offsetX;
            OffsetY = offsetY;
            State = state;
            Time = time;
            HitboxHeight = hitboxHeight;
            HitboxWidth = hitboxWidth;
            Frames = new AnimationFrame[frame_count];

            if (frames > 0)
            {
                AutoGenerate(frames);
            }

            frame_upto = 0;


        }

        public void AutoGenerate(short amount)
        {
            
            Frames = new AnimationFrame[amount];
            for (int i = 0; i < amount; i++)
            {
                Frames[i] = new AnimationFrame(Width,Height,(short)(OffsetX + (Width * i)), (short)OffsetY, Time);
            }
        }

        public void AddFrame(XmlNode node)
        {
            AnimationFrame frame = new AnimationFrame(Width, Height, OffsetX, OffsetY, Time);
            foreach (XmlAttribute attr in node.Attributes)
            {
                switch (attr.Name)
                {
                    case "width":
                        frame.Width = Int16.Parse(attr.Value);
                        break;
                    case "height":
                        frame.Height = Int16.Parse(attr.Value);
                        break;
                    case "x":
                        frame.X = (short)(Int16.Parse(attr.Value) + OffsetX);
                        break;
                    case "y":
                        frame.Y = (short)(Int16.Parse(attr.Value) + OffsetY);
                        break;
                    case "hitboxwidth":
                        frame.HitboxWidth = Int16.Parse(attr.Value);
                        break;
                    case "hitboxheight":
                        frame.HitboxHeight = Int16.Parse(attr.Value);
                        break;
                    case "time":
                        frame.Time = Int16.Parse(attr.Value);
                        break;
                    default:
                        break;
                }
            }
            Frames[frame_upto] = frame;
            frame_upto++;
        }



    }
}
