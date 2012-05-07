﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Blueprint
{
    class WeaponPackage
    {

        public Weapon[] Weapons;
        public WeaponType[] Types;
        public List<dynamic> LiveWeapons;
        public Texture2D WeaponTexture;

        public WeaponPackage()
        {
            
            // Types
            Types = new WeaponType[10];
            Types[0] = new WeaponType("Sword", true);
            Types[1] = new WeaponType("Axe", true);
            Types[2] = new WeaponType("Bow", true);
            Types[3] = new WeaponType("Arrow", false);

            // Weapons
            Weapons = new Weapon[10];
            

            // Live Weapons
            LiveWeapons = new List<dynamic>();

        }

        public void Initialize(Texture2D weaponTexture)
        {
            WeaponTexture = weaponTexture;
            Weapons[0] = new Weapon(11, Types[0], "Big Stick", new Rectangle(0, 0, WeaponTexture.Bounds.Width, WeaponTexture.Bounds.Height));
            Weapons[1] = new Weapon(12, Types[2], "Bow", new Rectangle(0, 0, WeaponTexture.Bounds.Width, WeaponTexture.Bounds.Height));
            Weapons[2] = new Weapon(13, Types[3], "Wooden Arrow", new Rectangle(0, 0, WeaponTexture.Bounds.Width, WeaponTexture.Bounds.Height));
        }

        public void Update(Control control, Camera camera, Player player, NpcPackage npcs, Ui.FloatingTextCollection floatingText, Map map)
        {

            #region Initialize Weapons

            if (player.Inventory.Quickbar.UsingItem.Type == "Weapon")
            {
                ItemUse use = player.Inventory.Quickbar.UsingItem;
                Weapon weapon = GetWeaponById(int.Parse(use.Value));
                
                if (weapon.Type.Name == "Sword" && !player.Inventory.Quickbar.IsUsingItem)
                {
                    LiveWeapons.Add(new LiveSword(GetWeaponById(player.Inventory.Quickbar.UsingItem.IntValue), startingSwordPosition(player, new Rectangle(0, 0, 47, 79)), player.Movement.Direction));
                    player.Inventory.Quickbar.IsUsingItem = true;
                }

                if (weapon.Type.Name == "Arrow")
                {
                    if(control.currentMouse.LeftButton == ButtonState.Pressed){
                        if(!player.Inventory.Quickbar.IsUsingItem) {
                            player.Inventory.Quickbar.IsUsingItem = true;
                            use.Charge = 10f;
                        } else {
                            use.Charge += 0.1f;
                        }
                    } else {
                        LiveProjectile liveWeapon = new LiveProjectile(Weapons[2], new Vector2(player.Movement.Area.Center.X, player.Movement.Area.Center.Y), (float)use.Charge, control, camera);
                        use.Charge = 0;
                        LiveWeapons.Add(liveWeapon);
                        player.Inventory.Quickbar.IsUsingItem = false;
                    }
                    Console.WriteLine(use.Charge);
                }
            }

            #endregion

            #region Update weapons

            for ( int i = 0; i < LiveWeapons.Count; i++)
            {
                if (LiveWeapons[i].Weapon.Type.Name == "Sword")
                {
                    LiveSword weapon = LiveWeapons[i];
                    //LiveWeapons[i].currentLocation.Rotation += LiveWeapons[i].Speed;
                    LiveWeapons[i].Rotation += LiveWeapons[i].Speed / 5;
                    LiveWeapons[i].Location += player.Movement.Moved;
                    if ((LiveWeapons[i].SwingEnd > 0 && LiveWeapons[i].Rotation > LiveWeapons[i].SwingEnd) || (LiveWeapons[i].SwingEnd < 0 && LiveWeapons[i].Rotation < LiveWeapons[i].SwingEnd))
                    {
                        LiveWeapons.RemoveAt(i);
                        player.Inventory.Quickbar.IsUsingItem = false;
                    }
                } else if(LiveWeapons[i].Weapon.Type.Name == "Arrow")
                {
                    LiveProjectile weapon = LiveWeapons[i];
                    if (weapon.Movement.Velocity != Vector2.Zero)
                    {
                        weapon.Movement.Update(map);
                    }
                    else
                    {
                        weapon.TimeToLive -= 1;
                        if (weapon.TimeToLive <= 0) { LiveWeapons.RemoveAt(i); }
                    }
                }
            }

            #endregion

            #region Check for weapon collisions

            foreach (var weapon in LiveWeapons)
            {
                for (int i = 0; i < npcs.ActiveNpcs.Count; i++)
                {

                    if (weapon.Weapon.Type.Name == "Sword")
                    {
                        LiveSword sword = weapon;
                        if (weapon.currentLocation.Intersects(npcs.ActiveNpcs[i].Movement.Area))
                        {
                            if (npcs.Damage(npcs.ActiveNpcs[i], 10, weapon.Location))
                            {
                                floatingText.Add("10", weapon.Location);
                            }
                        }
                    }
                    else if(weapon.Weapon.Type.Name == "Arrow" && weapon.Movement.Velocity != Vector2.Zero)
                    {
                        LiveProjectile projectile = weapon;
                        if (projectile.Movement.Area.Intersects(npcs.ActiveNpcs[i].Movement.Area))
                        {
                            if (npcs.Damage(npcs.ActiveNpcs[i], 10, weapon.Location))
                            {
                                floatingText.Add("10", new Vector2(projectile.Movement.Area.Center.X, projectile.Movement.Area.Center.Y));
                            }
                        }
                    }
                }
            }
            #endregion


        }

        public Vector2 startingSwordPosition( Player player, Rectangle weaponArea )
        {
            if (player.Movement.Direction == "right")
            {
                return new Vector2(player.Movement.Area.Center.X, player.Movement.Area.Top);
            } else {
                return new Vector2(player.Movement.Area.Center.X - weaponArea.Width, player.Movement.Area.Top);
            }

        }

        public void Draw(SpriteBatch spriteBatch, Camera camera)
        {

            foreach (var weapon in LiveWeapons)
            {
                if (weapon.Weapon.Type.Name == "Sword")
                {
                    //spriteBatch.Draw(WeaponTexture, camera.FromRectangle(new Rectangle((int)weapon.Location.X, (int)weapon.Location.Y, 47, 79)), new Rectangle(0,0,47,79), Color.White, weapon.Rotation, Vector2.Zero, SpriteEffects.None, 0 );
                    // Draw hitboxes
                    spriteBatch.Draw(WeaponTexture, camera.FromRectangle(new Rectangle((int)weapon.currentLocation.UpperLeftCorner().X, (int)weapon.currentLocation.UpperLeftCorner().Y, 5, 5)), Color.White);
                    spriteBatch.Draw(WeaponTexture, camera.FromRectangle(new Rectangle((int)weapon.currentLocation.UpperRightCorner().X, (int)weapon.currentLocation.UpperRightCorner().Y, 5, 5)), Color.White);
                    spriteBatch.Draw(WeaponTexture, camera.FromRectangle(new Rectangle((int)weapon.currentLocation.LowerLeftCorner().X, (int)weapon.currentLocation.LowerLeftCorner().Y, 5, 5)), Color.White);
                    spriteBatch.Draw(WeaponTexture, camera.FromRectangle(new Rectangle((int)weapon.currentLocation.LowerRightCorner().X, (int)weapon.currentLocation.LowerRightCorner().Y, 5, 5)), Color.White);
                }
                else if (weapon.Weapon.Type.Name == "Arrow")
                {
                    float angle = (float)Geometry.Angle(Vector2.Zero, weapon.Movement.Velocity);
                    spriteBatch.Draw(WeaponTexture, camera.FromRectangle(weapon.Movement.Area), WeaponTexture.Bounds, Color.White, MathHelper.ToRadians(angle + 90f), new Vector2(weapon.Movement.Area.Width / 2, weapon.Movement.Area.Height / 2), SpriteEffects.None, 0);
                }
            }

        }

        public Weapon GetWeaponById(int id)
        {
            foreach (Weapon weapon in Weapons)
            {
                if (id == weapon.Id) { return weapon; }
            }
            return null;
        }

    }
}
