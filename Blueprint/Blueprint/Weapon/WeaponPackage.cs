using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Blueprint
{
    class WeaponPackage
    {

        public Weapon[] Weapons;
        public WeaponType[] Types;
        public List<LiveWeapon> LiveWeapons;
        public Texture2D WeaponTexture;

        public WeaponPackage()
        {
            
            // Types
            Types = new WeaponType[10];
            Types[0] = new WeaponType("Sword", true);
            Types[1] = new WeaponType("Axe", true);
            Types[2] = new WeaponType("Bow", true);

            // Weapons
            Weapons = new Weapon[10];
            

            // Live Weapons
            LiveWeapons = new List<LiveWeapon>();

        }

        public void Initialize(Texture2D weaponTexture)
        {
            WeaponTexture = weaponTexture;
            Weapons[0] = new Weapon(Types[0], "Big Stick", new Rectangle(0, 0, WeaponTexture.Bounds.Width, WeaponTexture.Bounds.Height));
        }

        public void Update(Control control, Camera camera, Player player, NpcPackage npcs, Ui.FloatingTextCollection floatingText)
        {

            // Initialize Weapons
            if (player.Inventory.Quickbar.UsingWeapon != null)
            {
                if (player.Inventory.Quickbar.UsingWeapon.Type.Name == "Sword" && !player.Inventory.Quickbar.SwingingWeapon)
                {
                    LiveWeapons.Add(new LiveWeapon(player.Inventory.Quickbar.UsingWeapon, startingSwordPosition(player, new Rectangle(0,0,47,79)), player.Movement.Direction));
                    player.Inventory.Quickbar.SwingingWeapon = true;
                }
            }

            // Animate weapons
            for( int i = 0; i < LiveWeapons.Count; i++)
            {
                if (LiveWeapons[i].Weapon.Type.Name == "Sword")
                {
                    LiveWeapons[i].Rotation += LiveWeapons[i].Speed;
                    if ((LiveWeapons[i].SwingEnd > 0 && LiveWeapons[i].Rotation > LiveWeapons[i].SwingEnd) || (LiveWeapons[i].SwingEnd < 0 && LiveWeapons[i].Rotation < LiveWeapons[i].SwingEnd))
                    {
                        LiveWeapons.RemoveAt(i);
                        player.Inventory.Quickbar.SwingingWeapon = false;
                    }
                }
            }

            // Check for weapon collisions
            foreach (LiveWeapon weapon in LiveWeapons)
            {
                for (int i = 0; i < npcs.ActiveNpcs.Count; i++)
                {
                    if (weapon.currentLocation.Intersects(npcs.ActiveNpcs[i].Movement.Area))
                    {
                        if (npcs.Damage(npcs.ActiveNpcs[i], 10, weapon.Location))
                        {
                            floatingText.Add("10", weapon.Location);
                        }
                    }
                }
            }



        }

        public Vector2 startingSwordPosition( Player player, Rectangle weaponArea )
        {
            if (player.Movement.Direction == "right")
            {
                return new Vector2(player.Movement.Area.Right, player.Movement.Area.Center.Y);
            } else {
                return new Vector2(player.Movement.Area.Left, player.Movement.Area.Center.Y);
            }

        }

        public void Draw(SpriteBatch spriteBatch, Camera camera)
        {

            foreach (LiveWeapon weapon in LiveWeapons)
            {
                if (weapon.Weapon.Type.Name == "Sword")
                {
                    spriteBatch.Draw(WeaponTexture, camera.FromRectangle(new Rectangle((int)weapon.Location.X, (int)weapon.Location.Y, 47, 79)), new Rectangle(0,0,47,79), Color.White, weapon.Rotation, new Vector2(23.5f,79), SpriteEffects.None, 0 );
                }
            }

        }

    }
}
