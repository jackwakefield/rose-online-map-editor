using System.Collections.Generic;
using Map_Editor.Engine.Map;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Map_Editor.Misc;

namespace Map_Editor.Engine.Terrain
{
    /// <summary>
    /// Water class.
    /// </summary>
    public class Water : DrawableGameComponent
    {
        #region Sub Classes

        /// <summary>
        /// WorldObject class.
        /// </summary>
        public class WorldObject
        {
            #region Member Declarations

            /// <summary>
            /// Gets or sets the entry.
            /// </summary>
            /// <value>The entry.</value>
            public IFO.WaterBlock Entry { get; set; }

            /// <summary>
            /// Gets or sets the vertex buffer.
            /// </summary>
            /// <value>The vertex buffer.</value>
            public VertexBuffer VertexBuffer { get; set; }

            /// <summary>
            /// Gets or sets the bounding box.
            /// </summary>
            /// <value>The bounding box.</value>
            public BoundingBox BoundingBox { get; set; }

            #endregion
        }

        #endregion

        #region Accessors

        /// <summary>
        /// Gets the tool.
        /// </summary>
        /// <value>The tool.</value>
        public Tools.Water Tool
        {
            get { return (Tools.Water)ToolManager.Tool; }
        }

        #endregion

        #region Member Declarations

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Water"/> is loading.
        /// </summary>
        /// <value><c>true</c> if loading; otherwise, <c>false</c>.</value>
        public bool Loading { get; set; }

        /// <summary>
        /// Gets or sets the index buffer.
        /// </summary>
        /// <value>The index buffer.</value>
        private IndexBuffer indexBuffer { get; set; }

        /// <summary>
        /// Gets or sets the vertex declaration.
        /// </summary>
        /// <value>The vertex declaration.</value>
        private VertexDeclaration vertexDeclaration { get; set; }

        /// <summary>
        /// Gets or sets the water textures.
        /// </summary>
        /// <value>The water textures.</value>
        private Texture2D[] waterTextures { get; set; }

        /// <summary>
        /// Gets or sets the current texture.
        /// </summary>
        /// <value>The current texture.</value>
        private int currentTexture { get; set; }

        #endregion

        #region List Declarations

