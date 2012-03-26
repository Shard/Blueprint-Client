using System;
using System.IO;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Win32;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Lidgren.Network;

namespace Blueprint
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        string console;
        int MapId;

        KeyboardState currentKeyboardState;
        KeyboardState previousKeyboardState;
        MouseState previousMouseState;
        MouseState currentMouseState;

        Player Player; // The play that is currently being played
        Map Map; // The current map the player is on
        ItemCollection ItemCollection; // A collection of every item avaliable
        Chat Chat;

        Vector2 camera; // The camera position

        SpriteFont font; // The main font used
        Texture2D Cursor; // Cursor Texture

        public Game1(string[] args)
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            this.Window.AllowUserResizing = true;

            if (args.Length > 0)
            {
                console = args[0];
                // Handle Paramaters
                string arg = args[0];
                arg = arg.Replace("blueprint://", "");
                arg = arg.Replace("/", "");
                MapId = Convert.ToInt16(arg);
            }
            else
            {
                console = "No Paramaters";
                MapId = 3;
            }

            // Registery
            string gameLocation = System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase;
            string fullLocation = System.IO.Path.GetDirectoryName(gameLocation);
            fullLocation = "\"" + fullLocation.Replace("file:\\","") + "\\blueprint.exe\" \"%1\"";

            RegistryKey reg = Registry.ClassesRoot.OpenSubKey("blueprint");
            if (reg == null)
            {
                // Need to initialize Registery
                RegistryKey blueprint = Registry.ClassesRoot.CreateSubKey("blueprint");
                Registry.ClassesRoot.OpenSubKey("blueprint", true)
                    .SetValue("", "URL: Blueprint Protocol", RegistryValueKind.String);
                Registry.ClassesRoot.OpenSubKey("blueprint", true)
                    .SetValue("URL Protocol", "");

                Registry.ClassesRoot.CreateSubKey("blueprint\\shell");
                Registry.ClassesRoot.CreateSubKey("blueprint\\shell\\open");
                RegistryKey commandKey = Registry.ClassesRoot.CreateSubKey("blueprint\\shell\\open\\command");

                commandKey.SetValue("", fullLocation, RegistryValueKind.String);
            }
            else
            {
                //RegistryKey commandKey = Registry.ClassesRoot.CreateSubKey("blueprint\\shell\\open\\command");
                //commandKey.SetValue("", fullLocation, RegistryValueKind.String);
                // Make sure the path is ok, Send the server the update command if it needs updating
            }

            this.Window.ClientSizeChanged += new EventHandler<EventArgs>(Window_ClientSizeChanged);

        }

        void Window_ClientSizeChanged(object sender, EventArgs e)
        {
            camera = new Vector2((GraphicsDevice.Viewport.TitleSafeArea.X + GraphicsDevice.Viewport.TitleSafeArea.Width / 2) - Player.Position.X, (GraphicsDevice.Viewport.TitleSafeArea.Y + GraphicsDevice.Viewport.TitleSafeArea.Height / 2) - Player.Position.Y);
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {

            Player = new Player();
            camera = new Vector2((GraphicsDevice.Viewport.TitleSafeArea.X + GraphicsDevice.Viewport.TitleSafeArea.Width / 2) - 100, (GraphicsDevice.Viewport.TitleSafeArea.Y + GraphicsDevice.Viewport.TitleSafeArea.Height / 2) + 100);
            Map = new Map(MapId);
            Chat = new Chat();

            ItemCollection = new ItemCollection();
            ItemCollection.mock();

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {

            //StreamReader blocksReader = new StreamReader("C:\\Users\\Mark\\Desktop\\blocks.png");
            //Stream blocks = blocksReader.BaseStream;
            //Texture2D blockTexture = Texture2D.FromStream(GraphicsDevice, blocks);
            Cursor = Content.Load<Texture2D>("cursor");
            font = Content.Load<SpriteFont>("system");

            Map.Initialize(Content.Load<Texture2D>("blocks"));
            Player.Initialize(Content.Load<Texture2D>("player"), new Vector2(100, -100));
            ItemCollection.Initialize(Content.Load<Texture2D>("items"));
            Player.Inventory.Initialize(Content.Load<Texture2D>("inventory"), ItemCollection);

            spriteBatch = new SpriteBatch(GraphicsDevice);

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            

            previousKeyboardState = currentKeyboardState;
            currentKeyboardState = Keyboard.GetState();
            previousMouseState = currentMouseState;
            currentMouseState = Mouse.GetState();

            Player.Update(currentKeyboardState, previousKeyboardState,  Map);
            Player.Inventory.Update(currentKeyboardState, previousKeyboardState, currentMouseState, previousMouseState);
            Map.Update(currentMouseState, previousMouseState);
            camera -= Player.LastMovement;

            // Place blocks
            if (currentMouseState.RightButton == ButtonState.Pressed)
            {
                Map.placeBlock((currentMouseState.X - (int)camera.X) / 32, (currentMouseState.Y - (int)camera.Y) /32);
            }

            // Place blocks
            if (currentMouseState.LeftButton == ButtonState.Pressed)
            {
                Map.mineBlock((currentMouseState.X - (int)camera.X) / 32, (currentMouseState.Y - (int)camera.Y) / 32);
            }

            // Drop Items
            if (currentMouseState.MiddleButton == ButtonState.Pressed && previousMouseState.MiddleButton == ButtonState.Released)
            {
                Map.spawnItem(new Vector2(currentMouseState.X - camera.X, currentMouseState.Y - camera.Y), new Item(ItemCollection.ItemTypes[0],1,false));
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

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
         
            spriteBatch.Begin();
            Map.Draw(spriteBatch, camera);
            Player.Draw(spriteBatch, camera);
            Player.Inventory.Draw(spriteBatch, currentMouseState, font, ItemCollection);

            foreach (DroppedItem item in Map.DroppedItems)
            {
                if (item == null) { continue; }
                spriteBatch.Draw(ItemCollection.ItemTexture, new Rectangle( (int)(item.Location.X + camera.X), (int)(item.Location.Y + camera.Y), 15, 15), item.Item.Type.Location, Color.White);
            }
                
            spriteBatch.Draw(Cursor, new Rectangle(currentMouseState.X, currentMouseState.Y, 20, 20), new Rectangle(0,0,32,32), Color.White);

            spriteBatch.DrawString(font, console, new Vector2(10, 10), Color.Wheat);

            

            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
