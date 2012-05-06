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
        public EntityPackage Entities;

        /// <summary>Collection of all dropped items on the map</summary>
        public DroppedItemCollection DroppedItems;

        /// <summary>Collection of all the fluids on the map</summary>
        public Fluid.FluidCollection Fluids;

        public Texture2D BlockTexture;
        public Texture2D BlockState;
        public Texture2D WallTexture;

        /// <summary>Defines how wide the map is in terms of blocks</summary>
        public int SizeX;

        /// <summary>Defines how high the map is in terms of blocks</summary>
        public int SizeY;

        /// <summary>
        /// Spawn Location
        /// </summary>
        public Vector2 Spawn;

        public Map(int width = 1000, int height = 500)
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
            Entities = new EntityPackage();

            WallTypes[0] = new WallType(1, "Standard Wall", new Rectangle(0,0,24,24));

        }

        public void Initialize( Texture2D blockTexture, Texture2D blockState, Texture2D furnitureTexture, Texture2D wallTexture, Package package, Config config, GraphicsDevice graphics )
        {

            // Setup Liquids
            Fluids.Initialize(SizeX, SizeY, blockState);

            BlockTexture = blockTexture;
            BlockState = blockState;
            WallTexture = wallTexture;
            Entities.Initialize(furnitureTexture);

            // Gather Map Data
            string data = package.RemoteString("maps/manifest/" + config.MapId);
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(data);


            // Parse Map Data into types and blocks

            Spawn = new Vector2(Int32.Parse(xml.DocumentElement.Attributes["spawnx"].Value) * 24, Int32.Parse(xml.DocumentElement.Attributes["spawny"].Value) * 24);
            

            foreach (XmlNode node in xml.DocumentElement.ChildNodes)
            {
                if (node.Name == "BlockType")
                {
                    int upto = 0;
                    foreach (XmlNode blocktype in node.ChildNodes)
                    {
                        Types[upto] = new BlockType(blocktype.Attributes["name"].Value, Int32.Parse(blocktype.Attributes["id"].Value), 100);
                        foreach (XmlNode slice in blocktype.ChildNodes)
                        {
                            Types[upto].Slices[int.Parse(slice.Attributes["i"].Value)] = new Rectangle(int.Parse(slice.Attributes["x"].Value) * 24, int.Parse(slice.Attributes["y"].Value) * 24, 24, 24);
                        }
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
                else if (node.Name == "Liquid")
                {
                    foreach (XmlNode liquid in node.ChildNodes)
                    {
                        if(byte.Parse(liquid.Attributes["type"].Value) == 1){
                            Fluids.Water.Blocks[Int32.Parse(liquid.Attributes["x"].Value), int.Parse(liquid.Attributes["y"].Value)] = 24;
                        }
                        else if (byte.Parse(liquid.Attributes["type"].Value) == 1)
                        {
                            // lava
                        }
                    }
                }
                else if (node.Name == "EntityType")
                {
                    Entities.Types = new EntityType[node.ChildNodes.Count];
                    
                    int i = 0;
                    foreach (XmlNode entitytype in node.ChildNodes)
                    {
                        Entities.Types[i] = new EntityType(
                            int.Parse(entitytype.Attributes["id"].Value),
                            entitytype.Attributes["name"].Value,
                            package.RemoteTexture(entitytype.Attributes["sprite"].Value, graphics),
                            int.Parse(entitytype.Attributes["width"].Value),
                            int.Parse(entitytype.Attributes["height"].Value),
                            true
                        );
                        i++;
                    }
                } 
                else if (node.Name == "Entity")
                {
                    foreach (XmlNode entity in node.ChildNodes)
                    {
                        //EntityType type = Entities.getType();
                        EntityType type = Entities.getType(int.Parse(entity.Attributes["type"].Value));
                        // int.Parse(entity.Attributes["width"].Value)
                        Entities.Entities.Add(
                            new Entity(type, int.Parse(entity.Attributes["x"].Value), int.Parse(entity.Attributes["y"].Value))
                        );
                    }
                }
            }

            // Autogenerate
            //MapGenerator generator = new MapGenerator();
            //generator.Setup(SizeX, SizeY);
            //generator.Generate(this);
            
        }

        public void LoadContent()
        {

        }

        public void Update( Control control, Quickbar quickbar, Camera camera, Lighting lighting )
        {

            Fluids.Update(this);
            Entities.Update(control, camera);

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
            Entities.Draw(spriteBatch, camera);

            // Draw Blocks
            int startx = (int)MathHelper.Clamp(camera.X * -1 / 24f, 0, this.SizeX);
            int endx = startx + camera.Width / 24;
            int starty = (int)MathHelper.Clamp(camera.Y * -1 / 24f, 0, this.SizeY);
            int endy = starty + camera.Height / 24;
            
            for (int x = startx; x < endx + 2; x++)
            {
                for (int y = starty; y < endy + 2; y++)
                {
                    if (Walls[x, y] != null)
                    {
                        Vector2 position = new Vector2(camera.X + x * 24, camera.Y + y * 24);
                        spriteBatch.Draw(WallTexture, position, Walls[x, y].Type.Sprite, Color.White);
                    }
                    if (Blocks[x, y] != null) {
                        Vector2 position = new Vector2(camera.X + x * 24, camera.Y + y * 24);
                        BlockFrame frame = GetSpriteIndex(x,y);
                        spriteBatch.Draw(BlockTexture, new Rectangle((int)position.X + 12, (int)position.Y + 12, 24,24), Blocks[x, y].Type.Slices[frame.Index], Color.White, frame.Rotate, new Vector2(12,12), SpriteEffects.None, 0);


                        if (Blocks[x, y].Health < 10) { spriteBatch.Draw(BlockState, position, new Rectangle(8 * 24, 24, 24, 24), Color.Gray); } else
                        if (Blocks[x, y].Health < 20) { spriteBatch.Draw(BlockState, position, new Rectangle(7 * 24, 24, 24, 24), Color.Gray); } else
                        if (Blocks[x, y].Health < 30) { spriteBatch.Draw(BlockState, position, new Rectangle(6 * 24, 24, 24, 24), Color.Gray); } else
                        if (Blocks[x, y].Health < 40) { spriteBatch.Draw(BlockState, position, new Rectangle(5 * 24, 24, 24, 24), Color.Gray); } else
                        if (Blocks[x, y].Health < 50) { spriteBatch.Draw(BlockState, position, new Rectangle(4 * 24, 24, 24, 24), Color.Gray); } else
                        if (Blocks[x, y].Health < 60) { spriteBatch.Draw(BlockState, position, new Rectangle(3 * 24, 24, 24, 24), Color.Gray); } else
                        if (Blocks[x, y].Health < 70) { spriteBatch.Draw(BlockState, position, new Rectangle(2 * 24, 24, 24, 24), Color.Gray); } else
                        if (Blocks[x, y].Health < 80) { spriteBatch.Draw(BlockState, position, new Rectangle(1 * 24, 24, 24, 24), Color.Gray); } else
                        if (Blocks[x, y].Health < 90) { spriteBatch.Draw(BlockState, position, new Rectangle(0, 24, 24, 24), Color.Gray); }


                    }
                }
            }

            

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public BlockFrame GetSpriteIndex( int x, int y )
        {

            bool top = true;
            bool right = true;
            bool bottom = true;
            bool left = true;
            byte frame;
            float rotate = 0;

            if (getBlock(x, y - 1) != null) { top = false; }
            if (getBlock(x + 1, y) != null) { right = false; }
            if (getBlock(x, y + 1) != null) { bottom = false; }
            if (getBlock(x - 1, y) != null) { left = false; }

            if(top == true && right == false && bottom == false && left == false){
				frame = 1;
			} else if(top == false && right == true && bottom == false && left == false){
				frame = 1;
				rotate = 90;
			} else if(top == false && right == false && bottom == true && left == false){
                frame = 1;
				rotate = 180;
			} else if(top == false && right == false && bottom == false && left == true){
                frame = 1;
				rotate = 270;
			} else if(top == false && right == true && bottom == false && left == true){
                frame = 2;
			} else if(top == true && right == false && bottom == true && left == false){
                frame = 2;
				rotate = 90;
			} else if(top == true && right == false && bottom == false && left == true){
                frame = 3;
			} else if(top == true && right == true && bottom == false && left == false){
                frame = 3;
				rotate = 90;
			} else if(top == false && right == true && bottom == true && left == false){
                frame = 3;
				rotate = 180;
			} else if(top == false && right == false && bottom == true && left == true){
                frame = 3;
				rotate = 270;
			} else if(top == true && right == true && bottom == false && left == true){
                frame = 4;
				rotate = 180;
			} else if(top == true && right == true && bottom == true && left == false){
                frame = 4;
				rotate = 270;
			} else if(top == false && right == true && bottom == true && left == true){
                frame = 4;
			} else if(top == true && right == false && bottom == true && left == true){
                frame = 4;
				rotate = 90;
			} else if(top == true && right == true && bottom == true && left == true){
                frame = 5;
			} else if(top == false && right == false && bottom == false && left == false){
                frame = 6;
			} else { frame = 1; }

            return new BlockFrame(frame, rotate);
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

            Blocks[x, y].Health -= 2;
            if (Blocks[x, y].Health < 1)
            {
                Blocks[x, y] = null;
            }
        }

    }
}
