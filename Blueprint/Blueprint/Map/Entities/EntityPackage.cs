using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Blueprint.Events;

namespace Blueprint
{
    class EntityPackage
    {

        public Texture2D Texture;

        public Entity[] Entities;
        public EntityType[] Types;

        public EntityPackage()
        {
            Types = new EntityType[10];
            Types[0] = new EntityType(1, "flower", new Rectangle(0, 0, 24, 24), false, 1,1);
            Types[1] = new EntityType(2, "door", new Rectangle(24, 0, 24, 48), true, 1, 2);

            Entities = new Entity[10];
            Entities[0] = new Entity(Types[0], 7, 7);
            Entities[1] = new Entity(Types[1], 5, 6);
            Entities[1].Events.Add(new Event(Event.Triggers.PlayerInteract, Event.Actions.Solid, "toggle"));
            Entities[1].Events.Add(new Event(Event.Triggers.PlayerInteract, Event.Actions.Animation, "toggle"));
        }

        public void Initialize(Texture2D texture)
        {
            Texture = texture;
        }

        public void Update(Control control)
        {
            foreach (Entity item in Entities)
            {
                foreach (Event e in item.Events)
                {

                    // Proccess Triggers
                    switch (e.Trigger)
                    {
                        case Event.Triggers.PlayerInteract:
                            if (control.previousMouse.RightButton == ButtonState.Released && control.previousMouse.RightButton == ButtonState.Pressed)
                            {
                                if(Rectangle.Intersect(item.Area, new Rectangle(control.currentMouse.X,control.currentMouse.Y,1,1)) != null)
                                {
                                    e.TriggerEvent();
                                }
                            }
                            break;
                    }

                    // Proccess Conditions

                    // Proccess Actions
                    if (e.Active)
                    {
                        switch (e.Action)
                        {
                            case Event.Actions.Solid:
                                if (e.ActionData == "toggle")
                                {
                                    if (item.Solid) { item.Solid = false; } else { item.Solid = true; }
                                }
                                break;
                            case Event.Actions.Animation:
                                
                                break;
                        }
                    }


                }
            }
        }

        public void Draw(SpriteBatch spriteBatch, Camera camera)
        {

            foreach (Entity item in Entities)
            {
                if (item == null) { continue; }
                spriteBatch.Draw(Texture, camera.FromRectangle(item.Area), item.Type.Sprite, Color.White);
            }

        }

    }
}
