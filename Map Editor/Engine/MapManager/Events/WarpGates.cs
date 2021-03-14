using System.Collections.Generic;
using Map_Editor.Engine.Map;
using Map_Editor.Engine.Models;
using Map_Editor.Misc;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Map_Editor.Engine.Events
{
    /// <summary>
    /// WarpGates class.
    /// </summary>
    public class WarpGates : DrawableGameComponent
    {
        #region Sub Classes

        /// <summary>
        /// WorldObject class.
        /// </summary>
        public class WorldObject
        {
            /// <summary>
            /// Gets or sets the entry.
            /// </summary>
            /// <value>The entry.</value>
            public IFO.BaseIFO Entry { get; set; }

            /// <summary>
            /// Gets or sets the world.
            /// </summary>
            /// <value>The world.</value>
            public Matrix World { get; set; }

            /// <summary>
            /// Gets or sets the bounding box.
            /// </summary>
            /// <value>The bounding box.</value>
            public BoundingBox BoundingBox { get; set; }
        }

        #endregion

        #region Accessors

        /// <summary>
        /// Gets the tool.
        /// </summary>
        /// <value>The tool.</value>
        public Tools.WarpGates Tool
        {
            get { return (Tools.WarpGates)ToolManager.Tool; }
        }

        /// <summary>
        /// Gets the colour.
        /// </summary>
        /// <value>The colour.</value>
        public Vector3 Colour
        {
            get { return new Vector3(colour, colour, 0.0f); }
        }

        #endregion

        #region Member Declarations

        #region Warp Gate Model

        /// <summary>
        /// Gets or sets the vertex buffer.
        /// </summary>
        /// <value>The vertex buffer.</value>
        public VertexBuffer VertexBuffer { get; private set; }

        /// <summary>
        /// Gets or sets the index buffer.
        /// </summary>
        /// <value>The index buffer.</value>
        public IndexBuffer IndexBuffer { get; private set; }

        /// <summary>
        /// Gets or sets the vertex count.
        /// </summary>
        /// <value>The vertex count.</value>
        public short VertexCount { get; private set; }

        /// <summary>
        /// Gets or sets the index count.
        /// </summary>
        /// <value>The index count.</value>
        public short IndexCount { get; private set; }

        /// <summary>
        /// Gets or sets the world.
        /// </summary>
        /// <value>The world.</value>
        public Matrix World { get; private set; }

        /// <summary>
        /// Gets or sets the vertex declaration.
        /// </summary>
        /// <value>The vertex declaration.</value>
        public VertexDeclaration VertexDeclaration { get; private set; }

        /// <summary>
        /// Gets or sets the vertices.
        /// </summary>
        /// <value>The vertices.</value>
        private ZMS.Vertex[] vertices { get; set; }

        #endregion

        /// <summary>
        /// Gets or sets the warp gate model.
        /// </summary>
        /// <value>The warp gate model.</value>
        public Model WarpGateModel { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="WarpGates"/> is loading.
        /// </summary>
        /// <value><c>true</c> if loading; otherwise, <c>false</c>.</value>
        public bool Loading { get; set; }

        /// <summary>
        /// Gets or sets the colour.
        /// </summary>
        /// <value>The colour.</value>
        private float colour { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [increasing colour].
        /// </summary>
        /// <value><c>true</c> if [increasing colour]; otherwise, <c>false</c>.</value>
        private bool increasingColour { get; set; }

        #endregion

        #region List Declarations

        /// <summary>
        /// Gets or sets the world objects.
        /// </summary>
        /// <value>The world objects.</value>
        public List<WorldObject> WorldObjects { get; set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="WarpGates"/> class.
        /// </summary>
        /// <param name="game">The Game that the game component should be attached to.</param>
        public WarpGates(Game game)
            : base(game)
        {
            WarpGateModel = Game.Content.Load<Model>("WarpGate");

            WorldObjects = new List<WorldObject>();

            ZMS warpModel = new ZMS(FileManager.ZSCs["LIST_DECO_SPECIAL"].Models[FileManager.ZSCs["LIST_DECO_SPECIAL"].Objects[1].Models[0].ModelID]);

            vertices = warpModel.Vertices;

            World = Matrix.CreateFromQuaternion(FileManager.ZSCs["LIST_DECO_SPECIAL"].Objects[1].Models[0].Rotation) *
                    Matrix.CreateScale(FileManager.ZSCs["LIST_DECO_SPECIAL"].Objects[1].Models[0].Scale) *
                    Matrix.CreateTranslation(FileManager.ZSCs["LIST_DECO_SPECIAL"].Objects[1].Models[0].Position);

            VertexBuffer = warpModel.CreateVertexBuffer(Game.GraphicsDevice);
            IndexBuffer = warpModel.CreateIndexBuffer(Game.GraphicsDevice);

            VertexCount = warpModel.VertexCount;
            IndexCount = warpModel.IndexCount;

            VertexDeclaration = new VertexDeclaration(Game.GraphicsDevice, ZMS.Vertex.VertexElements);

            DrawOrder = MapManager.DRAWORDER_WARPGATES;

            colour = 1.0f;
            increasingColour = false;
        }

        /// <summary>
        /// Adds the specified warp gate.
        /// </summary>
        /// <param name="warpGate">The warp gate.</param>
        public void Add(IFO.BaseIFO warpGate)
        {
            WorldObjects.Add(new WorldObject());

            Add(WorldObjects.Count - 1, warpGate, false);
        }

        /// <summary>
        /// Adds the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="warpGate">The warp gate.</param>
        /// <param name="removePrevious">if set to <c>true</c> [remove previous].</param>
        public void Add(int id, IFO.BaseIFO warpGate, bool removePrevious)
        {
            Matrix objectWorld = Matrix.CreateFromQuaternion(warpGate.Rotation) *
                                             Matrix.CreateScale(warpGate.Scale) *
                                    Matrix.CreateTranslation(warpGate.Position);

            Vector3[] vectorPositions = new Vector3[VertexCount];

            for (int i = 0; i < vectorPositions.Length; i++)
                vectorPositions[i] = Vector3.Transform(vertices[i].Position, World * objectWorld);

            WorldObjects[id] = new WorldObject()
            {
                Entry = warpGate,
                World = objectWorld,
                BoundingBox = BoundingBox.CreateFromPoints(vectorPositions)
            };

            if (removePrevious)
            {
                for (int i = 0; i < UndoManager.Commands.Length; i++)
                {
                    if (UndoManager.Commands[i] == null)
                        continue;

                    if ((UndoManager.Commands[i].GetType() == typeof(Commands.WarpGates.ValueChanged) ||
                         UndoManager.Commands[i].GetType() == typeof(Commands.WarpGates.Positioned) ||
                         UndoManager.Commands[i].GetType() == typeof(Commands.WarpGates.Added) ||
                         UndoManager.Commands[i].GetType() == typeof(Commands.WarpGates.Removed)) && ((Commands.WarpGates.WarpGate)UndoManager.Commands[i]).ObjectID == id)
                        ((Commands.WarpGates.WarpGate)UndoManager.Commands[i]).Object = WorldObjects[id];
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
                UndoManager.AddCommand(new Commands.WarpGates.Removed()
                {
                    ObjectID = id,
                    Object = WorldObjects[id],
                    Entry = WorldObjects[id].Entry
                });
            }

            WorldObjects[id].Entry.Parent.WarpGates.Remove(WorldObjects[id].Entry);
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
        /// Draws the warp gate.
        /// </summary>
        public void DrawWarpGate()
        {
            Game.GraphicsDevice.VertexDeclaration = VertexDeclaration;

            Game.GraphicsDevice.Indices = IndexBuffer;
            Game.GraphicsDevice.Vertices[0].SetSource(VertexBuffer, 0, ZMS.Vertex.SIZE_IN_BYTES);

            Game.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, VertexCount, 0, IndexCount);
        }

        /// <summary>
        /// Gets the colour.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>The colour to use depending on the selected and hovered object.</returns>
        public Vector3 GetColour(int index)
        {
            if (Tool.SelectedObject == index)
                return new Vector3(1.0f, 1.0f, 1.0f);

            if (Tool.HoveredObject == index)
                return new Vector3(colour, 0.0f, 0.0f);

            return new Vector3(colour, colour, 0.0f);
        }

        /// <summary>
        /// Draws the warp gates.
        /// </summary>
        /// <param name="gameTime">Time passed since the last call to Draw.</param>
        public override void Draw(GameTime gameTime)
        {
            if (Loading || !ConfigurationManager.GetValue<bool>("Draw", "WarpGates"))
                return;

            if (ToolManager.GetToolMode() == ToolManager.ToolMode.WarpGates)
            {
                if (increasingColour)
                {
                    colour += 0.01f;

                    if (colour >= 0.8f)
                        increasingColour = false;
                }
                else
                {
                    colour -= 0.01f;

                    if (colour <= 0.3f)
                        increasingColour = true;
                }
            }

            for(int i = 0; i < WorldObjects.Count; i++)
            {
                foreach (ModelMesh mesh in WarpGateModel.Meshes)
                {
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.EnableDefaultLighting();

                        if (ToolManager.GetToolMode() == ToolManager.ToolMode.WarpGates)
                        {
                            if (Tool.SelectedObject == i)
                                effect.DiffuseColor = new Vector3(1.0f, 1.0f, 1.0f);
                            else if (Tool.HoveredObject == i)
                                effect.DiffuseColor = new Vector3(colour, 0.0f, 0.0f);
                            else
                                effect.DiffuseColor = new Vector3(colour, colour, 0.0f);
                        }
                        else
                            effect.DiffuseColor = new Vector3(1.0f, 1.0f, 0.0f);

                        Vector3 worldPosition = WorldObjects[i].BoundingBox.GetCenter();

                        effect.World = Matrix.CreateWorld(worldPosition, Vector3.Normalize(worldPosition - CameraManager.Position) * new Vector3(1.0f, 1.0f, 0.0f), Vector3.Backward);
                        effect.View = CameraManager.View;
                        effect.Projection = CameraManager.Projection;
                    }

                    mesh.Draw();
                }
            }

            if (ToolManager.GetToolMode() == ToolManager.ToolMode.WarpGates)
                Tool.Draw(ConfigurationManager.GetValue<bool>("WarpGates", "DrawWarpAreas"));
        }
    }
}