using System;
using System.Collections.Generic;
using Map_Editor.Engine.Map;
using Map_Editor.Engine.Models;
using Map_Editor.Engine.RenderManager;
using Map_Editor.Misc;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Map_Editor.Engine.Objects
{
    /// <summary>
    /// Animation class.
    /// </summary>
    public class Animation : DrawableGameComponent
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

            /// <summary>
            /// Gets or sets the model ID.
            /// </summary>
            /// <value>The model ID.</value>
            public int ModelID { get; set; }

            /// <summary>
            /// Gets or sets the texture ID.
            /// </summary>
            /// <value>The texture ID.</value>
            public int TextureID { get; set; }

            /// <summary>
            /// Gets or sets the batch object.
            /// </summary>
            /// <value>The batch object.</value>
            public AnimationManager.BatchObject BatchObject { get; set; }

            #endregion

            /// <summary>
            /// Adds the object to the specified animation manager.
            /// </summary>
            /// <param name="animationManager">The animation manager.</param>
            public void Add(AnimationManager animationManager)
            {
                BatchObject = animationManager.AddToBatch(ModelID, TextureID, BoundingBox, World);
            }

            /// <summary>
            /// Removes the object from the specified animation manager.
            /// </summary>
            /// <param name="animationManager">The animation manager.</param>
            public void Remove(AnimationManager animationManager)
            {
                animationManager.RemoveFromBatch(BatchObject);
            }
        }

        #endregion

        #region Accessors

        /// <summary>
        /// Gets the tool.
        /// </summary>
        /// <value>The tool.</value>
        public Tools.Animation Tool
        {
            get { return (Tools.Animation)ToolManager.Tool; }
        }

        /// <summary>
        /// Gets the colour.
        /// </summary>
        /// <value>The colour.</value>
        public Vector3 Colour
        {
            get { return new Vector3(colour, 0.0f, colour); }
        }

        #endregion

        #region Member Declarations

        /// <summary>
        /// Gets or sets the animation manager.
        /// </summary>
        /// <value>The animation manager.</value>
        public AnimationManager AnimationManager { get; set; }

        /// <summary>
        /// Gets or sets the texture manager.
        /// </summary>
        /// <value>The texture manager.</value>
        public TextureManager TextureManager { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Animation"/> is loading.
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
        /// Initializes a new instance of the <see cref="Animation"/> class.
        /// </summary>
        /// <param name="game">The Game that the game component should be attached to.</param>
        public Animation(Game game)
            : base(game)
        {
            WorldObjects = new List<WorldObject>();

            AnimationManager = new AnimationManager(game.GraphicsDevice, ZMS.Vertex.VertexElements, ZMS.Vertex.SIZE_IN_BYTES);
            TextureManager = new TextureManager(game.GraphicsDevice);

            DrawOrder = MapManager.DRAWORDER_ANIMATION;
        }

        /// <summary>
        /// Adds the specified ifo object.
        /// </summary>
        /// <param name="ifoObject">The ifo object.</param>
        public void Add(IFO.BaseIFO ifoObject)
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
        public void Add(int id, IFO.BaseIFO ifoObject, bool removePrevious)
        {
            string[] objectRow = FileManager.STBs["LIST_MORPH_OBJECT"].Cells[ifoObject.ObjectID].ToArray();

            Matrix objectWorld = Matrix.CreateFromQuaternion(ifoObject.Rotation) *
                                 Matrix.CreateScale(ifoObject.Scale) *
                                 Matrix.CreateTranslation(ifoObject.Position);

            int modelID = AnimationManager.Add(objectRow[2], objectRow[3]);

            WorldObject newObject = new WorldObject()
            {
                Entry = ifoObject,
                ModelID = modelID,
                World = objectWorld,
                TextureID = TextureManager.Add(objectRow[4], new TextureManager.RenderState()
                {
                    Alpha = 1.0f,
                    AlphaReference = 0x00000000,
                    AlphaEnabled = true,
                    AlphaTestEnabled = true,
                    AlphaRefEnabled = true,
                    BlendFunction = BlendFunction.Add,
                    AlphaFunction = CompareFunction.GreaterEqual,
                    SourceBlend = TextureManager.BlendingMode(Convert.ToInt32(objectRow[10])),
                    DestinationBlend = TextureManager.BlendingMode(Convert.ToInt32(objectRow[11])),
                    Cull = CullMode.None,
                    ZTestEnabled = true,
                    ZWriteEnabled = true,
                }),
                BoundingBox = AnimationManager.CreateBox(modelID, objectWorld)
            };

            if (removePrevious)
                WorldObjects[id].Remove(AnimationManager);

            WorldObjects[id] = newObject;
            WorldObjects[id].Add(AnimationManager);

            if (removePrevious)
            {
                for (int i = 0; i < UndoManager.Commands.Length; i++)
                {
                    if (UndoManager.Commands[i] == null)
                        continue;

                    if ((UndoManager.Commands[i].GetType() == typeof(Commands.Animation.ValueChanged) ||
                         UndoManager.Commands[i].GetType() == typeof(Commands.Animation.Positioned) ||
                         UndoManager.Commands[i].GetType() == typeof(Commands.Animation.Added) ||
                         UndoManager.Commands[i].GetType() == typeof(Commands.Animation.ObjectChanged) ||
                         UndoManager.Commands[i].GetType() == typeof(Commands.Animation.Removed)) && ((Commands.Animation.Animation)UndoManager.Commands[i]).ObjectID == id)
                        ((Commands.Animation.Animation)UndoManager.Commands[i]).Object = newObject;
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
                UndoManager.AddCommand(new Commands.Animation.Removed()
                {
                    ObjectID = id,
                    Object = WorldObjects[id],
                    Entry = WorldObjects[id].Entry
                });
            }

            WorldObjects[id].Entry.Parent.Animation.Remove(WorldObjects[id].Entry);
            WorldObjects[id].Remove(AnimationManager);
            WorldObjects.RemoveAt(id);
        }

        /// <summary>
        /// Clears this instance.
        /// </summary>
        public void Clear()
        {
            WorldObjects.Clear();
            AnimationManager.Clear();
            TextureManager.Clear();
        }

        /// <summary>
        /// Updates the objects.
        /// </summary>
        /// <param name="gameTime">Time elapsed since the last call to Update</param>
        public override void Update(GameTime gameTime)
        {
            if (Loading)
                return;

            AnimationManager.Update(gameTime);
        }

        /// <summary>
        /// Draws the objects.
        /// </summary>
        /// <param name="gameTime">Time passed since the last call to Draw.</param>
        public override void Draw(GameTime gameTime)
        {
            if (Loading || !ConfigurationManager.GetValue<bool>("Draw", "Animation"))
                return;

            if (ToolManager.GetToolMode() == ToolManager.ToolMode.EventTriggers)
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

            Effect shader = ShaderManager.GetShader(ShaderManager.ShaderType.Animation);

            shader.Start("Animation");

            AnimationManager.Draw(TextureManager, shader, true);

            shader.Finish();

            if (ToolManager.GetToolMode() == ToolManager.ToolMode.Animation)
                Tool.Draw(ConfigurationManager.GetValue<bool>("Animation", "DrawBoundingBoxes"));
        }
    }
}