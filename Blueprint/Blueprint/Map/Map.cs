using System;
using System.Xml;
using System.Net;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Blueprint
{
    class Map
    {

        #region SubClasses

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

        public FloraPackage Flora;

        /// <summary>Collection of all dropped items on the map</summary>
        public DroppedItemCollection DroppedItems;

        /// <summary>Collection of all the fluids on the map</summary>
        public Fluid.FluidCollection Fluids;

        #endregion

        #region Textures and assets

        public Texture2D BlockTexture;
        public Texture2D BlockState;
        public Texture2D WallTexture;

        #endregion

        #region Metrics

        /// <summary>Defines how wide the map is in terms of blocks</summary>
        public int SizeX;

        /// <summary>Defines how high the map is in terms of blocks</summary>
        public int SizeY;

        /// <summary>
        /// Spawn Location
        /// </summary>
        public Vector2 Spawn;

        #endregion

        /// <summary>
        /// Used for delaying the grow function in the flora subclass
        /// </summary>
        private int GrowCounter;

        public Map(int width = 900, int height = 500)
        {

            // Init
            SizeX = width;
            SizeY = height;
            Types = new BlockType[30];
            WallTypes = new WallType[10];
            Blocks = new Block[SizeX, SizeY];
            Walls = new Wall[SizeX, SizeY];
            DroppedItems = new DroppedItemCollection();
            Fluids = new Fluid.FluidCollection();
            Entities = new EntityPackage();
            Flora = new FloraPackage();
            WallTypes[0] = new WallType(1, "Standard Wall", new Rectangle(0,0,24,24));

        }

        public void Initialize( Texture2D mapTexture, Package package, Config config, GraphicsDevice graphics, ContentManager content )
        {

            // Setup Liquids
            Fluids.Initialize(SizeX, SizeY, content.Load<Texture2D>("Blocks/blocks"));

            BlockTexture = mapTexture;
            BlockState = content.Load<Texture2D>("Blocks/blocks");
            WallTexture = content.Load<Texture2D>("Blocks/wall");
            Entities.Initialize(content);
            Flora.Initialize(this, content.Load<Texture2D>("flora"));
            // Gather Map Data
            string data = package.RemoteString("maps/manifest/" + config.MapId);
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(data);



            Spawn = new Vector2(Int32.Parse(xml.DocumentElement.Attributes["spawnx"].Value) * 24, Int32.Parse(xml.DocumentElement.Attributes["spawny"].Value) * 24);

            #region Xml Parsing

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
                    //Entities.Types = new EntityType[node.ChildNodes.Count];
                    
                    int i = 0;
                    foreach (XmlNode entitytype in node.ChildNodes)
                    {
                        /*Entities.Types[i] = new EntityType(
                            int.Parse(entitytype.Attributes["id"].Value),
                            entitytype.Attributes["name"].Value,
                            package.RemoteTexture(entitytype.Attributes["sprite"].Value, graphics),
                            int.Parse(entitytype.Attributes["width"].Value),
                            int.Parse(entitytype.Attributes["height"].Value),
                            true
                        );*/
                        i++;
                    }
                } 
                else if (node.Name == "Entity")
                {
                    foreach (XmlNode entity in node.ChildNodes)
                    {
                        //EntityType type = Entities.getType();
                        //EntityType type = Entities.getType(int.Parse(entity.Attributes["type"].Value));
                        // int.Parse(entity.Attributes["width"].Value)
                        /*
                        Entities.Entities.Add(
                            new Entity(type, int.Parse(entity.Attributes["x"].Value), int.Parse(entity.Attributes["y"].Value))
                        );*/
                    }
                }
            }

            #endregion

            // Autogenerate
            //MapGenerator generator = new MapGenerator();
            //generator.Setup(SizeX, SizeY);
            //generator.Generate(this);
            
        }

        public void Update( Control control, Quickbar quickbar, Camera camera, Lighting lighting, Player player )
        {

            Fluids.Update(this);
            Entities.Update(control, camera);

            #region Flora

            // Grow Flora
            GrowCounter++;
            if (GrowCounter > 1000)
            {
                Flora.Update(this);
                GrowCounter = 0;
            }

            #endregion

            #region Using Items

            // preview
            Entities.Preview = null;
            if (quickbar.CurrentItem != null && Geometry.Range(camera.FromRectangle(player.Movement.Area), control.MousePos) < 6)
            {
                ItemUse use = new ItemUse(quickbar.CurrentItem.Type.Use);
                if (use.Type == "placeentity" && CanPlaceEntity(Entities.getType(use.IntValue), control.AtTileX, control.AtTileY))
                {
                    Entities.Preview = new Entity(Entities.getType(int.Parse(use.Value)), control.AtTileX, control.AtTileY);
                }
            }

            if (quickbar.UsingItem.Type == "placeblock" && Geometry.Range(camera.FromRectangle(player.Movement.Area), control.MousePos) < 6)
            {
                if (placeBlock(control.AtTileX, control.AtTileY, GetBlockType(quickbar.UsingItem.IntValue)))
                    { quickbar.useItem(quickbar.Selected); }
            }

            if (quickbar.UsingItem.Type == "placewall" && Geometry.Range(camera.FromRectangle(player.Movement.Area), control.MousePos) < 6)
            {
                if (placeWall(control.AtTileX, control.AtTileY, GetWallType(quickbar.UsingItem.IntValue)))
                    { quickbar.useItem(quickbar.Selected); }
            }

            if (quickbar.UsingItem.Type == "mineblock" && Geometry.Range(camera.FromRectangle(player.Movement.Area), control.MousePos) < 6)
            {
                if (mineBlock(control.AtTileX, control.AtTileY))
                    { quickbar.useItem(quickbar.Selected); }
            }

            if (quickbar.UsingItem.Type == "removewall" && Geometry.Range(camera.FromRectangle(player.Movement.Area), control.MousePos) < 6)
            {
                if (RemoveWall(control.AtTileX, control.AtTileY))
                { quickbar.useItem(quickbar.Selected); }
            }

            if (quickbar.UsingItem.Type == "placeentity" && Geometry.Range(camera.FromRectangle(player.Movement.Area), control.MousePos) < 6 && CanPlaceEntity(Entities.getType(quickbar.UsingItem.IntValue), control.AtTileX, control.AtTileY))
            {
                if( Entities.Add(control.AtTileX, control.AtTileY, quickbar.UsingItem.IntValue) )
                    { quickbar.useItem(quickbar.Selected); }
            }

            if (quickbar.UsingItem.Type == "removeentity" && Geometry.Range(camera.FromRectangle(player.Movement.Area), control.MousePos) < 6)
            {
                if( Entities.Damage(control.AtTileX, control.AtTileY))
                    { quickbar.useItem(quickbar.Selected); }
            }

            #endregion

            #region Lighting
            
            lighting.ClearShadows();

            int startx = (int)MathHelper.Clamp(camera.X * -1 / 24f, 0, this.SizeX);
            int endx = startx + camera.Width / 24;
            int starty = (int)MathHelper.Clamp(camera.Y * -1 / 24f, 0, this.SizeY);
            int endy = starty + camera.Height / 24;

            for (int x = startx; x <= endx + 2; x++)
            {
                for (int y = starty; y <= endy; y++)
                {
                    if (Blocks[x, y] != null)
                    {
                        lighting.AddShadow(camera.FromRectangle(new Rectangle(x * 24, y * 24,24,24)));
                    }
                    if (Fluids.Water.Blocks[x, y] > 18)
                    {
                        lighting.AddShadow(camera.FromRectangle(new Rectangle(x * 24, y * 24, 24, 24)));
                    }
                }
            }
            
            #endregion

        }

        public void Draw(SpriteBatch spriteBatch, Camera camera)
        {

            #region Draw Blocks

            int startx = (int)MathHelper.Clamp(camera.X * -1 / 24f, 0, this.SizeX);
            int endx = startx + camera.Width / 24;
            int starty = (int)MathHelper.Clamp(camera.Y * -1 / 24f, 0, this.SizeY);
            int endy = starty + camera.Height / 24;
            
            for (int x = startx; x < endx + 2; x++)
            {
                for (int y = starty; y < endy + 2; y++)
                {
                    Vector2 position = new Vector2(camera.X + x * 24, camera.Y + y * 24);
                    if (Walls[x, y] != null)
                    {
                        spriteBatch.Draw(WallTexture, position, Walls[x, y].Type.Sprite, Color.White);
                    }
                    if (Blocks[x, y] != null) {
                        BlockFrame frame = GetSpriteIndex(x,y);
                        spriteBatch.Draw(BlockTexture, new Rectangle((int)position.X + 12, (int)position.Y + 12, 24,24), Blocks[x, y].Type.Slices[frame.Index], Color.White, frame.Rotate, new Vector2(12,12), SpriteEffects.None, 0);

                        // Draw Block Damage
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
                    if (Flora.Flora[x, y] != null)
                    {
                        spriteBatch.Draw(Flora.Sprite, position, Flora.Flora[x, y].Type.Sprite, Color.White);
                    }
                }
            }

            #endregion

            Entities.Draw(spriteBatch, camera);

        }

        /// <summary>
        /// Attempts to discover a new "house area" at x/y
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void DiscoverAreas(int x, int y)
        {

            List<Vector2> blocks = new List<Vector2>();
            List<Vector2> ground = new List<Vector2>();
            bool found_room = false;
            int checks_left = 1000;
            string current_direction = "left";
            Vector2 starting_position = new Vector2(x, y);

            // Find the ground
            while (true)
                { if (getBlock(x, y + 1) == null) { y += 1; } else { break; } }


            // Go around and around
            while (checks_left > 0)
            {
                
                // Find best block to move to
                if (current_direction == "left") { current_direction = "down"; }
                if (current_direction == "top") { current_direction = "left"; }
                if (current_direction == "right") { current_direction = "top"; }
                if (current_direction == "bottom") { current_direction = "right"; }
                while (true)
                {
                    if (current_direction == "left")
                    {
                        if (getBlock(x - 1, y) == null) { x -= 1; break; } else { current_direction = "top"; }
                    }
                    if (current_direction == "top")
                    {
                        if (getBlock(x, y - 1) == null) { y -= 1; break; } else { current_direction = "right"; }
                    }
                    if (current_direction == "right")
                    {
                        if (getBlock(x + 1, y) == null) { x += 1; break; } else { current_direction = "bottom"; }
                    }
                    if (current_direction == "bottom")
                    {
                        if (getBlock(x, y + 1) == null) { y+=1; break; } else { current_direction = "left"; }
                    }
                }

                // Checks
                if (new Vector2(x, y) == starting_position) { found_room = true; break; }
                blocks.Add(new Vector2(x, y));
                if (getBlock(x, y + 1) != null) { ground.Add(new Vector2(x, y)); }

                checks_left--;
            }


            // Did we make it around? If we did, gather the rest of the blocks by looking upwards from each ground block
            if (found_room)
            {
                foreach (Vector2 item in ground)
                {
                    // do stuff
                }
            }

            // Finish Up

        }

        /// <summary>
        /// Returns a block frame for a block on the map
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

            if (top == false && right == false && bottom == false && left == false) {
                frame = 6;
            } else if(top == true && right == false && bottom == false && left == false){
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
			} else { frame = 1; }

            return new BlockFrame(frame, rotate);
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

        #region Blocks

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
        /// Safely gets a block from the map based of x/y
        /// </summary>
        /// <param name="x">Coordianate X of the block location</param>
        /// <param name="y">Coordianate Y of the block location</param>
        /// <returns>Returns a Block object if a block exists, otherwise returns null</returns>
        public Block getBlock(int x, int y)
        {
            if (inBounds(x, y)) { return Blocks[x, y]; } else { return null; }
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
            if (getBlock(x - 1, y) == null && getBlock(x + 1, y) == null && getBlock(x, y - 1) == null && getBlock(x, y + 1) == null)
            {
                return false;
            }

            Blocks[x, y] = new Block(type);
            Fluids.Water.Blocks[x, y] = 0;
            return true;
        }

        /// <summary>
        /// Does damage to a block, returns true if broken
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public bool mineBlock(int x, int y)
        {
            if (!inBounds(x, y)) { return false; }

            if (Blocks[x, y] == null) { return false; }

            Blocks[x, y].Health -= 2;
            if (Blocks[x, y].Health < 1)
            {
                Blocks[x, y] = null;
                Walls[x, y] = new Wall(WallTypes[1]);
                return true;
            }
            return false;

        }

        #endregion

        #region Walls

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

        public Wall getWall(int x, int y)
        {
            if (inBounds(x, y)) { return Walls[x, y]; } else { return null; }
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

        public bool RemoveWall(int x, int y)
        {

            if (!inBounds(x, y)) { return false; }
            if (Walls[x, y] == null) { return false; }

            Walls[x, y].Health -= 2;
            if (Walls[x, y].Health < 1)
            {
                Walls[x, y] = null;
                return true;
            }
            return false;
        }

        #endregion

        #region Entities

        /// <summary>
        /// Returns wether an entity can be placed
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool CanPlaceEntity(EntityType type, int x, int y)
        {
            if (Blocks[x, y + 1] == null) { return false; }
            for (int x_upto = x; x_upto < x + type.Width; x_upto++)
            {
                for (int y_upto = y; y_upto > y - type.Height; y_upto--)
                {
                    if (!inBounds(x_upto, y_upto)) { continue; }
                    if (Blocks[x_upto, y_upto] != null) { return false; }
                    if (Fluids.Water.Blocks[x_upto, y_upto] != 0) { return false; }
                    if (Flora.Flora[x_upto, y_upto] != null) { return false; }
                }
            }

            Rectangle rect = new Rectangle(x * 24, y * 24, type.Width * 24, type.Height * 24);
            foreach (Entity entity in Entities.Entities)
            {
                if (entity.Area.Intersects(rect))
                    return false;
            }

            return true;
        }

        #endregion

    }
}
