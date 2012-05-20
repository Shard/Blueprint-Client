using System;
using System.Collections.Generic;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Blueprint
{

    /// <summary>
    /// Used to describe animations which are managed by the Animations class
    /// </summary>
    class Animation
    {

        #region Variables

        /// <summary>
        /// The name of the animation sequence
        /// </summary>
        public string Name;

        /// <summary>
        /// The state of said animation (start, loop, end)
        /// </summary>
        public string State;

        /// <summary>
        /// The amount of frame to hold on the animation until the animation frame will move on
        /// </summary>
        public short Time;

        /// <summary>
        /// The default width of the spirtes in the animation sequence
        /// </summary>
        public short Width;

        /// <summary>
        /// The default height of the sprites in the animation sequence
        /// </summary>
        public short Height;

        /// <summary>
        /// The default hitbox height of the animation sequence
        /// </summary>
        public short HitboxHeight;

        /// <summary>
        /// The default hitbox width of the animation sequence
        /// </summary>
        public short HitboxWidth;

        /// <summary>
        /// The offset x of the whole animation sequence
        /// </summary>
        public short OffsetX;

        /// <summary>
        /// The offset y of the whole animation sequence
        /// </summary>
        public short OffsetY;

        /// <summary>
        /// The collection of all animation frames used in the animation
        /// </summary>
        public AnimationFrame[] Frames;

        /// <summary>
        /// Defines how many frames the current frame has been used for
        /// </summary>
        private short frame_upto;

        #endregion

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

        /// <summary>
        /// Autogenerates a set of frames based off amount
        /// </summary>
        /// <param name="amount"></param>
        public void AutoGenerate(short amount)
        {
            
            Frames = new AnimationFrame[amount];
            for (int i = 0; i < amount; i++)
            {
                Frames[i] = new AnimationFrame(Width,Height,(short)(OffsetX + (Width * i)), (short)OffsetY, Time);
            }
        }

        /// <summary>
        /// Takes an xml frame node and adds it to the animations current frames
        /// </summary>
        /// <param name="node"></param>
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

        /// <summary>
        /// Returns a rectangle of the current 
        /// </summary>
        /// <returns></returns>
        public Rectangle ToRectangle()
        {
            return new Rectangle();
        }



    }
}
