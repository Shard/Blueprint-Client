using System;
using System.Xml;
using System.Net;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Blueprint
{
    class Map
    {

        /// <summary>Array of all the blocks on the map</summary>
        public Block[,] Blocks;

        /// <summary>Array of all block types</summary>
        public BlockType[] Types;

        /// <summary>Array of all the walls on the map</summary>
        public Wall[,] Walls;

        /// <summary>Array of all wall types</summary>
        public WallType[] WallTypes;

        /// <summary>Contains Furniture</summary>
        public EntityPackage Furniture;

        /// <summary>Collection of all dropped items on the map</summary>
        public DroppedItemCollection DroppedItems;

        /// <summary>Collection of all the fluids on the map</summary>
        public Fluid.FluidCollection Fluids;

        public Texture2D BlockTexture;
        public Texture2D BlockState;

        /// <summary>Defines how wide the map is in terms of blocks</summary>
        public int SizeX;

        /// <summary>Defines how high the map is in terms of blocks</summary>
        public int SizeY;

        public Map(int width = 700, int height = 100)
        {

            // Init
            SizeX = width;
            SizeY = height;
            Types = new BlockType[50];
            WallTypes = new WallType[10];
            Blocks = new Block[SizeX, SizeY];
            Walls = new Wall[SizeX, SizeY];
            DroppedItems = new DroppedItemCollection();
            Fluids = new Fluid.FluidCollection();
            Furniture = new EntityPackage();

            WallTypes[0] = new WallType(1, "Standard Wall", new Rectangle(0,0,24,24));

        }

        public void Initialize( Texture2D blockTexture, Texture2D blockState, Texture2D furnitureTexture, Package package, Config config )
        {

            // Setup Liquids
            Fluids.Initialize(SizeX, SizeY, blockState);

            BlockTexture = blockTexture;
            BlockState = blockState;
            Furniture.Initialize(furnitureTexture);

            // Gather Map Data
            string data = package.RemoteString("maps/manifest/" + config.MapId);
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(data);


            // Parse Map Data into types and blocks
            foreach (XmlNode node in xml.DocumentElement.ChildNodes)
            {
                if (node.Name == "BlockType")
                {
                    int upto = 0;
                    foreach (XmlNode blocktype in node.ChildNodes)
                    {
                        Types[upto] = new BlockType(blocktype.Attributes["name"].Value, Int32.Parse(blocktype.Attributes["id"].Value), new Rectangle(Int32.Parse(blocktype.Attributes["x"].Value) * 24, Int32.Parse(blocktype.Attributes["y"].Value) * 24, 24, 24), 100);
                        upto++;
                    }
                }
                else if (node.Name == "Block")
                {
                    foreach (XmlNode block in node.ChildNodes)
                    {
                        Blocks[Int32.Parse(block.Attributes["x"].Value), Int32.Parse(block.Attributes["y"].Value)] = new Block(GetBlockType(Int32.Parse(block.Attributes["type"].Value)));
                    }
                }
            }

            // Autogenerate
            MapGenerator generator = new MapGenerator();
            generator.Setup(SizeX, SizeY);
            generator.Generate(this);
            
        }

        public void Update( Control control, Quickbar quickbar, Camera camera, Lighting lighting )
        {
            if (control.currentKeyboard.IsKeyDown(Keys.O) && control.previousKeyboard.IsKeyUp(Keys.O))
            {
                Fluids.Water.Blocks[30, 1] = 9;
                Fluids.Water.Blocks[29, 1] = 9;
                Fluids.Water.Blocks[26, 1] = 9;
                Fluids.Water.Blocks[30, 0] = 9;
                Fluids.Water.Blocks[25, 1] = 9;
            }

            Fluids.Update(this);
            Furniture.Update(control, camera);

            // Use Items

            if (quickbar.UsingItem.Type == "Block")
            {
                if (placeBlock(control.AtBlockX, control.AtBlockY, GetBlockType(quickbar.UsingItem.IntValue)))
                {
                    quickbar.useItem(quickbar.Selected);
                }
            }

            if (quickbar.UsingItem.Type == "BackgroundTile")
            {
                if (placeWall(control.AtBlockX, control.AtBlockY, GetWallType(quickbar.UsingItem.IntValue)))
                {
                    quickbar.useItem(quickbar.Selected);
                }
            }

            lighting.ClearShadows();

            for (int x = 0; x < 100; x++)
            {
                for (int y = 0; y < 100; y++)
                {
                    if (Blocks[x, y] != null)
                    {
                        lighting.AddShadow(camera.FromRectangle(new Rectangle(x * 24, y * 24,24,24)));
                    }
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch, Camera camera)
        {

            Fluids.Draw(spriteBatch, camera);
            Furniture.Draw(spriteBatch, camera);

            for (int x = 0; x < 100; x++)
            {
                for (int y = 0; y < 100; y++)
                {
                    if (Walls[x, y] != null)
                    {
                        Vector2 position = new Vector2(camera.X + x * 24, camera.Y + y * 24);
                        spriteBatch.Draw(BlockTexture, position, Walls[x, y].Type.Sprite, Color.White);
                    }
                    if (Blocks[x, y] != null) {
                        Vector2 position = new Vector2(camera.X + x * 24, camera.Y + y * 24);
                        spriteBatch.Draw(BlockTexture, position, Blocks[x, y].Type.Location, Color.White);
                        if (Blocks[x, y].Health < 25)
                        {
                            spriteBatch.Draw(BlockState, position, new Rectangle(48, 24, 24, 24), Color.White);
                        }
                        else if (Blocks[x, y].Health < 50)
                        {
                            spriteBatch.Draw(BlockState, position, new Rectangle(24, 24, 24, 24), Color.White);
                        }
                        else if (Blocks[x, y].Health < 75)
                        {
                            spriteBatch.Draw(BlockState, position, new Rectangle(0, 24, 24, 24), Color.White);
                        }
                    }
                }
            }

        }

        public void Pathfind(Vector2 from, Vector2 to)
        {

        }

        /// <summary>
        /// Gets a block type by its Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public BlockType GetBlockType(int id)
        {
            for (int i = 0; i < Types.Length; i++)
            {
                if (id == Types[i].Id) { return Types[i]; }
            }
            return null;
        }

        /// <summary>
        /// Gets a wall type by its Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public WallType GetWallType(int id)
        {
            for (int i = 0; i < WallTypes.Length; i++)
            {
                if (id == WallTypes[i].Id) { return WallTypes[i]; }
            }
            return null;
        }

        /// <summary>
        /// Checks to see if a block place at x,y would be out of bounds
        /// </summary>
        /// <param name="x">Coordianate X of the block location</param>
        /// <param name="y">Coordianate Y of the block location</param>
        /// <returns>Wether the location is within bounds of the map</returns>
        public bool inBounds(int x, int y)
        {
            if (x < 0 || y < 0 || x >= SizeX || y >= SizeY) { return false; } else { return true; } 
        }

        /// <summary>
        /// Safely gets a block from the map based of x/y
        /// </summary>
        /// <param name="x">Coordianate X of the block location</param>
        /// <param name="y">Coordianate Y of the block location</param>
        /// <returns>Returns a Block object if a block exists, otherwise returns null</returns>
        public Block getBlock(int x, int y)
        {
            if (inBounds(x, y)) { return Blocks[x, y]; } else { return null; }
        }

        public Wall getWall(int x, int y)
        {
            if (inBounds(x, y)) { return Walls[x, y]; } else { return null; }
        }

        /// <summary>
        /// Generates and places a new block at x/y
        /// </summary>
        /// <param name="x">Coordianate X of the block location</param>
        /// <param name="y">Coordianate X of the block location</param>
        /// <param name="type">Block Type</param>
        /// <returns>Returns true if the block placement was successfull, false if no block was placed</returns>
        public bool placeBlock(int x, int y, BlockType type)
        {
            if (getBlock(x, y) != null) { return false; }

            // Check for blocks to attach to
            if (getBlock(x - 1, y) == null && getBlock(x + 1, y) == null && getBlock(x, y - 1) == null && getBlock(x, y + 1) == null) {
                return false;
            }

            // Check range from player

            Blocks[x, y] = new Block(type);
            return true;
        }

        public bool placeWall(int x, int y, WallType type)
        {
            if (getWall(x, y) != null) { return false; }

            // Check for walls or block to attach to
            if (getWall(x - 1, y) == null && getWall(x + 1, y) == null && getWall(x, y - 1) == null && getWall(x, y + 1) == null && getBlock(x - 1, y) == null && getBlock(x + 1, y) == null && getBlock(x, y - 1) == null && getBlock(x, y + 1) == null)
            {
                return false;
            }

            // Check range from player

            Walls[x, y] = new Wall(type);
            return true;
        }

        /// <summary>
        /// Does damage to a block
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void mineBlock(int x, int y)
        {
            if (x < 0 || y < 0) { return; }

            if (Blocks[x, y] == null) { return; }

            Blocks[x, y].Health --;
            if (Blocks[x, y].Health < 1)
            {
                Blocks[x, y] = null;
            }
        }

    }
}
