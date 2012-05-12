using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Blueprint.Events;

namespace Blueprint
{
    class EntityPackage
    {

        public Texture2D Texture;

        public List<Entity> Entities;
        public EntityType[] Types;

        /// <summary>
        /// If is set, will preview where an entity will build
        /// </summary>
        public Entity Preview;

        public EntityPackage()
        {
            Types = new EntityType[10];
            //Types[0] = new EntityType(1, "flower", new Rectangle(0, 0, 24, 24), false, 1,1);
            //Types[1] = new EntityType(2, "door", new Rectangle(24, 0, 24, 48), true, 1, 2);

            Entities = new List<Entity>();
            //Entities[0] = new Entity(Types[0], 7, 7);
            //Entities[1] = new Entity(Types[1], 5, 6);
            //Entities[1].Events.Add(new Event(Event.Triggers.PlayerInteract, Event.Actions.Solid, "toggle"));
            //Entities[1].Events.Add(new Event(Event.Triggers.PlayerInteract, Event.Actions.Animation, "toggle"));
        }

        public void Initialize(ContentManager content)
        {
            Texture = content.Load<Texture2D>("furniture");
            Types[0] = new EntityType(1, "Door", content.Load<Texture2D>("Entities/door"), 1, 3, true, EntityType.EntityFunction.Door);
            //Entities.Add(new Entity(Types[0], 10, 10));
        }

        public void Update(Control control, Camera camera)
        {
            foreach (Entity item in Entities)
            {
                if (item == null) { continue; }

                EntityType type = getType(item.Type);
                if (type.Function == EntityType.EntityFunction.Door)
                {
                    if (camera.FromRectangle(item.Area).Intersects(control.MousePos))
                    {
                        control.State = Control.CursorStates.Interact;
                        if (control.currentMouse.RightButton == ButtonState.Pressed && control.previousMouse.RightButton == ButtonState.Released)
                        {
                            if (item.Solid) { item.Solid = false; } else { item.Solid = true; }
                        }
                    }
                }
                else
                {
                    #region Custom Entities
                    foreach (Event e in item.Events)
                    {

                        // Proccess Triggers
                        switch (e.Trigger)
                        {
                            case Event.Triggers.PlayerInteract:
                                if (control.previousMouse.RightButton == ButtonState.Released && control.currentMouse.RightButton == ButtonState.Pressed)
                                {
                                    if (Rectangle.Intersect(item.Area, camera.FromRectangle(new Rectangle(control.currentMouse.X, control.currentMouse.Y, 1, 1))) != Rectangle.Empty)
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
                                    if (item.AltSprite) { item.AltSprite = false; } else { item.AltSprite = true; }
                                    break;
                            }
                            e.Active = false; // Finish Firing Event
                        }


                    }
                    #endregion
                }

              
            }
        }

        public void Draw(SpriteBatch spriteBatch, Camera camera)
        {

            foreach (Entity entity in Entities)
            {
                if (entity == null) { continue; }
                EntityType type = getType(entity.Type);
                if (type.Function == EntityType.EntityFunction.Door)
                {
                    if(entity.Solid)
                        spriteBatch.Draw(type.Sprite, camera.FromRectangle(entity.Area), new Rectangle(0,0, type.Sprite.Width / 2, type.Sprite.Height), Color.White);
                    else
                        spriteBatch.Draw(type.Sprite, camera.FromRectangle(entity.Area), new Rectangle(type.Sprite.Width / 2, 0, type.Sprite.Width / 2, type.Sprite.Height), Color.White);
                }
                else
                {
                    spriteBatch.Draw(getType(entity.Type).Sprite, camera.FromRectangle(entity.Area), Color.White);
                }
                
            }

        }

        public EntityType getType(int id)
        {
            return Types[0];
            for (int i = 0; i < Types.Length; i++)
            {
                if (Types[i] != null && Types[i].Id == id) { return Types[i]; }
            }
            return null;
        }

        public Entity Get(int x, int y)
        {
            Rectangle rect = new Rectangle(x * 24, y * 24, 24, 24);
            foreach (Entity entity in Entities)
                { if (rect.Intersects(entity.Area)) { return entity; } }
            return null;
        }

        public bool Add(int x, int y, EntityType type)
        {
            if (Get(x, y) != null) { return false; }
            Entities.Add(new Entity(type, x, y));
            return true;
        }

        public bool Add(int x, int y, int type)
        {
            if (Get(x, y) != null) { return false; }
            Entities.Add(new Entity(getType(type), x, y));
            return true;
        }


        /// <summary>
        /// Attempt to deal damage to entities at x,y. returns true if destroyed
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool Damage(int x, int y)
        {
            Entity entity = Get(x, y);
            if (entity == null) { return false; }
            entity.Health -= 1;
            if (entity.Health < 1)
                { Entities.Remove(entity); return true; }
            return false;
        }

        

    }
}
