﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Blueprint
{
    class DroppedItemCollection
    {

        public List<DroppedItem> Items;

        public DroppedItemCollection()
        {
            Items = new List<DroppedItem>();
        }

        public void Update(Map map, Player player)
        {
            for (int i = 0; i < Items.Count; i++)
            {
                DroppedItem item = Items[i];

                if (item.Movement.Velocity == Vector2.Zero && item.SettleCounter > 0)
                    item.SettleCounter -= 1;
                
                if(item.SettleCounter > 0)
                    Items[i].Movement.Update(map);


                if (item.Movement.Area.Intersects(player.Movement.Area))
                {
                    if (player.Inventory.Pickup(item.Item))
                        Items.Remove(item);
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch, Camera camera, ItemPackage itemPackage)
        {
            foreach (DroppedItem item in Items)
            {
                if (item == null) { continue; }
                spriteBatch.Draw(itemPackage.ItemTexture, camera.FromRectangle(item.Movement.Area), item.Item.Type.Location, Color.White);
            }
        }

        public void Add(DroppedItem drop)
        {
            Items.Add(drop);
        }

        public void Add(Vector2 location, Item item)
        {
            Items.Add(new DroppedItem(location, item));
        }

    }
}