        /// <summary>
        /// Gets or sets the world objects.
        /// </summary>
        /// <value>The world objects.</value>
        public List<WorldObject> WorldObjects { get; set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="Water"/> class.
        /// </summary>
        /// <param name="game">The Game that the game component should be attached to.</param>
        public Water(Game game)
            : base(game)
        {
            WorldObjects = new List<WorldObject>();

            waterTextures = new Texture2D[24];

            for (int i = 0; i < 24; i++)
                waterTextures[i] = Texture2D.FromFile(game.GraphicsDevice, string.Format(@"3Ddata\JUNON\WATER\OCEAN01_{0:00}.DDS", i + 1));

            currentTexture = 0;

            indexBuffer = new IndexBuffer(game.GraphicsDevice, typeof(short), sizeof(short) * 6, BufferUsage.WriteOnly);
            indexBuffer.SetData(new short[]
            { 
                0, 3, 2,
                2, 1, 0
            });

            vertexDeclaration = new VertexDeclaration(game.GraphicsDevice, VertexPositionTexture.VertexElements);

            DrawOrder = MapManager.DRAWORDER_WATER;
        }

        /// <summary>
        /// Adds the specified ifo object.
        /// </summary>
        /// <param name="ifoObject">The ifo object.</param>
        public void Add(IFO.WaterBlock ifoObject)
        {
            WorldObjects.Add(new WorldObject());

            Add(WorldObjects.Count - 1, ifoObject, false);
        }

        /// <summary>
        /// Adds the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="ifoObject">The ifo object.</param>
        /// <param name="removePrevious">if set to <c>true</c> [remove previous].</param>
        public void Add(int id, IFO.WaterBlock ifoObject, bool removePrevious)
        {
            VertexPositionTexture[] veritces = new VertexPositionTexture[]
            {
                new VertexPositionTexture()
                {
                    Position = new Vector3(ifoObject.Minimum.X, ifoObject.Minimum.Y, ifoObject.Minimum.Z),
                    TextureCoordinate = new Vector2(ifoObject.Minimum.X / 16.0f, ifoObject.Minimum.Y / 16.0f)
                },                
                new VertexPositionTexture()
                {
                    Position = new Vector3(ifoObject.Maximum.X, ifoObject.Minimum.Y, ifoObject.Minimum.Z),
                    TextureCoordinate = new Vector2(ifoObject.Maximum.X / 16.0f, ifoObject.Minimum.Y / 16.0f)
                },
                new VertexPositionTexture()
                {
                    Position = new Vector3(ifoObject.Maximum.X, ifoObject.Maximum.Y, ifoObject.Minimum.Z),
                    TextureCoordinate = new Vector2(ifoObject.Maximum.X / 16.0f, ifoObject.Maximum.Y / 16.0f)
                },
                new VertexPositionTexture()
                {
                    Position = new Vector3(ifoObject.Minimum.X, ifoObject.Maximum.Y, ifoObject.Minimum.Z),
                    TextureCoordinate = new Vector2(ifoObject.Minimum.X / 16.0f, ifoObject.Maximum.Y / 16.0f)
                }
            };

            WorldObject newObject = new WorldObject()
            {
                Entry = ifoObject,
                VertexBuffer = new VertexBuffer(Game.GraphicsDevice, VertexPositionTexture.SizeInBytes * 4, BufferUsage.WriteOnly),
                BoundingBox = BoundingBox.CreateFromPoints(new Vector3[]
                {
                    new Vector3(ifoObject.Minimum.X, ifoObject.Minimum.Y, ifoObject.Minimum.Z),
                    new Vector3(ifoObject.Maximum.X, ifoObject.Minimum.Y, ifoObject.Minimum.Z),
                    new Vector3(ifoObject.Maximum.X, ifoObject.Maximum.Y, ifoObject.Minimum.Z),
                    new Vector3(ifoObject.Minimum.X, ifoObject.Maximum.Y, ifoObject.Minimum.Z)
                })
            };

            newObject.VertexBuffer.SetData(veritces);

            WorldObjects[id] = newObject;

            if (removePrevious)
            {
                for (int i = 0; i < UndoManager.Commands.Length; i++)
                {
                    if (UndoManager.Commands[i] == null)
                        continue;

                    if ((UndoManager.Commands[i].GetType() == typeof(Commands.Water.ValueChanged) ||
                         UndoManager.Commands[i].GetType() == typeof(Commands.Water.Added) ||
                         UndoManager.Commands[i].GetType() == typeof(Commands.Water.Removed)) && ((Commands.Water.Water)UndoManager.Commands[i]).ObjectID == id)
                        ((Commands.Water.Water)UndoManager.Commands[i]).Object = WorldObjects[id];
                }
            }
        }

        /// <summary>
        /// Removes at.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="undoAction">if set to <c>true</c> [undo action].</param>
        public void RemoveAt(int id, bool undoAction)
        {
            if (!undoAction)
            {
                UndoManager.AddCommand(new Commands.Water.Removed()
                {
                    ObjectID = id,
                    Object = WorldObjects[id],
                    IFOEntry = WorldObjects[id].Entry
                });
            }

            WorldObjects[id].Entry.Parent.Water.Remove(WorldObjects[id].Entry);
            WorldObjects.RemoveAt(id);
        }

        /// <summary>
        /// Clears this instance.
        /// </summary>
        public void Clear()
        {
            WorldObjects.Clear();
        }

        /// <summary>
        /// Updates the texture index.
        /// </summary>
        /// <param name="gameTime">Time elapsed since the last call to Update</param>
        public override void Update(GameTime gameTime)
        {
            currentTexture = (int)((float)gameTime.TotalGameTime.TotalSeconds * 10) % 24;
        }

        /// <summary>
        /// Draws the objects.
        /// </summary>
        /// <param name="gameTime">Time passed since the last call to Draw.</param>
        public override void Draw(GameTime gameTime)
        {
            if (Loading || !ConfigurationManager.GetValue<bool>("Draw", "Water"))
                return;

            BoundingFrustum boundingFrustum = new BoundingFrustum(CameraManager.View * CameraManager.Projection);

            Game.GraphicsDevice.Indices = indexBuffer;
            Game.GraphicsDevice.VertexDeclaration = vertexDeclaration;

            Game.GraphicsDevice.RenderState.AlphaBlendEnable = true;
            Game.GraphicsDevice.RenderState.AlphaBlendEnable = true;

            Game.GraphicsDevice.RenderState.SourceBlend = Blend.SourceAlpha;
            Game.GraphicsDevice.RenderState.DestinationBlend = Blend.One;
            Game.GraphicsDevice.RenderState.BlendFunction = BlendFunction.Add;

            Effect shader = ShaderManager.GetShader(ShaderManager.ShaderType.SimpleTexture);

            shader.SetValue("WorldViewProjection", CameraManager.View * CameraManager.Projection);
            shader.SetValue("Texture", waterTextures[currentTexture]);
            shader.SetValue("Alpha", 0.5f);

            shader.Start("SimpleTexture");

            for(int i = 0; i < WorldObjects.Count; i++)
            {
                if (!boundingFrustum.OnScreen(WorldObjects[i].BoundingBox))
                    continue;

                if (ToolManager.GetToolMode() == ToolManager.ToolMode.Water)
                {
                    if(i == Tool.SelectedObject)
                        shader.SetValue("Addition", new Vector4(0.25f, 0.25f, 0.25f, 0.0f));
                    else if (i == Tool.HoveredObject)
                        shader.SetValue("Addition", new Vector4(0.25f, 0.0f, 0.0f, 0.0f));
                    else
                        shader.SetValue("Addition", Vector4.Zero);
                }
                else
                    shader.SetValue("Addition", Vector4.Zero);

                shader.CommitChanges();

                Game.GraphicsDevice.Vertices[0].SetSource(WorldObjects[i].VertexBuffer, 0, VertexPositionTexture.SizeInBytes);

                Game.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 4, 0, 2);
            }

            if (ToolManager.GetToolMode() == ToolManager.ToolMode.Water)
                Tool.Draw();
            else
                shader.Finish();


            Game.GraphicsDevice.RenderState.SourceBlend = Blend.One;
            Game.GraphicsDevice.RenderState.DestinationBlend = Blend.Zero;
            Game.GraphicsDevice.RenderState.BlendFunction = BlendFunction.Add;
        }
    }
}