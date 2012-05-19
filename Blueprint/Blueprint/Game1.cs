using System;
using System.IO;
using System.Net;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Lidgren.Network;
using Krypton;
using Krypton.Lights;

namespace Blueprint
{

    public class BlueprintGame : Microsoft.Xna.Framework.Game
    {

        Loading Loading;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteBatch altBatch;

        Player Player; // The play that is currently being played
        Map Map; // The current map the player is on
        ItemPackage ItemPackage; // A collection of every item avaliable
        Chat Chat; // Manages the chatting interface
        Config Config; // Handles argument parsing and other core functions key to making the game run
        Server Server; // If client is hosting, provides functions to host a server
        Client Client; //If client is joining, provides function to communicate with a server
        Package Package; 
        Control Control;
        WeaponPackage WeaponPackage;
        NpcPackage NpcPackage;
        Ui.Ui Ui;

        Camera Camera; // The camera

        SpriteFont font; // The main font used
        Texture2D Cursor; // Cursor Texture
        Texture2D Wallpaper;
        SoundEffect ShotgunSound;

        Lighting Lighting;

        float ElapsedTime;
        int ElapsedFrames;
        int Fps;

        public BlueprintGame(string[] args)
        {
            
            // Graphics Setup
            graphics = new GraphicsDeviceManager(this);
            this.graphics.PreferredBackBufferWidth = 1280;
            this.graphics.PreferredBackBufferHeight = 720;
            this.graphics.SynchronizeWithVerticalRetrace = true;
            Content.RootDirectory = "Content";
            this.Window.AllowUserResizing = true;
            this.Window.ClientSizeChanged += new EventHandler<EventArgs>(Window_ClientSizeChanged);

            // Setup a few classes
            Config = new Config(args);
            Package = new Package(Config);
            Lighting = new Lighting(this);
            Loading = new Loading();

        }

        /// <summary>Called on window resize</summary>
        void Window_ClientSizeChanged(object sender, EventArgs e)
        {
            Camera = new Camera(GraphicsDevice, Player);
        }

        protected override void Initialize()
        {

            Player = new Player();
            Map = new Map();
            Chat = new Chat();
            ItemPackage = new ItemPackage();
            WeaponPackage = new WeaponPackage();
            Control = new Control();
            NpcPackage = new NpcPackage();
            Ui = new Ui.Ui();
            Lighting.Initialize();

            base.Initialize();
        }


        public void registerGame()
        {
            string ip = Config.getIP();
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Config.Server + "games/register/" + Config.MapId + "/" + ip);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            NetPeerConfiguration config = new NetPeerConfiguration("host");
            config.Port = 8877;
            NetServer server = new NetServer(config);
            server.Start();

        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {

            // Loading Screen
            font = Content.Load<SpriteFont>("Misc/system");

            // Load Remote Texutres
            Texture2D mapTexture = Package.RemoteTexture("/maps/sprite/" + Config.MapId, GraphicsDevice);

            ShotgunSound = Content.Load<SoundEffect>("Sounds/shotgun");

            // Load Local Textures
            Cursor = Content.Load<Texture2D>("Ui/cursor");

            // Initialize
            Control.Initialize(Content.Load<Texture2D>("Ui/cursor"));
            Wallpaper = Content.Load<Texture2D>("Wallpaper/wallpaper");
            Map.Initialize(mapTexture, Package, Config, GraphicsDevice, Content);
            Player.Initialize(Package.LocalTexture("C:\\blueprint\\player.png", GraphicsDevice), Content.Load<Texture2D>("Ui/bars"), Package, Map.Spawn);
            Camera = new Camera(GraphicsDevice, Player);
            WeaponPackage.Initialize(Content.Load<Texture2D>("Weapons/sword"),Content.Load<Texture2D>("Weapons/arrow"));
            ItemPackage.mock(Map.Types, WeaponPackage.Weapons);
            ItemPackage.Initialize(Content.Load<Texture2D>("items"));
            Player.Inventory.Initialize(Content.Load<Texture2D>("Ui/buttons"), ItemPackage);
            NpcPackage.Initialize(Content.Load<Texture2D>("npcs"), Content.Load<Texture2D>("Ui/ui"));
            Chat.Initialize(Content.Load<Texture2D>("Ui/chat"));
            Lighting.LoadContent(GraphicsDevice);

            spriteBatch = new SpriteBatch(GraphicsDevice);
            altBatch = new SpriteBatch(GraphicsDevice);

            if (Config.Hosting) { Server = new Server(Config); Server.Initialize(Content.Load<Texture2D>("player"), Content.Load<Texture2D>("Ui/bars")); } else { Server = null; }
            if (Config.Join != null) { Client = new Client(Config); Client.Initialize(Content.Load<Texture2D>("player"), Content.Load<Texture2D>("Ui/bars")); } else { Client = null; }

        }

