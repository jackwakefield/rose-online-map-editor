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
    /// EventTriggers class.
    /// </summary>
    public class EventTriggers : DrawableGameComponent
    {
        #region Sub Classes

        /// <summary>
        /// WorldObject class.
        /// </summary>
        public class WorldObject
        {
            #region Sub Classes

            /// <summary>
            /// Part class.
            /// </summary>
            public class Part
            {
                #region Member Declarations

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
                /// Gets or sets the bounding box.
                /// </summary>
                /// <value>The bounding box.</value>
                public BoundingBox BoundingBox { get; set; }

                /// <summary>
                /// Gets or sets the position.
                /// </summary>
                /// <value>The position.</value>
                public Vector3 Position { get; set; }

                /// <summary>
                /// Gets or sets the scale.
                /// </summary>
                /// <value>The scale.</value>
                public Vector3 Scale { get; set; }

                /// <summary>
                /// Gets or sets the rotation.
                /// </summary>
                /// <value>The rotation.</value>
                public Quaternion Rotation { get; set; }

                /// <summary>
                /// Gets or sets the batch object.
                /// </summary>
                /// <value>The batch object.</value>
                private ObjectManager.BatchObject batchObject { get; set; }

                #endregion

                /// <summary>
                /// Adds the object to the specified object manager.
                /// </summary>
                /// <param name="objectManager">The object manager.</param>
                /// <param name="world">The world.</param>
                /// <param name="decorationID">The decoration ID.</param>
                public void Add(ObjectManager objectManager, Matrix world, int decorationID)
                {
                    batchObject = objectManager.AddToBatch(ModelID, TextureID, world, Position, Scale, Rotation, BoundingBox);
                }

                /// <summary>
                /// Removes the object from the specified object manager.
                /// </summary>
                /// <param name="objectManager">The object manager.</param>
                public void Remove(ObjectManager objectManager)
                {
                    objectManager.RemoveFromBatch(batchObject);
                }
            }

            #endregion

            #region Member Declarations

            /// <summary>
            /// Gets or sets the entry.
            /// </summary>
            /// <value>The entry.</value>
            public IFO.EventTrigger Entry { get; set; }

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

            #endregion

            #region List Declarations

            /// <summary>
            /// Gets or sets the parts.
            /// </summary>
            /// <value>The parts.</value>
            public List<Part> Parts { get; set; }

            #endregion

            /// <summary>
            /// Adds all objects to the specified object manager.
            /// </summary>
            /// <param name="objectManager">The object manager.</param>
            /// <param name="decorationID">The decoration ID.</param>
            public void Add(ObjectManager objectManager, int decorationID)
            {
                Parts.ForEach(delegate(Part part)
                {
                    part.Add(objectManager, World, decorationID);
                });
            }

            /// <summary>
            /// Removes all objects from the specified object manager.
            /// </summary>
            /// <param name="objectManager">The object manager.</param>
            public void Remove(ObjectManager objectManager)
            {
                Parts.ForEach(delegate(Part part)
                {
                    part.Remove(objectManager);
                });
            }
        }

        #endregion

        #region Accessors

        /// <summary>
        /// Gets the tool.
        /// </summary>
        /// <value>The tool.</value>
        public Tools.EventTriggers Tool
        {
            get { return (Tools.EventTriggers)ToolManager.Tool; }
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
        /// Gets or sets the object manager.
        /// </summary>
        /// <value>The object manager.</value>
        public ObjectManager ObjectManager { get; set; }

        /// <summary>
        /// Gets or sets the texture manager.
        /// </summary>
        /// <value>The texture manager.</value>
        public TextureManager TextureManager { get; set; }

        /// <summary>
        /// Gets or sets the event trigger model.
        /// </summary>
        /// <value>The event trigger model.</value>
        public Model EventTriggerModel { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="EventTriggers"/> is loading.
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
        /// Initializes a new instance of the <see cref="EventTriggers"/> class.
        /// </summary>
        /// <param name="game">The Game that the game component should be attached to.</param>
        public EventTriggers(Game game)
            : base(game)
        {
            EventTriggerModel = Game.Content.Load<Model>("EventTrigger");

            WorldObjects = new List<WorldObject>();

            ObjectManager = new ObjectManager(game.GraphicsDevice, ZMS.Vertex.VertexElements, ZMS.Vertex.SIZE_IN_BYTES);
            TextureManager = new TextureManager(game.GraphicsDevice);

            DrawOrder = MapManager.DRAWORDER_EVENTRIGGERS;
        }

        /// <summary>
        /// Adds the specified ifo object.
        /// </summary>
        /// <param name="ifoObject">The ifo object.</param>
        public void Add(IFO.EventTrigger ifoObject)
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
        public void Add(int id, IFO.EventTrigger ifoObject, bool removePrevious)
        {
            ZSC.Object zscObject = FileManager.ZSCs["EVENT_OBJECT"].Objects[ifoObject.ObjectID];

            Matrix objectWorld = Matrix.CreateFromQuaternion(ifoObject.Rotation) *
                                 Matrix.CreateScale(ifoObject.Scale) *
                                 Matrix.CreateTranslation(ifoObject.Position);

            WorldObject newObject = new WorldObject()
            {
                Entry = ifoObject,
                World = objectWorld,
                Parts = new List<WorldObject.Part>(zscObject.Models.Count),
            };

            for (int i = 0; i < zscObject.Models.Count; i++)
            {
                int modelID = -1;

                if (zscObject.Models[i].Motion != null)
                    modelID = ObjectManager.Add(FileManager.ZSCs["EVENT_OBJECT"].Models[zscObject.Models[i].ModelID], zscObject.Models[i].Motion);
                else
                    modelID = ObjectManager.Add(FileManager.ZSCs["EVENT_OBJECT"].Models[zscObject.Models[i].ModelID]);

                if (modelID == -1)
                    return;

                Matrix world = Matrix.CreateFromQuaternion(zscObject.Models[i].Rotation) *
                               Matrix.CreateScale(zscObject.Models[i].Scale) *
                               Matrix.CreateTranslation(zscObject.Models[i].Position);

                newObject.Parts.Add(new WorldObject.Part()
                {
                    TextureID = TextureManager.Add(FileManager.ZSCs["EVENT_OBJECT"].Textures[zscObject.Models[i].TextureID]),
                    ModelID = modelID,
                    Position = zscObject.Models[i].Position,
                    Scale = zscObject.Models[i].Scale,
                    Rotation = zscObject.Models[i].Rotation,
                    BoundingBox = ObjectManager.CreateBox(modelID, world * objectWorld)
                });

                if (i == 0)
                    newObject.BoundingBox = newObject.Parts[i].BoundingBox;
                else
                    newObject.BoundingBox = BoundingBox.CreateMerged(newObject.BoundingBox, newObject.Parts[i].BoundingBox);
            }

            if (removePrevious)
                WorldObjects[id].Remove(ObjectManager);

            WorldObjects[id] = newObject;
            WorldObjects[id].Add(ObjectManager, id);

            if (removePrevious)
            {
                for (int i = 0; i < UndoManager.Commands.Length; i++)
                {
                    if (UndoManager.Commands[i] == null)
                        continue;

                    if ((UndoManager.Commands[i].GetType() == typeof(Commands.EventTriggers.ValueChanged) ||
                         UndoManager.Commands[i].GetType() == typeof(Commands.EventTriggers.Positioned) ||
                         UndoManager.Commands[i].GetType() == typeof(Commands.EventTriggers.Added) ||
                         UndoManager.Commands[i].GetType() == typeof(Commands.EventTriggers.ObjectChanged) ||
                         UndoManager.Commands[i].GetType() == typeof(Commands.EventTriggers.Removed)) && ((Commands.EventTriggers.EventTrigger)UndoManager.Commands[i]).ObjectID == id)
                        ((Commands.EventTriggers.EventTrigger)UndoManager.Commands[i]).Object = newObject;
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
                UndoManager.AddCommand(new Commands.EventTriggers.Removed()
                {
                    ObjectID = id,
                    Object = WorldObjects[id],
                    Entry = WorldObjects[id].Entry
                });
            }

            WorldObjects[id].Entry.Parent.EventTriggers.Remove(WorldObjects[id].Entry);
            WorldObjects[id].Remove(ObjectManager);
            WorldObjects.RemoveAt(id);
        }

        /// <summary>
        /// Clears this instance.
        /// </summary>
        public void Clear()
        {
            WorldObjects.Clear();
            ObjectManager.Clear();
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

            ObjectManager.Update(gameTime);
        }

        /// <summary>
        /// Draws the objects.
        /// </summary>
        /// <param name="gameTime">Time passed since the last call to Draw.</param>
        public override void Draw(GameTime gameTime)
        {
            if (Loading || !ConfigurationManager.GetValue<bool>("Draw", "EventTriggers"))
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

            for (int i = 0; i < WorldObjects.Count; i++)
            {
                foreach (ModelMesh mesh in EventTriggerModel.Meshes)
                {
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.EnableDefaultLighting();

                        if (ToolManager.GetToolMode() == ToolManager.ToolMode.EventTriggers)
                        {
                            if (Tool.SelectedObject == i)
                                effect.DiffuseColor = new Vector3(1.0f, 1.0f, 1.0f);
                            else if (Tool.HoveredObject == i)
                                effect.DiffuseColor = new Vector3(colour, 0.0f, 0.0f);
                            else
                                effect.DiffuseColor = new Vector3(colour, 0.0f, colour);
                        }
                        else
                            effect.DiffuseColor = new Vector3(1.0f, 0.0f, 1.0f);

                        Vector3 worldPosition = WorldObjects[i].BoundingBox.GetCenter();

                        effect.World = Matrix.CreateWorld(worldPosition, Vector3.Normalize(worldPosition - CameraManager.Position) * new Vector3(1.0f, 1.0f, 0.0f), Vector3.Backward);
                        effect.View = CameraManager.View;
                        effect.Projection = CameraManager.Projection;
                    }

                    mesh.Draw();
                }
            }

            Effect shader = ShaderManager.GetShader(ShaderManager.ShaderType.SimpleTexture);
            shader.SetValue("Addition", Vector4.Zero);

            shader.Start("SimpleTexture");

            ObjectManager.Draw(TextureManager, shader, true);

            shader.Finish();

            if (ToolManager.GetToolMode() == ToolManager.ToolMode.EventTriggers)
                Tool.Draw(ConfigurationManager.GetValue<bool>("EventTriggers", "DrawBoundingBoxes"));
        }
    }
}