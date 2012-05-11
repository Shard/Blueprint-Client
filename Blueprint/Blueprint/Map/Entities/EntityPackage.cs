﻿using System;
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

        public List<Entity> Entities;
        public EntityType[] Types;

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

        public void Initialize(Texture2D texture)
        {
            Texture = texture;
            Types[0] = new EntityType(1, "Door", texture, 1, 2, true, EntityType.EntityFunction.Door);
            Types[4] = new EntityType(1, "Door", texture, 1, 2, true, EntityType.EntityFunction.Door);
            Entities.Add(new Entity(Types[0], 10, 10));
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
                spriteBatch.Draw(getType(entity.Type).Sprite, camera.FromRectangle(entity.Area), Color.White);
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
