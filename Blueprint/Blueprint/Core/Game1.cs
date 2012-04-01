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

        Camera Camera; // The camera

        SpriteFont font; // The main font used
        Texture2D Cursor; // Cursor Texture
        Texture2D Wallpaper;

        public Game1(string[] args)
        {

            // Setup
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            this.Window.AllowUserResizing = true;
            this.Window.ClientSizeChanged += new EventHandler<EventArgs>(Window_ClientSizeChanged);

            // Parse url arguments and do registery stuff
            Config = new Config(args);
            Config.SetupRegistery();

        }

        void Window_ClientSizeChanged(object sender, EventArgs e)
        {
            Camera = new Camera(GraphicsDevice);
        }

        protected override void Initialize()
        {

            if (Config.Hosting) { Server = new Server(Config); Server.Initialize(); } else { Server = null; }
            if (Config.Join != null) { Client = new Client(Config); Client.Initialize(); } else { Client = null; }

            Player = new Player();
            Camera = new Camera(GraphicsDevice);
            Map = new Map(Config.MapId);
            Chat = new Chat();
            Package = new Package();
            ItemPackage = new ItemPackage();
            WeaponPackage = new WeaponPackage();
            ItemPackage.mock(Map.Types, WeaponPackage.Weapons);
            Control = new Control();
            NpcPackage = new NpcPackage();

            base.Initialize();
        }


        public void registerGame()
        {
            string ip = Config.getIP();
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://local.blueprintgame.com:8888/games/register/" + Config.MapId + "/" + ip);
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

            // Load Remote Texutres
            Texture2D mapTexture = Package.RemoteTexture("/maps/sprite/" + Config.MapId, GraphicsDevice);

            // Load Local Textures
            Cursor = Content.Load<Texture2D>("cursor");
            font = Content.Load<SpriteFont>("system");

            // Initialize
            Wallpaper = Content.Load<Texture2D>("wallpaper");
            Map.Initialize(mapTexture, Content.Load<Texture2D>("blocks"));
            Player.Initialize(Content.Load<Texture2D>("player"), new Vector2(100, -100));
            ItemPackage.Initialize(Content.Load<Texture2D>("items"));
            Player.Inventory.Initialize(Content.Load<Texture2D>("inventory"), ItemPackage);
            WeaponPackage.Initialize(Content.Load<Texture2D>("weapons"));
            NpcPackage.Initialize(Content.Load<Texture2D>("npcs"));

            spriteBatch = new SpriteBatch(GraphicsDevice);

        }

        protected override void UnloadContent()
        {
            
        }

        protected override void Update(GameTime gameTime)
        {
            // Update Input States
            Control.Update(Keyboard.GetState(), Mouse.GetState(), Camera);

            // Update Modules
            if (Server != null) { Server.Update(gameTime); }
            Player.Inventory.Update(Control);
            Player.Update(Control,  Map, Chat);
            Map.Update(Control, Player.Inventory.Quickbar);
            WeaponPackage.Update(Control, Camera, Player);

            // Update Chat
            if (Server != null) { Chat.Add(Server.Messages); }
            if (Client != null) { Chat.Add(Client.Messages, gameTime); }
            Chat.Update();

            // Update Camera
            Camera.Update(Player.Movement.Moved);

            // Mine blocks
            if (Control.currentMouse.LeftButton == ButtonState.Pressed)
            {
                Map.mineBlock((Control.currentMouse.X - (int)Camera.X) / 32, (Control.currentMouse.Y - (int)Camera.Y) / 32);
            }

            // Drop Items
            if (Control.currentMouse.MiddleButton == ButtonState.Pressed && Control.previousMouse.MiddleButton == ButtonState.Released)
            {
                Map.spawnItem(new Vector2(Control.currentMouse.X - Camera.X, Control.currentMouse.Y - Camera.Y), new Item(ItemPackage.ItemTypes[0], 1, false));
            }

            foreach (DroppedItem item in Map.DroppedItems)
            {
                if (item == null || item.Falling == false) { continue; }
                item.Location.Y++;
                // Collect Intersections and calculate some numbers
                for (int x = 0; x < 100; x++)
                {
                    for (int y = 0; y < 100; y++)
                    {
                        if (Map.Blocks[x, y] == null) { continue; }
                        Rectangle blockArea = new Rectangle(x * 32, y * 32, 32, 32);
                        Rectangle intersection = Rectangle.Intersect(new Rectangle((int)item.Location.X,(int)item.Location.Y,15,15), blockArea);

                        if (intersection != Rectangle.Empty)
                        {
                            item.Falling = false;
                        }
                    }
                }
            }


            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
         
            spriteBatch.Begin();
            spriteBatch.Draw(Wallpaper, GraphicsDevice.Viewport.TitleSafeArea, Color.White);
            Map.Draw(spriteBatch, Camera);
            Player.Draw(spriteBatch, Camera);
            Player.Inventory.Draw(spriteBatch, Control, font, ItemPackage);
            WeaponPackage.Draw(spriteBatch);

            foreach (DroppedItem item in Map.DroppedItems)
            {
                if (item == null) { continue; }
                spriteBatch.Draw(ItemPackage.ItemTexture, new Rectangle( (int)(item.Location.X + Camera.X), (int)(item.Location.Y + Camera.Y), 15, 15), item.Item.Type.Location, Color.White);
            }

            Chat.Draw(spriteBatch, font, gameTime);

            spriteBatch.Draw(Cursor, new Rectangle(Control.currentMouse.X, Control.currentMouse.Y, 20, 20), new Rectangle(0, 0, 32, 32), Color.White);


            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
