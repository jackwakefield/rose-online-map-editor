using System;
using System.Collections.Generic;
using Map_Editor.Engine.Map;
using Map_Editor.Engine.Models;
using Map_Editor.Engine.RenderManager;
using Map_Editor.Misc;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Map_Editor.Engine.Characters
{
    /// <summary>
    /// Monsters class.
    /// </summary>
    public class Monsters : DrawableGameComponent
    {
        /// <summary>
        /// Spawn status.
        /// </summary>
        public enum SpawnStatus
        {
            /// <summary>
            /// Normal, has monsters.
            /// </summary>
            Normal,

            /// <summary>
            /// Inactive, all monsters are inactive.
            /// </summary>
            Inactive,

            /// <summary>
            /// Empty, no monsters.
            /// </summary>
            Empty
        }

        #region Sub Classes

        /// <summary>
        /// World Object.
        /// </summary>
        public class WorldObject
        {
            #region Sub Classes

            /// <summary>
            /// Part class.
            /// </summary>
            public class Part
            {
                #region Member Declaration

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
                /// Gets or sets a value indicating whether [alpha enabled].
                /// </summary>
                /// <value><c>true</c> if [alpha enabled]; otherwise, <c>false</c>.</value>
                public bool AlphaEnabled { get; set; }

                /// <summary>
                /// Gets or sets the bounding box.
                /// </summary>
                /// <value>The bounding box.</value>
                public BoundingBox BoundingBox { get; set; }

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
                public void Add(ObjectManager objectManager, Matrix world)
                {
                    batchObject = objectManager.AddToBatch(ModelID, TextureID, world, Position, Scale, Rotation, BoundingBox, new ObjectManager.EffectParameter[]
                    {
                        new ObjectManager.EffectParameter()
                        {
                            Type = ObjectManager.KeyType.Bool,
                            Key = "AlphaEnabled",
                            Value = AlphaEnabled
                        }
                    });
                }

                /// <summary>
                /// Removes the object from the specified object manager.
                /// </summary>
                /// <param name="objectManager">Object Manager</param>
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
            public IFO.MonsterSpawn Entry { get; set; }

            /// <summary>
            /// Gets or sets the status.
            /// </summary>
            /// <value>The status.</value>
            public SpawnStatus Status { get; set; }

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
            public List<List<Part>> Parts { get; set; }

            #endregion

            /// <summary>
            /// Adds all objects to the specified object manager.
            /// </summary>
            /// <param name="objectManager">The object manager.</param>
            public void Add(ObjectManager objectManager)
            {
                Parts.ForEach(delegate(List<Part> part)
                {
                    part.ForEach(delegate(Part subPart)
                    {
                        subPart.Add(objectManager, World);
                    });
                });
            }

            /// <summary>
            /// Removes all objects from the specified object manager.
            /// </summary>
            /// <param name="objectManager">The object manager.</param>
            public void Remove(ObjectManager objectManager)
            {
                Parts.ForEach(delegate(List<Part> part)
                {
                    part.ForEach(delegate(Part subPart)
                    {
                        subPart.Remove(objectManager);
                    });
                });
            }
        }

        #endregion

        #region Accessors

        /// <summary>
        /// Gets the tool.
        /// </summary>
        /// <value>The tool.</value>
        public Tools.Monsters Tool
        {
            get { return (Tools.Monsters)ToolManager.Tool; }
        }

        /// <summary>
        /// Gets the colour.
        /// </summary>
        /// <value>The colour.</value>
        public Vector3 Colour
        {
            get { return new Vector3(0.0f, 0.0f, 1.0f); }
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
        /// Gets or sets the empty spawn.
        /// </summary>
        /// <value>The empty spawn.</value>
        public Model EmptySpawn { get; set; }

        /// <summary>
        /// Gets or sets the inactive spawn.
        /// </summary>
        /// <value>The inactive spawn.</value>
        public Model InactiveSpawn { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Monsters"/> is loading.
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
        /// Initializes a new instance of the <see cref="Monsters"/> class.
        /// </summary>
        /// <param name="game">The Game that the game component should be attached to.</param>
        public Monsters(Game game)
            : base(game)
        {
            WorldObjects = new List<WorldObject>();

            ObjectManager = new ObjectManager(game.GraphicsDevice, ZMS.Vertex.VertexElements, ZMS.Vertex.SIZE_IN_BYTES);
            TextureManager = new TextureManager(game.GraphicsDevice);

            EmptySpawn = Game.Content.Load<Model>("Empty");
            InactiveSpawn = Game.Content.Load<Model>("Inactive");

            DrawOrder = MapManager.DRAWORDER_MONSTERS;

            colour = 1.0f;
            increasingColour = false;
        }

        /// <summary>
        /// Adds the specified ifo object.
        /// </summary>
        /// <param name="ifoObject">The ifo object.</param>
        public void Add(IFO.MonsterSpawn ifoObject)
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
        public void Add(int id, IFO.MonsterSpawn ifoObject, bool removePrevious)
        {
            Matrix objectWorld = Matrix.CreateTranslation(ifoObject.Position);

            WorldObject newObject = new WorldObject()
            {
                Entry = ifoObject,
                World = objectWorld,
                Parts = new List<List<WorldObject.Part>>(0),
                Status = (ifoObject.Basic.Count > 0 || ifoObject.Tactic.Count > 0) ? SpawnStatus.Inactive : SpawnStatus.Empty
            };

            int monsterID = -1;

            for (int i = 0; i < ifoObject.Basic.Count; i++)
            {
                if (!FileManager.CHRs["LIST_NPC"].Characters[ifoObject.Basic[i].ID].IsActive)
                    continue;

                monsterID = ifoObject.Basic[i].ID;

                break;
            }

            if (monsterID == -1)
            {
                for (int i = 0; i < ifoObject.Tactic.Count; i++)
                {
                    if (!FileManager.CHRs["LIST_NPC"].Characters[ifoObject.Tactic[i].ID].IsActive)
                        continue;

                    monsterID = ifoObject.Tactic[i].ID;

                    break;
                }
            }

            if (monsterID >= 0)
            {
                float npcScale = 0.0f;

                try
                {
                    npcScale = Convert.ToInt32(FileManager.STBs["LIST_NPC"].Cells[monsterID][5]);
                }
                catch
                {
                    npcScale = 100.0f;
                }
                finally
                {
                    npcScale /= 100.0f;
                }

                objectWorld = Matrix.CreateScale(npcScale) *
                              Matrix.CreateTranslation(ifoObject.Position);

                List<short> chrModels = FileManager.CHRs["LIST_NPC"].Characters[monsterID].Models;

                newObject = new WorldObject()
                {
                    Entry = ifoObject,
                    World = objectWorld,
                    Parts = new List<List<WorldObject.Part>>(chrModels.Count),
                    Status = SpawnStatus.Normal
                };

                chrModels.ForEach(delegate(short characterID)
                {
                    ZSC.Object zscObject = FileManager.ZSCs["PART_NPC"].Objects[characterID];

                    List<WorldObject.Part> newObjectList = new List<WorldObject.Part>(zscObject.Models.Count);

                    zscObject.Models.ForEach(delegate(ZSC.Object.Model model)
                    {
                        int modelID = ObjectManager.Add(FileManager.ZSCs["PART_NPC"].Models[model.ModelID]);

                        if (modelID == -1)
                            return;

                        Matrix world = Matrix.CreateFromQuaternion(model.Rotation) *
                                       Matrix.CreateScale(model.Scale) *
                                       Matrix.CreateTranslation(model.Position);

                        int textureID;

                        newObjectList.Add(new WorldObject.Part()
                        {
                            TextureID = textureID = TextureManager.Add(FileManager.ZSCs["PART_NPC"].Textures[model.TextureID]),
                            ModelID = modelID,
                            Position = model.Position,
                            Scale = model.Scale,
                            Rotation = model.Rotation,
                            AlphaEnabled = TextureManager[textureID].RenderState.AlphaEnabled,
                            BoundingBox = ObjectManager.CreateBox(modelID, world * objectWorld)
                        });

                        if (newObject.BoundingBox.Min == Vector3.Zero)
                            newObject.BoundingBox = newObjectList[newObjectList.Count - 1].BoundingBox;
                        else
                            newObject.BoundingBox = BoundingBox.CreateMerged(newObject.BoundingBox, newObjectList[newObjectList.Count - 1].BoundingBox);
                    });

                    newObject.Parts.Add(newObjectList);
                });

                newObject.Add(ObjectManager);
            }
            else
            {
                newObject.BoundingBox = BoundingBox.CreateFromSphere(EmptySpawn.Meshes[0].BoundingSphere);

                newObject.BoundingBox = new BoundingBox()
                {
                    Min = Vector3.Transform(newObject.BoundingBox.Min, Matrix.CreateTranslation(ifoObject.Position)),
                    Max = Vector3.Transform(newObject.BoundingBox.Max, Matrix.CreateTranslation(ifoObject.Position))
                }; ;
            }

            if (removePrevious)
                WorldObjects[id].Remove(ObjectManager);

            WorldObjects[id] = newObject;

            if (removePrevious)
            {
                for (int i = 0; i < UndoManager.Commands.Length; i++)
                {
                    if (UndoManager.Commands[i] == null)
                        continue;

                    if ((UndoManager.Commands[i].GetType() == typeof(Commands.Monsters.ValueChanged) ||
                         UndoManager.Commands[i].GetType() == typeof(Commands.Monsters.Positioned) ||
                         UndoManager.Commands[i].GetType() == typeof(Commands.Monsters.Added) ||
                         UndoManager.Commands[i].GetType() == typeof(Commands.Monsters.Removed)) && ((Commands.Monsters.Monster)UndoManager.Commands[i]).ObjectID == id)
                        ((Commands.Monsters.Monster)UndoManager.Commands[i]).Object = newObject;
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
                UndoManager.AddCommand(new Commands.Monsters.Removed()
                {
                    ObjectID = id,
                    Object = WorldObjects[id],
                    Entry = WorldObjects[id].Entry
                });
            }

            WorldObjects[id].Entry.Parent.Monsters.Remove(WorldObjects[id].Entry);
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
        /// Draws the objects.
        /// </summary>
        /// <param name="gameTime">Time passed since the last call to Draw.</param>
        public override void Draw(GameTime gameTime)
        {
            if (Loading || !ConfigurationManager.GetValue<bool>("Draw", "Monsters"))
                return;

            if (ToolManager.GetToolMode() == ToolManager.ToolMode.Monsters)
            {
                if (increasingColour)
                {
                    colour += 0.01f;

                    if (colour >= 1.0f)
                        increasingColour = false;
                }
                else
                {
                    colour -= 0.01f;

                    if (colour <= 0.5f)
                        increasingColour = true;
                }
            }

            Effect shader = ShaderManager.GetShader(ShaderManager.ShaderType.NPC);

            shader.Start("NPC");

            ObjectManager.Draw(TextureManager, shader, true);

            shader.Finish();

            BoundingFrustum boundingFrustum = new BoundingFrustum(CameraManager.View * CameraManager.Projection);

            for(int i = 0; i < WorldObjects.Count; i++)
            {
                if (WorldObjects[i].Status == SpawnStatus.Normal)
                    continue;

                Model monsterModel = (WorldObjects[i].Status == SpawnStatus.Empty) ? EmptySpawn : InactiveSpawn;

                if (!boundingFrustum.OnScreen(WorldObjects[i].BoundingBox))
                    continue;

                foreach (ModelMesh mesh in monsterModel.Meshes)
                {
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.EnableDefaultLighting();

                        if (ToolManager.GetToolMode() == ToolManager.ToolMode.Monsters)
                        {
                            if (Tool.SelectedObject == i)
                                effect.DiffuseColor = new Vector3(1.0f, 1.0f, 1.0f);
                            else if (Tool.HoveredObject == i)
                                effect.DiffuseColor = new Vector3(colour, 0.0f, 0.0f);
                            else
                                effect.DiffuseColor = (WorldObjects[i].Status == SpawnStatus.Empty) ? new Vector3(0.0f, 0.0f, colour) : new Vector3(colour, colour - 0.15f, 0.0f);
                        }
                        else
                            effect.DiffuseColor = (WorldObjects[i].Status == SpawnStatus.Empty) ? new Vector3(0.0f, 0.0f, 1.0f) : new Vector3(1.0f, 0.85f, 0.0f);
                                               

                        Vector3 worldPosition = WorldObjects[i].Entry.Position;

                        effect.World = Matrix.CreateWorld(worldPosition, Vector3.Normalize(worldPosition - CameraManager.Position) * new Vector3(1.0f, 1.0f, 0.0f), Vector3.Backward);
                        effect.View = CameraManager.View;
                        effect.Projection = CameraManager.Projection;
                    }

                    mesh.Draw();
                }
            }
            
            if (ToolManager.GetToolMode() == ToolManager.ToolMode.Monsters)
                Tool.Draw(ConfigurationManager.GetValue<bool>("Monsters", "DrawRadii"));
        }
    }
}