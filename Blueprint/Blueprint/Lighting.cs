using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Krypton;
using Krypton.Lights;

namespace Blueprint
{
    class Lighting
    {


        public KryptonEngine krypton;
        public Texture2D Light;

        public Light2D Sun;

        public Lighting(Game game)
        {
            krypton = new KryptonEngine(game, "KryptonEffect");

        }

        public void Initialize()
        {
            krypton.Initialize();
        }

        public void LoadContent(GraphicsDevice graphics)
        {
            Light = LightTextureBuilder.CreatePointLight(graphics, 512);

            this.Sun = new Light2D()
            {
                Texture = Light,
                Range = 300000f,
                Color = new Color(150, 150, 150),
                //Intensity = (float)(this.mRandom.NextDouble() * 0.25 + 0.75),
                Intensity = 2222f,
                Angle = MathHelper.TwoPi * 1f,
                X = 0,
                Y = -1000f,

            };

            krypton.Lights.Add(this.Sun);


            //this.CreateLights(Light, this.mNumLights);
            //this.CreateHulls(this.mNumHorzontalHulls, this.mNumVerticalHulls);
        }

        public void Update(GameTime gameTime, Control control, Camera camera, EntityPackage entities)
        {

            Sun.X = 100 * 24;
            Sun.Y = -500 * 24;

            for (int i = 0; i < entities.Entities.Count; i++)
            {
                if (entities.Entities[i] == null) { continue; }
                if (entities.Entities[i].Light == null)
                {
                    entities.Entities[i].Light = new Light2D()
                    {
                        Texture = Light,
                        Range = 100f,
                        Color = new Color(255,255,255),
                        X = camera.FromRectangle(entities.Entities[i].Area).Center.X,
                        Y = camera.FromRectangle(entities.Entities[i].Area).Center.Y,
                        Angle = MathHelper.TwoPi * 1f,
                        Intensity = 2222f,
                    };
                    krypton.Lights.Add(entities.Entities[i].Light);
                }
                else
                {
                    entities.Entities[i].Light.X = camera.FromRectangle(entities.Entities[i].Area).Center.X;
                    entities.Entities[i].Light.Y = camera.FromRectangle(entities.Entities[i].Area).Center.Y;
                }
            }
        }

        public void Draw(GraphicsDevice graphics)
        {
            // Assign the matrix and pre-render the lightmap.
            // Make sure not to change the position of any lights or shadow hulls after this call, as it won't take effect till the next frame!
            this.krypton.SpriteBatchCompatablityEnabled = true;
            this.krypton.CullMode = CullMode.CullClockwiseFace;
            this.krypton.Bluriness = 20;
            this.krypton.AmbientColor = new Color(0,0,0);
            this.krypton.LightMapPrepare();
            
        }

        public void PostDraw(GameTime gameTime, GraphicsDevice graphics)
        {
            this.krypton.Draw(gameTime);
            //DebugDraw(graphics);
        }

        public void AddShadow(Rectangle area)
        {

            var hull = ShadowHull.CreateRectangle(new Vector2(area.Width,area.Height));
            hull.Position.X = area.X + 12;
            hull.Position.Y = area.Y + 12;
            hull.Opacity = 0.2f;
            krypton.Hulls.Add(hull);

        }

        /// <summary>
        /// Clears all current shadows
        /// </summary>
        public void ClearShadows()
            { krypton.Hulls.Clear(); }

        private void DebugDraw(GraphicsDevice graphics)
        {
            this.krypton.RenderHelper.Effect.CurrentTechnique = this.krypton.RenderHelper.Effect.Techniques["DebugDraw"];
            graphics.RasterizerState = new RasterizerState()
            {
                CullMode = CullMode.None,
                FillMode = FillMode.WireFrame,
            };
            if (Keyboard.GetState().IsKeyDown(Keys.H))
            {
                // Clear the helpers vertices
                this.krypton.RenderHelper.ShadowHullVertices.Clear();
                this.krypton.RenderHelper.ShadowHullIndicies.Clear();

                foreach (var hull in krypton.Hulls)
                {
                    this.krypton.RenderHelper.BufferAddShadowHull(hull);
                }


                foreach (var effectPass in krypton.RenderHelper.Effect.CurrentTechnique.Passes)
                {
                    effectPass.Apply();
                    this.krypton.RenderHelper.BufferDraw();
                }
            }

            if (Keyboard.GetState().IsKeyDown(Keys.L))
            {
                this.krypton.RenderHelper.ShadowHullVertices.Clear();
                this.krypton.RenderHelper.ShadowHullIndicies.Clear();

                foreach (Light2D light in krypton.Lights)
                {
                    this.krypton.RenderHelper.BufferAddBoundOutline(light.Bounds);
                }

                foreach (var effectPass in krypton.RenderHelper.Effect.CurrentTechnique.Passes)
                {
                    effectPass.Apply();
                    this.krypton.RenderHelper.BufferDraw();
                }
            }
        }

    }
}
