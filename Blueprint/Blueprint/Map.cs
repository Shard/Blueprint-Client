using System;
using System.Xml;
using System.Net;
using System.IO;
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
        public DroppedItem[] DroppedItems;

        public Map()
        {

            Types = new BlockType[50];
            Types[0] = new BlockType("Grass", new Rectangle(0, 0, 32, 32),100);
            Types[1] = new BlockType("Dirt", new Rectangle(32, 0, 32, 32), 100);
            Types[2] = new BlockType("Stone", new Rectangle(64, 0, 32, 32), 100);
            Types[3] = new BlockType("Rock", new Rectangle(96, 0, 32, 32), 100);
            Types[4] = new BlockType("Copper", new Rectangle(128, 0, 32, 32), 100);

            Blocks = new Block[100, 100];

            // Build Map
            for (int x = 0; x < 10; x++)
            {
                for (int y = 0; y < 10; y++)
                {
                    Blocks[x, y] = new Block(Types[0]);
                }
            }

            DroppedItems = new DroppedItem[255];

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://local.blueprintgame.com:8888/maps/manifest/3");
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            StreamReader reader = new StreamReader(response.GetResponseStream());
            string data = reader.ReadToEnd();
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(data);
            foreach (XmlNode node in xml.DocumentElement.ChildNodes)
            {
                Blocks[Int32.Parse(node.Attributes["x"].Value), Int32.Parse(node.Attributes["y"].Value)] = new Block(Types[1]);
            }


        }

        public void Initialize( Texture2D blockTexture )
        {
            BlockTexture = blockTexture;
        }

        public void Update( MouseState currentMouseState, MouseState previousMouseState )
        {

        }

        public void Draw(SpriteBatch spriteBatch, Vector2 camera)
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
                        spriteBatch.Draw(BlockTexture, position, new Rectangle(64, 32, 32, 32), Color.White);
                    }
                    else if (Blocks[x, y].Health < 50)
                    {
                        spriteBatch.Draw(BlockTexture, position, new Rectangle(32, 32, 32, 32), Color.White);
                    }
                    else if (Blocks[x, y].Health < 75)
                    {
                        spriteBatch.Draw(BlockTexture, position, new Rectangle(0, 32, 32, 32), Color.White);
                    }
                }
            }

        }

        public void spawnItem(Vector2 location, Item item)
        {
            DroppedItems[0] = new DroppedItem(location, item);
        }

        public void placeBlock(int x, int y)
        {
            this._placeBlock(x,y,Types[0]);
        }
        
        public void _placeBlock(int x, int y, BlockType type)
        {

            if (x < 0 || y < 0) { return; }

            Blocks[x, y] = new Block(Types[0]);

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
