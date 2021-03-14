using Map_Editor.Engine.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Map_Editor.Misc;

namespace Map_Editor.Engine.Misc
{
    /// <summary>
    /// Sky class.
    /// </summary>
    public class Sky : DrawableGameComponent
    {
        #region Member Declarations

        #region Object Declarations

        /// <summary>
        /// Gets or sets the vertex buffer.
        /// </summary>
        /// <value>The vertex buffer.</value>
        private VertexBuffer vertexBuffer { get; set; }

        /// <summary>
        /// Gets or sets the index buffer.
        /// </summary>
        /// <value>The index buffer.</value>
        private IndexBuffer indexBuffer { get; set; }

        /// <summary>
        /// Gets or sets the vertex count.
        /// </summary>
        /// <value>The vertex count.</value>
        private int vertexCount { get; set; }

        /// <summary>
        /// Gets or sets the index count.
        /// </summary>
        /// <value>The index count.</value>
        private int indexCount { get; set; }

        /// <summary>
        /// Gets or sets the vertex declaration.
        /// </summary>
        /// <value>The vertex declaration.</value>
        private VertexDeclaration vertexDeclaration { get; set; }

        /// <summary>
        /// Gets or sets the texture.
        /// </summary>
        /// <value>The texture.</value>
        private Texture2D texture { get; set; }

        #endregion

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Sky"/> is loading.
        /// </summary>
        /// <value><c>true</c> if loading; otherwise, <c>false</c>.</value>
        public bool Loading { get; set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="Sky"/> class.
        /// </summary>
        /// <param name="game">The Game that the game component should be attached to.</param>
        public Sky(Game game)
            : base(game)
        {
            DrawOrder = MapManager.DRAWORDER_SKY;
        }

        /// <summary>
        /// Loads the specified model.
        /// </summary>
        /// <param name="modelPath">The model path.</param>
        /// <param name="texturePath">The texture path.</param>
        public void Load(string modelPath, string texturePath)
        {
            ZMS skyModel = new ZMS();
            skyModel.Load(modelPath);

            vertexBuffer = skyModel.CreateVertexBuffer(Game.GraphicsDevice);
            indexBuffer = skyModel.CreateIndexBuffer(Game.GraphicsDevice);

            vertexCount = skyModel.VertexCount;
            indexCount = skyModel.IndexCount;

            texture = Texture2D.FromFile(Game.GraphicsDevice, texturePath);

            vertexDeclaration = new VertexDeclaration(Game.GraphicsDevice, ZMS.Vertex.VertexElements);

            Enabled = true;
        }

        /// <summary>
        /// Clears the screen and draws the sky.
        /// </summary>
        /// <param name="gameTime">Time passed since the last call to Draw.</param>
        public override void Draw(GameTime gameTime)
        {
            Game.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, new Color(127, 127, 127), 1.0f, 0);

            if (vertexCount == 0 || Loading || !ConfigurationManager.GetValue<bool>("Draw", "Sky"))
                return;

            Game.GraphicsDevice.RenderState.DepthBufferEnable = false;
            Game.GraphicsDevice.RenderState.DepthBufferWriteEnable = false;

            Effect shader = ShaderManager.GetShader(ShaderManager.ShaderType.SimpleTexture);

            Game.GraphicsDevice.VertexDeclaration = vertexDeclaration;

            Game.GraphicsDevice.Indices = indexBuffer;
            Game.GraphicsDevice.Vertices[0].SetSource(vertexBuffer, 0, ZMS.Vertex.SIZE_IN_BYTES);

            shader.SetValue("WorldViewProjection", Matrix.CreateTranslation(CameraManager.Position) * CameraManager.View * CameraManager.Projection);
            shader.SetValue("Texture", texture);
            shader.SetValue("Addition", Vector4.Zero);

            shader.Start("SimpleTexture");

            Game.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, vertexCount, 0, indexCount);

            shader.Finish();

            Game.GraphicsDevice.RenderState.DepthBufferWriteEnable = true;
            Game.GraphicsDevice.RenderState.DepthBufferEnable = true;
        }
    }
}