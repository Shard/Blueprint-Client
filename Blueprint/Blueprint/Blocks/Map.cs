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

        public Block[,] Blocks;
        public BlockType[] Types;
        public Texture2D BlockTexture;
        public Texture2D BlockState;
        public DroppedItemCollection DroppedItems;
        public Fluid.FluidCollection Fluids;

        public int SizeX;
        public int SizeY;

        public Map(Config config)
        {

            // Init
            SizeX = 700;
            SizeY = 100;
            Types = new BlockType[50];
            Blocks = new Block[SizeX, SizeY];
            DroppedItems = new DroppedItemCollection();
            Fluids = new Fluid.FluidCollection();

        }

        public void Initialize( Texture2D blockTexture, Texture2D blockState, Package package, Config config )
        {

            // Setup Liquids
            Fluids.Initialize(SizeX, SizeY);

            BlockTexture = blockTexture;
            BlockState = blockState;

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
                        Types[upto] = new BlockType(blocktype.Attributes["name"].Value, Int32.Parse(blocktype.Attributes["id"].Value), new Rectangle(Int32.Parse(blocktype.Attributes["x"].Value) * 32, Int32.Parse(blocktype.Attributes["y"].Value) * 32, 32, 32), 100);
                        upto++;
                    }
                }
                else if (node.Name == "Block")
                {
                    foreach (XmlNode block in node.ChildNodes)
                    {
                        Blocks[Int32.Parse(block.Attributes["x"].Value), Int32.Parse(block.Attributes["y"].Value)] = new Block(findBlockType(Int32.Parse(block.Attributes["type"].Value)));
                    }
                }
            }

        }

        public void Update( Control control, Quickbar quickbar )
        {

            if (quickbar.AttemptPlace != null)
            {
                if (placeBlock(control.AtBlockX, control.AtBlockY, quickbar.AttemptPlace))
                {
                    quickbar.useItem(quickbar.Selected);
                }
            }

        }

        public void Draw(SpriteBatch spriteBatch, Camera camera)
        {

            for (int x = 0; x < 100; x++)
            {
                for (int y = 0; y < 100; y++)
                {
                    if (Blocks[x, y] == null) { continue; }
                    Vector2 position = new Vector2(camera.X + x * 32, camera.Y + y * 32);
                    spriteBatch.Draw(BlockTexture, position, Blocks[x, y].Type.Location, Color.White);
                    if (Blocks[x, y].Health < 25)
                    {
                        spriteBatch.Draw(BlockState, position, new Rectangle(64, 32, 32, 32), Color.White);
                    }
                    else if (Blocks[x, y].Health < 50)
                    {
                        spriteBatch.Draw(BlockState, position, new Rectangle(32, 32, 32, 32), Color.White);
                    }
                    else if (Blocks[x, y].Health < 75)
                    {
                        spriteBatch.Draw(BlockState, position, new Rectangle(0, 32, 32, 32), Color.White);
                    }
                }
            }

        }

        public BlockType findBlockType(int id)
        {
            for (int i = 0; i < Types.Length; i++)
            {
                if (id == Types[i].Id) { return Types[i]; }
            }
            return null;
        }

        public bool inBounds(int x, int y)
        {
            if (x < 0 || y < 0 || x >= SizeX || y >= SizeY) { return false; } else { return true; } 
        }

        public Block getBlock(int x, int y)
        {
            if (inBounds(x, y)) { return Blocks[x, y]; } else { return null; }
        }

        public bool placeBlock(int x, int y, BlockType type)
        {
            if (!inBounds(x, y) || getBlock(x, y) != null) { return false; }

            // Check for blocks to attach to
            if (getBlock(x - 1, y) == null && getBlock(x + 1, y) == null && getBlock(x, y - 1) == null && getBlock(x, y + 1) == null) {
                return false;
            }

            // Check range from player

            Blocks[x, y] = new Block(type);
            return true;
        }

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