        protected override void UnloadContent()
        {
            
        }

        protected override void Update(GameTime gameTime)
        {
            ElapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
            ElapsedFrames++;

            if (ElapsedTime > 1)
            {
                Fps = ElapsedFrames;
                ElapsedFrames = 0;
                ElapsedTime = 0;
            }

            if (Loading.IsLoading)
            {
                Loading.Update(gameTime);
                base.Update(gameTime);
                return;
            }


            // Update Input States
            Control.Update(Keyboard.GetState(), Mouse.GetState(), Camera);
            
            #region Game functions

            if (Control.Pressed(Keys.F11))
            {
                if (!graphics.IsFullScreen)
                {
                    graphics.PreferredBackBufferWidth = 1920;
                    graphics.PreferredBackBufferHeight = 1080;
                }
                else 
                {
                    graphics.PreferredBackBufferWidth = 1280;
                    graphics.PreferredBackBufferHeight = 720;
                }
                graphics.ToggleFullScreen();
                graphics.ApplyChanges();
                
            }

            #endregion

            // Update Modules
            if (Server != null) { Server.Update(Control, Map, Player, gameTime, Package); }
            if (Client != null) { Client.Update(Control, Map, Player, gameTime, Package); }

            // Update Player
            Player.Inventory.Update(Camera, Control);
            if(!Control.IsLocked){
                Player.Movement.HandleControls(Control);
            }
            Player.Update(Control,  Map);
            Camera.Update(Player.Movement.Moved);

            // Updates Players
            

            Map.Update(Control, Player.Inventory.Quickbar, Camera, Lighting, Player);
            Map.DroppedItems.Update(Map, Player);
            NpcPackage.Update(Map, Player, Control, Camera, font);
            WeaponPackage.Update(Control, Camera, Player, NpcPackage, Ui.FloatingTexts, ref Map);
            Ui.Update();

            // Update Chat
            if (Server != null) { Chat.Add(Server.Messages); }
            if (Client != null) { Chat.Add(Client.Messages, gameTime); }
            Chat.Update(ref Control, gameTime);

            Lighting.Update(gameTime, Control, Camera, Map.Entities);

            // Mine blocks
            if (Control.currentMouse.LeftButton == ButtonState.Pressed)
            {
                //Map.mineBlock((Control.currentMouse.X - (int)Camera.X) / 24, (Control.currentMouse.Y - (int)Camera.Y) / 24);
            }

            // Drop Items
            if (Control.currentMouse.MiddleButton == ButtonState.Pressed && Control.previousMouse.MiddleButton == ButtonState.Released)
            {
                Map.DroppedItems.Add(new Vector2(Control.currentMouse.X - Camera.X, Control.currentMouse.Y - Camera.Y), new Item(ItemPackage.ItemTypes[0], 1, false));
            }

            if (Control.currentMouse.RightButton == ButtonState.Pressed && !Player.Movement.Falling)
            {
                //Player.Movement.PushbackFrom(new Vector2(Control.currentMouse.X - Camera.X, Control.currentMouse.Y - Camera.Y), 10f);
                //ShotgunSound.Play();
            }

            // Test
            if (Control.currentKeyboard.IsKeyDown(Keys.P) && Control.previousKeyboard.IsKeyUp(Keys.P))
            {
                // Pathfind
                Point npc = new Point(NpcPackage.ActiveNpcs[0].Movement.Area.X / 24, NpcPackage.ActiveNpcs[0].Movement.Area.Y / 24);
                NpcPackage.ActiveNpcs[0].CurrentPath = Player.Movement.Pathfind(npc, new Point(Control.AtTileX, Control.AtTileY), Map);
                NpcPackage.ActiveNpcs[0].CurrentDestination = Point.Zero;
            }


            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            // Loading
            if (Loading.IsLoading)
            {
                Loading.Draw(spriteBatch, GraphicsDevice, font);
                base.Draw(gameTime);
                return;
            }

            // Setup
            Lighting.Draw(GraphicsDevice);
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();
            
            // Before Shadows
            spriteBatch.Draw(Wallpaper, GraphicsDevice.Viewport.TitleSafeArea, GraphicsDevice.Viewport.TitleSafeArea, Color.White);
            Map.DroppedItems.Draw(spriteBatch, Camera, ItemPackage);
            WeaponPackage.Draw(spriteBatch, Camera);
            NpcPackage.Draw(spriteBatch, Camera, font);
            Map.Draw(spriteBatch, Camera);
            Player.Draw(spriteBatch, Camera);
            Map.Fluids.Draw(spriteBatch, Camera);
            // Shadows
            spriteBatch.End();
            Lighting.PostDraw(gameTime, GraphicsDevice);
            altBatch.Begin();

            // After Shadows
            Player.DrawUi(altBatch, font);
            Player.Inventory.Draw(altBatch, Control, font, ItemPackage);
            Chat.Draw(altBatch, font, gameTime);
            if (Server != null) { Server.Draw(altBatch, Camera); }
            if (Client != null) { Client.Draw(altBatch, Camera); }
            Ui.Draw(altBatch, Camera, font);
            NpcPackage.Interaction.Draw(altBatch, font, Camera, NpcPackage.UiTexture, NpcPackage.NpcTexture);
            Control.Draw(altBatch);

            #region debug
            if (NpcPackage.ActiveNpcs[0].CurrentPath != null)
            {
                foreach (Point point in NpcPackage.ActiveNpcs[0].CurrentPath)
                {
                    altBatch.Draw(Wallpaper, Camera.FromRectangle(new Rectangle(point.X * 24, point.Y * 24, 24, 24)), Color.Green);
                }
                altBatch.Draw(Wallpaper, Camera.FromRectangle(new Rectangle(NpcPackage.ActiveNpcs[0].CurrentDestination.X * 24,  NpcPackage.ActiveNpcs[0].CurrentDestination.Y * 24, 24,24)), Color.Red);
            }
            #endregion

            #region Previews

            // preview
            if (Map.Entities.Preview != null)
            {
                EntityType type = Map.Entities.getType(Map.Entities.Preview.Type);
                Color preview_color = new Color((byte)200, (byte)200, (byte)200, (byte)100);
                if (type.Function == EntityType.EntityFunction.Door)
                {
                    if(Map.Entities.Preview.Solid)
                        altBatch.Draw(type.Sprite, Camera.FromRectangle(Map.Entities.Preview.Area), new Rectangle(0,0, type.Sprite.Width / 2, type.Sprite.Height), preview_color);
                    else
                        altBatch.Draw(type.Sprite, Camera.FromRectangle(Map.Entities.Preview.Area), new Color((byte)100, (byte)100, (byte)100, (byte)100));
                }
                else
                {

                }
            }

            #endregion

            TextHelper.DrawString(altBatch, font, Fps.ToString(), new Vector2(20, 20), Color.White);

            altBatch.End();

            
            base.Draw(gameTime);
        }
    }
}
