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

namespace Blueprint
{

    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

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

        public Game1(string[] args)
        {
            
            // Graphics Setup
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            this.Window.AllowUserResizing = true;
            this.Window.ClientSizeChanged += new EventHandler<EventArgs>(Window_ClientSizeChanged);

            // Parse url arguments and do registery stuff
            Config = new Config(args);
            Package = new Package(Config);


        }

        void Window_ClientSizeChanged(object sender, EventArgs e)
        {
            Camera = new Camera(GraphicsDevice, Player);
        }

        protected override void Initialize()
        {

            Player = new Player(new Vector2(100, -100) );
            Map = new Map();
            Chat = new Chat();
            ItemPackage = new ItemPackage();
            WeaponPackage = new WeaponPackage();
            Control = new Control();
            NpcPackage = new NpcPackage();
            Camera = new Camera(GraphicsDevice, Player);
            Ui = new Ui.Ui();

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
            font = Content.Load<SpriteFont>("system");

            // Load Remote Texutres
            Texture2D mapTexture = Package.RemoteTexture("/maps/sprite/" + Config.MapId, GraphicsDevice);

            ShotgunSound = Content.Load<SoundEffect>("shotgun");

            // Load Local Textures
            Cursor = Content.Load<Texture2D>("cursor");

            // Initialize
            Wallpaper = Content.Load<Texture2D>("wallpaper");
            Map.Initialize(mapTexture, Content.Load<Texture2D>("blocks"), Package, Config);
            Player.Initialize(Content.Load<Texture2D>("player"));
            WeaponPackage.Initialize(Content.Load<Texture2D>("weapons"));
            ItemPackage.mock(Map.Types, WeaponPackage.Weapons);
            ItemPackage.Initialize(Content.Load<Texture2D>("items"));
            Player.Inventory.Initialize(Content.Load<Texture2D>("inventory"), ItemPackage);
            NpcPackage.Initialize(Content.Load<Texture2D>("npcs"));

            spriteBatch = new SpriteBatch(GraphicsDevice);

            if (Config.Hosting) { Server = new Server(Config); Server.Initialize(Content.Load<Texture2D>("player")); } else { Server = null; }
            if (Config.Join != null) { Client = new Client(Config); Client.Initialize(Content.Load<Texture2D>("player")); } else { Client = null; }

        }

        protected override void UnloadContent()
        {
            
        }

        protected override void Update(GameTime gameTime)
        {
            // Update Input States
            Control.Update(Keyboard.GetState(), Mouse.GetState(), Camera);

            // Update Modules
            if (Server != null) { Server.Update(Control, Map, Player, gameTime); }
            if (Client != null) { Client.Update(Control, Map, Player, gameTime); }

            // Update Player
            Player.Inventory.Update(Control);
            Player.Movement.HandleControls(Control);
            Player.Update(Control,  Map);

            // Updates Players
            

            Map.Update(Control, Player.Inventory.Quickbar);
            Map.DroppedItems.Update(Map, Player);
            NpcPackage.Update(Map);
            WeaponPackage.Update(Control, Camera, Player, NpcPackage, Ui.FloatingTexts);
            Ui.Update();

            // Update Chat
            if (Server != null) { Chat.Add(Server.Messages); }
            if (Client != null) { Chat.Add(Client.Messages, gameTime); }
            Chat.Update();

            // Update Camera
            Camera.Update(Player.Movement.Moved);

            // Mine blocks
            if (Control.currentMouse.LeftButton == ButtonState.Pressed)
            {
                Map.mineBlock((Control.currentMouse.X - (int)Camera.X) / 24, (Control.currentMouse.Y - (int)Camera.Y) / 24);
            }

            // Drop Items
            if (Control.currentMouse.MiddleButton == ButtonState.Pressed && Control.previousMouse.MiddleButton == ButtonState.Released)
            {
                Map.DroppedItems.Add(new Vector2(Control.currentMouse.X - Camera.X, Control.currentMouse.Y - Camera.Y), new Item(ItemPackage.ItemTypes[0], 1, false));
            }

            if (Control.currentMouse.RightButton == ButtonState.Pressed && !Player.Movement.Falling)
            {
                Player.Movement.PushbackFrom(new Vector2(Control.currentMouse.X - Camera.X, Control.currentMouse.Y - Camera.Y), 10f);
                ShotgunSound.Play();
            }


            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            
            spriteBatch.Begin();
            spriteBatch.Draw(Wallpaper, GraphicsDevice.Viewport.TitleSafeArea, Color.White);
            Map.Draw(spriteBatch, Camera);
            Map.DroppedItems.Draw(spriteBatch, Camera, ItemPackage);
            Player.Inventory.Draw(spriteBatch, Control, font, ItemPackage);
            WeaponPackage.Draw(spriteBatch, Camera);
            NpcPackage.Draw(spriteBatch, Camera);

            Chat.Draw(spriteBatch, font, gameTime);

            Player.Draw(spriteBatch, Camera);
            if (Server != null) { Server.Draw(spriteBatch, Camera); }
            if (Client != null) { Client.Draw(spriteBatch, Camera); }
            Ui.Draw(spriteBatch, Camera, font);
            spriteBatch.Draw(Cursor, new Rectangle(Control.currentMouse.X, Control.currentMouse.Y, 20, 20), new Rectangle(0, 0, 24, 24), Color.White);

            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
